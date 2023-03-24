using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class SalarySlipViewModel
    {
        public int Id { get; set; } 

        public string FullName { get; set; }

        public int WorkingDays { get; set; }

        public string Salary { get; set;}

        public int LeaveWithoutpay { get; set; }

        public DateTime Date { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public DateTime HolidaysDate { get; set; }

        public List<SalarySlipViewModel> salarySlipViews { get; set; }

        public string Type { get; set; } 

        public string EmpId { get; set; }

        public string Designation { get; set; }

        public string DateOfJoining { get; set; }
    }
}