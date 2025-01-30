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

        [Route("case-studies/clf/")]
        public ActionResult CLFCaseStudyDetail()
        {
            return View();
        }
        //[Route("case-studies/orisuun/")]
        //public ActionResult OrisuunCaseStudyDetail()
        //{
        //    return View();
        //}

        [Route("case-studies/vyking/")]
        public ActionResult VykingCaseStudyDetail()
        
        {
            return View();
        }
       
    }
}