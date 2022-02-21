using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlfredESK;

namespace ProjeArayuz1
{
    public partial class Form1 : Form
    {
        Alfred esk = new Alfred(Alfred.Website.Sahibinden);

        private bool mouseDown;
        private Point lastLocation;

        public Form1()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Nasıl.BackColor = ButNasıl.BackColor;
            Evoz.BackColor = panel1.BackColor;
            Eks.BackColor = panel1.BackColor;
            kon.BackColor = panel1.BackColor;
            panelny.BringToFront();
            
        }

        private void ButKon_Click(object sender, EventArgs e)
        {
            kon.BackColor = ButKon.BackColor;
            Evoz.BackColor = panel1.BackColor;
            Eks.BackColor = panel1.BackColor;
            Nasıl.BackColor = panel1.BackColor;
            panelkb.BringToFront();
            
        }

        private void ButEvoz_Click(object sender, EventArgs e)
        {
            Evoz.BackColor = ButEvoz.BackColor;
            kon.BackColor = panel1.BackColor;
            Eks.BackColor = panel1.BackColor;
            Nasıl.BackColor = panel1.BackColor;
            paneleo.BringToFront();
            
        }

        private void ButEks_Click(object sender, EventArgs e)
        {
            Eks.BackColor = ButEks.BackColor;
            Evoz.BackColor = panel1.BackColor;
            kon.BackColor = panel1.BackColor;
            Nasıl.BackColor = panel1.BackColor;
            panelam.BringToFront();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //panelsonuc.BringToFront();
            //int m2 = int.Parse(textBox6.Text);
            //String pos = textBox2.Text;
            //SearchInfo info = new SearchInfo(pos, (int)(m2 * 0.9), (int)(m2 * 1.1));
            SearchInfo info = new SearchInfo("İstanbul (Tümü)", "Kağıthane", "Çağlayan Mh.", 90, 100,"rental");
            var results = esk.Search(info);
            //Results'dan gelenlerin fiyat ortalaması alınacak
            //info = new SearchInfo("İstanbul (Tümü)", "Kağıthane", "Çağlayan Mh.", 90, 100, "sale");
            results = esk.Search(info);
            //Result'dan gelenler için ESK hesaplanacak.
            //foreach (var res in results)
            {
                // Tabloya ekle;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
