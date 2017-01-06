using System.Web.Optimization;

namespace NewEnglandBeerMapApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquerybundle").Include(
                        "~/Scripts/jquery-3.1.1.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularbundle").Include(                        
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-sanitize.js",
                        "~/Scripts/angular-recaptcha.js",
                        "~/Scripts/ng-map.js",                                              
                        "~/Scripts/app.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/bootstrapbundle").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/googleanalytics").Include(
                      "~/Scripts/googleAnalytics.js"));

            bundles.Add(new StyleBundle("~/Content/cssbundle").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            BundleTable.EnableOptimizations = true;
        }
    }
}
