using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.Methods
{
    public class FrequentMethod
    {
        public int CountWeekends(int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var weekendDays = 0;
            while (firstDayOfMonth <= lastDayOfMonth)
            {
                var weekday = firstDayOfMonth.DayOfWeek.ToString();
                if (weekday == "Saturday" || weekday == "Sunday")
                {
                    weekendDays++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }

            //throw new NotImplementedException();
            return weekendDays;
        }

        public string NumberToWords(int number)
        {
            var units = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tens = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
            {
                return units[number];
            }
            if (number < 100)
            {
                return tens[number / 10] + ((number % 10 > 0) ? " " + NumberToWords(number % 10) : "");
            }
            if (number < 1000)
            {
                return units[number / 100] + " Hundred"
                        + ((number % 100 > 0) ? " And " + NumberToWords(number % 100) : "");
            }
            if (number < 100000)
            {
                return NumberToWords(number / 1000) + " Thousand "
                        + ((number % 1000 > 0) ? " " + NumberToWords(number % 1000) : "");
            }
            if (number < 10000000)
            {
                return NumberToWords(number / 100000) + " Lakh "
                        + ((number % 100000 > 0) ? " " + NumberToWords(number % 100000) : "");
            }
            if (number < 1000000000)
            {
                return NumberToWords(number / 10000000) + " Crore "
                        + ((number % 10000000 > 0) ? " " + NumberToWords(number % 10000000) : "");
            }
            return NumberToWords(number / 1000000000) + " Arab "
                    + ((number % 1000000000 > 0) ? " " + NumberToWords(number % 1000000000) : "");
        }

        public int CountNonWorkingDays(int Joinmonth, int JoinYear, string Date)
        {
            var firstDayOfMonth = new DateTime(JoinYear, Joinmonth, 1);
            var lastDayOfMonth = DateTime.Parse(Date);
            var nonWorkingDays = 0;
            while (firstDayOfMonth < lastDayOfMonth)
            {
                var weekday = firstDayOfMonth.DayOfWeek.ToString();
                if (weekday != "Saturday" && weekday != "Sunday")
                {
                    nonWorkingDays++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }


            return nonWorkingDays;
        }
    }
}