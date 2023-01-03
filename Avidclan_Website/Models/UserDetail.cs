using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_Website.Models
{
    public class UserDetail
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phoneumber { get; set; }
        public string Message { get; set; }

        public int CountryCode { get; set; }
    }
}