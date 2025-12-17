using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class UserLogin
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }

        public string RoleName { get; set; }

        public DateTime JoiningDate { get; set; }

        public int ProbationPeriod { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? NoticePeriodDate { get; set; }
    }

    public class UserRegister
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }

        public int ProbationPeriod { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? NoticePeriodDate { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public bool OnBreak { get; set; }
        public string BreakMonth { get; set; }
        public string PaidLeave { get; set; }
        public string SickLeave { get; set; }
        public string CompOffLeave { get; set; }
        public string FinalBalance { get; set; }
        public string SickLeaveFinalBalance { get; set; }
        public string CompensationLeaveFinalBalance { get; set; }
    }

    public class UseridList
    {
        public List<int> Id { get; set; }
    }

}