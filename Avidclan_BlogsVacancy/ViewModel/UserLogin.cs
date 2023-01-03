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

        [Required(ErrorMessage ="Please Enter EmailAddress")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
    }
}