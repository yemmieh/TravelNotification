using System;
using System.Web;
using System.Web.Optimization;

namespace TravelNotification
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include("~/Content/themes/base/all.css"));
            bundles.Add(new StyleBundle("~/Content/jqueryui").Include("~/Content/themes/flick/all.css"));

            bundles.Add(new StyleBundle("~/Content/cssjqryUi").Include("~/Content/jquery-ui.css"));




            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/base/js").Include("~/Scripts/Site.js"));

            bundles.Add(new StyleBundle("~/bundles/base/css").Include("~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/flick/js").Include("~/Scripts/Site.js"));

            bundles.Add(new StyleBundle("~/bundles/flick/css").Include("~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css",
                                                                    "~/Content/Site.css", 
                                                                    "~/Content/Mainbody.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js",
            "~/Scripts/jquery-ui.unobtrusive-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
        "~/Content/themes/base/jquery.ui.core.css",
        "~/Content/themes/base/jquery.ui.datepicker.css",
        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/flick/css").Include(
       "~/Content/themes/base/jquery.ui.core.css",
       "~/Content/themes/base/jquery.ui.datepicker.css",
       "~/Content/themes/base/jquery.ui.theme.css")); 




            BundleTable.EnableOptimizations = true;
        }

        public static void ConfigureIgnoreList(IgnoreList ignoreList)
        {
            if (ignoreList == null) throw new ArgumentNullException("ignoreList");

            ignoreList.Clear(); // Clear the list, then add the new patterns.

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            // ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}
