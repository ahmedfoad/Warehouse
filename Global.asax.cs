using Warehouse.Models.Administration;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using System.Web.Http.WebHost;

namespace Warehouse
{
    public class MvcApplication : System.Web.HttpApplication
    {
        
        
        string[] date = DateTime.Now.ToString(CultureInfo.GetCultureInfo("ar-SA")).Substring(0, 8).Split('/');
        private string[] time = DateTime.Now.TimeOfDay.ToString().Split(':');
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);


            //this for owin security
            UnityConfig.RegisterComponents();

            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultNameClaimType;

            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
      
    }
}
