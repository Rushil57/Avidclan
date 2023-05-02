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
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
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