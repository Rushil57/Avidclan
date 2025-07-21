using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class HeaderMenu
    {
        public string MenuName { get; set; }
        public bool Status { get; set; }
    }

    public class UpdatePasswordModel
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }
}