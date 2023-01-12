using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Avidclan_BlogsVacancy
{
    public static class WebApiConfig
    {
        public static string UrlPrefixRelative { get { return "~/api"; } }
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
