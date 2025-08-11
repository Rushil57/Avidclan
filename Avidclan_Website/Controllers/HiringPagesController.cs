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
        [Route("hire-ai-developers/")]
        public ActionResult HireAIDevelopers()
        {
            return View();
        }
        [Route("hire-Angular-developers/")]
        public ActionResult HireAngularDevelopers()
        {
            return View();
        }
        [Route("hire-reactjs-developers/")]
        public ActionResult HireReactJSDevelopers()
        {
            return View();
        }
        [Route("hire-ml-developers/")]
        public ActionResult HireMachineLearningDevelopers()
        {
            return View();
        }
        [Route("hire-custom-software-developers/")]
        public ActionResult HireCustomSoftwareDevelopers()
        {
            return View();
        }
    }
}