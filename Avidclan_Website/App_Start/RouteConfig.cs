using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Avidclan_Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
            name: "BlogPages",
            url: "blog/{id}",
            defaults: new { controller = "BlogPages", action = "blog" }
        );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Avidclan", action = "AvidclanAction", id = UrlParameter.Optional }
            );
            
        }
    }
}
