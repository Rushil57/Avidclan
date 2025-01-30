using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avidclan_Website.Controllers
{
    public class HiringPagesController : Controller
    {
        // GET: HiringPages
       
        [Route("hire-dot-net-developers/")]
        public ActionResult HiredotNetDevelopers()
        {
            return View();
        }

        [Route("hire-ui-ux-designers/")]
        public ActionResult HireUIUXDevelopers()
        {
            return View();
        }

        [Route("hire-qa-automation-tester/")]
        public ActionResult HireAutomationTestingDevelopers()
        {
            return View();
        }

    }
}