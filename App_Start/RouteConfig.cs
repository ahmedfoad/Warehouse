using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Warehouse
{
     [ExcludeFromCodeCoverage]
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

            //      routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional },// Parameter defaults
            //    new[] { "Warehouse.Controllers" }
            //);

            //routes.MapRoute(
            //       "DefaultApi", // Route name
            //      "api/{controller}/{action}/{id}", // URL with parameters
            //      new { controller = "Home", action = "Index", id = UrlParameter.Optional },// Parameter defaults
            //      new[] { "Warehouse.Controllers" }
            //  );
        }
    }
}
