using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Connection_Task.Extensions
{
    internal static class Extensions
    {
        public static bool isValidChoice(this string choice)
        {
            if (choice.ToLower() == "y" || choice.ToLower() == "n")
                return true;
            return false;
        }

        public static bool isEnoughArea(this decimal countryArea, decimal cityArea, decimal sumOfCities) 
        {
            if (countryArea >= cityArea + sumOfCities)
                return true;
            return false;
        }
    }
}
