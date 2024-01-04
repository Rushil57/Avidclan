using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Avidclan_Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /*
            Note : Add bellow code for reduce canonicle error in seo.
         */
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    string rawUrl = HttpContext.Current.Request.RawUrl;

        //    if (!rawUrl.EndsWith("/") && !System.IO.Path.HasExtension(rawUrl))
        //    {
        //        HttpContext.Current.Response.RedirectPermanent(rawUrl + "/");
        //        HttpContext.Current.Response.End();
        //    }
        //}
    }
}
