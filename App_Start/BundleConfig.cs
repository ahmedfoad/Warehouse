using System.Web;
using System.Web.Optimization;

namespace Warehouse
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
              "~/assets/Scripts/Global/calendar/jquery-2.1.4.min.js"
            , "~/assets/Scripts/Global/calendar/jquery.plugin.js"
            , "~/assets/Scripts/Global/calendar/jquery.calendars.js"
            , "~/assets/Scripts/Global/calendar/jquery.calendars.all.js"
            , "~/assets/Scripts/Global/calendar/jquery.calendars.picker.js"
            ,"~/assets/Scripts/Global/calendar/jquery.calendars.ummalqura.js"));
            bundles.Add(new StyleBundle("~/bundles/calendarCSS").Include("~/assets/Scripts/Global/calendar/jquery.calendars.picker.css"));
            bundles.Add(new StyleBundle("~/bundles/angularfancymodalcss").Include("~/assets/layouts/css/angular-fancy-modal.css"));

            bundles.Add(new StyleBundle("~/bundles/GlobalMandatoryStyles").Include(
                  "~/assets/layouts/css/font-awesome.css"
                , "~/assets/layouts/css/simple-line-icons.css"
                , "~/assets/layouts/css/bootstrap-rtl.css"
                , "~/assets/layouts/css/bootstrap-switch-rtl.min.css"));


            BundleTable.EnableOptimizations = true;
        }
    }
}