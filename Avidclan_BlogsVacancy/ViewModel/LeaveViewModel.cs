﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class LeaveViewModel
    {
        public int Id { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }

        public int UserId { get; set; }

        public DateTime LeaveDates { get; set; }

        public string Halfday { get; set; }

        public string LeaveStatus { get; set; }

        public string FirstName { get; set; }

        public string EmailId { get; set; }

        public List<string> ReportingPerson { get; set; }

        public string Persons { get; set; }

        public List<LeaveDetailsViewModel> Leaves { get;set; }
    }

    public class LeaveDetailsViewModel
    {
        public int Id { get; set; }

        public DateTime LeaveDate { get; set; }

        public string Halfday { get; set; }

        public int LeaveId { get; set; }
    }

    public class TypeOfLeave
    {
       // public int SickLeaves { get; set; }

        public string SickLeave { get; set; }
        public string PersonalLeave { get; set; }
        public string LwpLeave { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }
        //public int PersonalLeaves { get; set; }

        //public int Lwp { get; set; }
    }
}