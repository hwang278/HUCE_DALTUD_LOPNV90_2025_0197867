using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarStair.helpers
{
    public static class DoubleUtils
    {
        public static double FeetToMet(this double feet) => feet * 0.304800;
        public static double MetToFeet(this double met) => met * 3.2808399;
        public static double FeetToMet(this int feet) => feet * 0.304800;
        public static double MetToFeet(this int met) => met * 3.2808399;
        public static double MmToFeet(this int mm) => mm * 3.2808399 / 1000.0;
        public static double MmToFeet(this double mm) => mm * 3.2808399 / 1000.0;
        public static double FeetToM2(this double feet) => feet * 0.092903;
        public static double FeetToMm(this double feet) => feet * 0.304800 * 1000;
        public static double FeetToMm(this int feet) => feet * 0.304800 * 1000;



    }
}
