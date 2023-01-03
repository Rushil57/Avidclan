using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_Website.Models
{
    public class Career
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public string NoOfPosition { get; set; }
        public string Responsibilities { get; set; }
        public string Qualification { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}