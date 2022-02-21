using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlfredESK
{
    public struct SearchInfo
    {
        public string province;
        public string district;
        public string neigh;
        public int areaMin;
        public int areaMax;
        public string searchType;
        public string roomNumber;
        public string buildingAge;
        public string floor;

        public SearchInfo(string province, string district, string neigh, int areaMin, int areaMax, string searchType, string roomNumber, string buildingAge, string floor)
        {
            this.province = province;
            this.district = district;
            this.neigh = neigh;
            this.areaMin = areaMin;
            this.areaMax = areaMax;
            this.searchType = searchType;

        }
    }

    //public struct SearchResult
    //{
    //    public readonly string name;
    //    public readonly int area;

    //    public SearchResult(string name, int area)
    //    {
    //        this.name = name;
    //        this.area = area;
    //    }

    //    override public String ToString()
    //    {
    //        return $"{name} | {area}";
    //    }
    //}

    public struct ResultItem
    {
        public readonly string URL;
        public readonly string title;
        public readonly int area;
        public readonly int price;

        public ResultItem(string url, string title, int area, int price)
        {
            this.URL = url;
            this.title = title;
            this.area = area;
            this.price = price;
        }

    }

    public class Alfred
    {
        private ChromeDriver driver;
        private void SetDriver()
        {
            ChromeOptions options = new ChromeOptions();
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            //options.AddArgument("--headless");
            options.AddArgument("--log-level=3");
            driver = new ChromeDriver(service, options);
            driver.Manage().Window.Maximize();

        }
        private string CutComma(string text)
        {
            string hold = string.Empty;
            foreach (var item in text)
            {
                if (item == ',') break;
                hold += item;

            }
            return hold;
        }
        private IWebElement FindElement_Safe(By by)
        {
            Stopwatch sw = new Stopwatch();
            IWebElement element;
            while (true)
            {
                try
                {
                    element = driver.FindElement(by);
                    break;
                }
                catch (Exception)
                {
                    if (sw.ElapsedMilliseconds > 10000)
                        throw;
                }

            }
            return element;
        }
        private IReadOnlyCollection<IWebElement> FindElements_Safe(By by)
        {
            Stopwatch sw = new Stopwatch();
            IReadOnlyCollection<IWebElement> elements;
            while (true)
            {
                try
                {
                    elements = driver.FindElements(by);
                    break;
                }
                catch (Exception)
                {
                    if (sw.ElapsedMilliseconds > 10000)
                        throw;
                }

            }
            return elements;
        }
        private void SafeClick(IWebElement element)
        {
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                try
                {
                    element.Click();
                    break;
                }
                catch (Exception)
                {
                    if (sw.ElapsedMilliseconds > 10000)
                        throw;
                }

            }
        }
        private void SafeType(IWebElement element, string keys)
        {
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                try
                {
                    element.SendKeys(keys);
                    break;
                }
                catch (Exception)
                {
                    if (sw.ElapsedMilliseconds > 10000)
                        throw;
                }

            }
        }


        public enum Website { Sahibinden };
        public Website searchSite;

        public Alfred(Website searchSite = Website.Sahibinden)
        {
            this.searchSite = searchSite;
        }

        public List<ResultItem> Search(SearchInfo info)
        {

            List<ResultItem> resultList = new List<ResultItem>();
            
            SetDriver(); //Chrome için gerekli ayarları yapan fonksiyonun çağrılması.
            
            if(info.searchType.ToLower()=="rental")
                driver.Navigate().GoToUrl("https://www.sahibinden.com/arama/detayli?category=16622"); //Kiralık Emlak arama sayfasının açılması.
            else if (info.searchType.ToLower() == "sale")
                driver.Navigate().GoToUrl("https://www.sahibinden.com/arama/detayli?category=16623"); //Satılık Emlak arama sayfasının açılması.
            
            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[2]/a"))); //il kutucuğunun açılması
            SafeType(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[2]/div/div[2]/input")), info.province);
            
            var elementColTemp = FindElements_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[2]/div/div[3]/div/div/ul/div/ul"));
            foreach (var elementTemp in elementColTemp)
            {
                if (elementTemp.Text.Contains(info.province)) 
                    SafeClick(elementTemp);
            }
            
            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[2]/div/div[3]/div/div[1]/ul/div/ul/li[1]/a")));//"istanbul tümü" seçeneğinin seçilmesi.
            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[3]/a"))); //ilçe kutucuğunun açılması.
            SafeType(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[3]/div/div[2]/input")), info.district);
            
            elementColTemp = driver.FindElementsByClassName("facetedCheckbox"); //Tüm ilçelerin çekilmesi.
            
            foreach (var elementTemp in elementColTemp)
            {
                if (elementTemp.Text.Contains(info.district)) 
                    SafeClick(elementTemp);//fonksiyona verilen ilçe kelimesini içeren seçeğin tıklanması.
            }
            SafeClick(FindElement_Safe(By.ClassName("address-overlay")));//boş bir yere tıklayarak adres kutusunun kapatılması.
            
            
            
            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[4]/a"))); //semt kutucuğunun açılması.
            SafeType(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[1]/td[2]/ul/li[4]/div/div[2]/input")), info.neigh);
            
            elementColTemp = driver.FindElementsByClassName("facetedCheckbox"); //Tüm semtlerin çekilmesi.
            foreach (var elementTemp in elementColTemp)
            {
                if (elementTemp.Text.Contains(info.neigh)) 
                    SafeClick(elementTemp);//fonksiyona verilen ilçe kelimesini içeren seçeğin tıklanması.
            }
            SafeClick(FindElement_Safe(By.ClassName("address-overlay")));//boş bir yere tıklayarak adres kutusunun kapatılması.



            FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[3]/td[2]/dl/dd/input[1]")).SendKeys(info.areaMin.ToString()); //min m2 ayarının girilmesi
            FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[3]/td[2]/dl/dd/input[2]")).SendKeys(info.areaMax.ToString()); //max m2 ayarının girilmesi
            driver.ExecuteScript("window.scrollBy(0,5000)"); //devam butonunun görülebilmesi için sayfada aşağıya scroll yapılması gerekiyor.
            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/p/button"))); //butona tıklanması.
            SafeClick(FindElement_Safe(By.ClassName("address-overlay")));//boş bir yere tıklayarak adres kutusunun kapatılması.




            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[4]/td[2]/dl/dd/div/div/span"))); //oda sayısı butonuna tıklanması
            SafeType(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[4]/td[2]/dl/dd/div/div/div/div[1]/input")), info.roomNumber);
            foreach (var elementTemp in elementColTemp)
            {
                if (elementTemp.Text.Contains(info.roomNumber))
                    SafeClick(elementTemp);//fonksiyona verilen oda sayısını içeren seçeğin tıklanması.
            }
            SafeClick(FindElement_Safe(By.ClassName("address-overlay")));




            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[5]/td[2]/dl/dd/div/div/span"))); //bina yaşı butonuna tıklanması
            SafeType(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[5]/td[2]/dl/dd/div/div/div/div[1]/input")), info.buildingAge);
            foreach (var elementTemp in elementColTemp)
            {
                if (elementTemp.Text.Contains(info.buildingAge))
                    SafeClick(elementTemp);//fonksiyona verilen bina yaşı içeren seçeğin tıklanması.
            }
            SafeClick(FindElement_Safe(By.ClassName("address-overlay")));




            SafeClick(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[6]/td[2]/dl/dd/div/div/span"))); //bulunduğu kat butonuna tıklanması
            SafeType(FindElement_Safe(By.XPath("/html/body/div[4]/div[2]/div[1]/div/div[1]/div/div/form/div/div/table[1]/tbody/tr[6]/td[2]/dl/dd/div/div/div/div[1]/input")), info.floor);
            foreach (var elementTemp in elementColTemp)
            {
                if (elementTemp.Text.Contains(info.floor))
                    SafeClick(elementTemp);//fonksiyona verilen bulunduğu kat içeren seçeğin tıklanması.
            }
            SafeClick(FindElement_Safe(By.ClassName("address-overlay")));



            
            
            bool read = true;
            bool first = true;
            driver.ExecuteScript("window.scrollBy(0,5000)");//yine sayfanın en altına inmek gerekiyor.
            Thread.Sleep(100);
            driver.ExecuteScript("window.scrollBy(0,-500)");//yine sayfanın en altına inmek gerekiyor.
            int resultCount = int.Parse(FindElement_Safe(By.XPath("/html/body/div[4]/div[4]/form/div/div[3]/div[1]/div[2]/div[1]/div[1]/span")).Text.Split()[0]);
            if (resultCount > 20)
            {
                while (true) //benzeri bir yapı fonksiyon haline getirilecek
                {
                    try
                    {
                        FindElement_Safe(By.XPath("/html/body/div[4]/div[4]/form/div/div[3]/div[3]/div[2]/ul/li[2]/a")).Click();
                        break;

                    }
                    catch (Exception)
                    {
                        driver.ExecuteScript("window.scrollBy(0,-200)");//yine sayfanın en altına inmek gerekiyor.
                        Thread.Sleep(100);
                        driver.ExecuteScript("window.scrollBy(0,200)");//yine sayfanın en altına inmek gerekiyor.
                    }

                }
                Thread.Sleep(2000);
            }
            while (read)
            {
                //sayfa navigate.gotourl() ile değil de butona basılarak yönlendirildiği için burada başka bir bekleme fonksiyonu denedim. yoksa yine hata veriyor.
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                var searchResults = FindElements_Safe(By.ClassName("searchResultsItem"));//emlak ilanlarının çekilmesi
                foreach (var searchResult in searchResults)
                {
                    if (string.IsNullOrEmpty(searchResult.Text)) continue; //boş ilanlar geliyor, boş ilanları geçmesi için.
                    string url = searchResult.FindElement(By.ClassName("classifiedTitle")).GetAttribute("href");
                    int area = int.Parse(searchResult.FindElement(By.ClassName("searchResultsAttributeValue")).Text);
                    var lineText = searchResult.Text.Split("\r\n".ToCharArray()); //satırlarına ayırıyor.
                    int price = int.Parse(CutComma(lineText[4]).Replace(" TL", "").Replace(".", ""));
                    string title = lineText[1];
                    resultList.Add(new ResultItem(url, title, area, price));
                }

                if (resultCount > 50)
                {
                    driver.ExecuteScript("window.scrollBy(0,6000)");//yine sayfanın en altına inmek gerekiyor.
                    Thread.Sleep(1000);
                    driver.ExecuteScript("window.scrollBy(0,-500)");//yine sayfanın en altına inmek gerekiyor.
                    while (true) //kaç sayfa varsa sonuna kadar gitmesi için döngü içinde.
                    {
                        try
                        {
                            /* "sonraki sayfa" tuşuna basmak gerekiyor ancak ilk sayfadayken ileri-geri sayfa tuşu 1 tane var, ara sayfalarda ise "önceki sayfa" tuşu da geldiğinden
                             tuş sayısı ikiye çıkıyor. farkı ayırabilmek için aşağıdaki yapıyı kurdum.*/
                            if (first) { SafeClick(FindElement_Safe(By.ClassName("prevNextBut"))); first = false; }
                            else driver.FindElementsByClassName("prevNextBut")[1].Click();
                            Thread.Sleep(1000);
                            break;
                        }
                        catch (StaleElementReferenceException) { } //tuşa basılması sırasında sıklıkla karşılaşılan hataları tuttum.
                        catch (ElementClickInterceptedException) { }
                        catch (ArgumentOutOfRangeException) { read = false; break; }
                        catch (NoSuchElementException) { read = false; break; }

                    }
                }
                else read = false;

            }
            driver.Quit();
            return resultList;
        }
    }
}
