using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_Website.Models
{
    public class CandidateDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Resume { get; set; }
        public string Message { get; set; }

        public string Position { get; set; }

        public HttpPostedFileBase file { get; set; }
    }
}