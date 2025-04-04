using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avidclan_Website.Controllers
{
    public class CaseStudyController : Controller
    {
        [Route("case-studies/")]
        public ActionResult CaseStudy()
        {
            return View();
        }
        [Route("goolgoalcasestudydetail/")]
        public ActionResult GoolGoalCaseStudyDetail()
        {
            return View();
        }
        [Route("boltwellcasestudydetail/")]
        public ActionResult BoltWellCaseStudyDetail()
        {
            return View();
        }

        [Route("case-studies/concierge-live-feed/")]
        public ActionResult CLFCaseStudyDetail()
        {
            return View();
        }
        //[Route("case-studies/orisuun/")]
        //public ActionResult OrisuunCaseStudyDetail()
        //{
        //    return View();
        //}

        [Route("case-studies/vyking-ship/")]
        public ActionResult VykingCaseStudyDetail()
        
        {
            return View();
        }

        [Route("case-studies/readysetconnect/")]
        public ActionResult RSCCaseStudyDetail()
        {
            return View();
        }

        [Route("case-studies/kathy-kuo-home/")]
        public ActionResult KKHCaseStudyDetail()
        {
            return View();
        }

        [Route("case-studies/iot-water-heater-management/")]
        public ActionResult IOTWaterHeaterCaseStudyDetail()
        {
            return View();
        }

        [Route("case-studies/iot-health-monitor/")]
        public ActionResult IOTMedicationTrackerCaseStudyDetail()
        {
            return View();
        }
    }
}