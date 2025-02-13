using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class CompOffViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string NumberOfDays { get; set; }

        public string Reason { get; set; }
        public string FirstName { get; set; }
        public string Status { get; set; }
        public string EmailId { get; set; }
        public string TotalDays { get; set; }
    }
}