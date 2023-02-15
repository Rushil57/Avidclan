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
    }
}