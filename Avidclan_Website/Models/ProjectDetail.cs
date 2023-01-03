using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_Website.Models
{
    public class ProjectDetail
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Service { get; set; }
        public string Budget { get; set; }
        public string StartDate { get; set; }
        public string Requirement { get; set; }
        public string ProjectDetails { get; set; }
        public int CountryCode { get; set; }
    }
}