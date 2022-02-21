using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace AlfredESK
{
    internal static class ESKCalculator
    {
        public static double GetIndex(List<RentalResult> searchList)
        {
            double rentEstimate; //belirtilen kriterlerdeki evlerin ortalama kirası
            double rate = 0.1; // ülkedeki faiz oranının %10 olduğunu varsayalım 
            int year = 10;
            double saleEstimate = budget * Math.Pow(1.15, year);

            double realValue = 0;


            for (int i = 0; i < year; i++)
            {
                double divider = Math.Pow((1 + rate), i);
                realValue = realValue + rentEstimate*12 / divider;
            }

            realValue = realValue + saleEstimate / Math.Pow((1 + rate), year);

            return realValue;
        }

        public class RentalResult
        {
        }
    }
}
