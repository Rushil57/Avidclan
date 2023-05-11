using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class BalanaceLeaveViewModel
    {
        public int Id { get; set; }
        public string PersonalLeave { get; set; }

        public string SickLeave { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public bool NoticePeriod { get; set; }
    }
}