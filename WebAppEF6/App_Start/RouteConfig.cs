using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebAppEF6
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                   name: "SeachPage",
                   url: "{controller}/{action}/{id}/{page}/{pagesize}",
                   defaults: new { controller = "Home", action = "Index", page = UrlParameter.Optional, pagesize = UrlParameter.Optional });
        }
    }
}
