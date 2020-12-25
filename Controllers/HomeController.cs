using Warehouse.Models;
using Warehouse.Models.Administration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using Warehouse.CustomFilters;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Net;

namespace Warehouse.Controllers
{
    // [CustomAuthorize]
    // [CustomAuthorize(Roles = "Administrator")]
    //  [CustomAuthorize(Roles = "Administrator&save")]
    public class HomeController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        ErrorViewModel Error = new ErrorViewModel();
 
        [CustomAuthorize]
        public ActionResult Index()
        {


            //string userid = HttpContext.User.Identity.Name;
            //string userid2 = HttpContext.User.Identity.GetUserId();

            ////Get the current claims principal
            //var identity2 = (ClaimsPrincipal)Thread.CurrentPrincipal;

            //// Get the claims values
            //var name = identity2.Claims.Where(c => c.Type == ClaimTypes.Name)
            //                   .Select(c => c.Value).SingleOrDefault();
            //var sid = identity2.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
            //                   .Select(c => c.Value).SingleOrDefault();


            //string input_decimal_number = "1.50";
            //var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
            //if (regex.IsMatch(input_decimal_number))
            //{
            //    string decimal_places = regex.Match(input_decimal_number).Value;
            //}
            //var roundedD = Math.Round(2.5, 0); // Output: 2
            //var roundedE = Math.Round(2.5, 0, MidpointRounding.AwayFromZero); // Output: 3
            return View();
        }

        //public ActionResult Unauthorized()
        //{
        //    return View();
        //}
        public ActionResult About()
        {
            List<Invoice_Company> list = db.Invoice_Company.ToList();
            foreach (var item in list)
            {
                if (item.Invoice_Number <= 0)
                {
                    int Invoice_Number = db.Invoice_Company.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                    item.Invoice_Number = Invoice_Number;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View();
        }
        public JsonResult Statistics(int id)
        {
            //List<StatisticsModel> Statistics = new List<StatisticsModel>();
            //int Month = int.Parse(DateTime.Now.ToString(CultureInfo.GetCultureInfo("ar-SA")).Substring(0, 8).Split('/')[1]);
            //int Year = int.Parse(year);
            //string m;
            //for (int i = 0; i < id; i++)
            //{
            //    if (Month.ToString().Length == 1) m = "0" + Month;
            //    else m = Month.ToString();
            //    StatisticsModel model = new StatisticsModel();
            //    model.الشهر = DateOperation.getHijriMonth(Month) + ":" + Year;
            //    model.الصادر = UniRep.ExMonth(Year + "/" + m);
            //    model.الوارد = UniRep.ImMonth(Year + "/" + m);
            //    Statistics.Add(model);
            //    Month--;
            //    if (Month == 0) { Month = 12; Year--; }
            //}
            //return Json(Statistics.AsEnumerable().Reverse(), JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}