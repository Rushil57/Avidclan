using System.Web.Optimization;

namespace Avidclan_Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                          "~/assets/css/Avidclan/bootstrap-custom.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assets/css/responsive.css",
                      "~/assets/css/color.css",
                      "~/Content/site.css"
                      ));

            bundles.Add(new ScriptBundle("~/Content/js").Include(
                    "~/assets/js/isotope.js",
                    "~/assets/js/appear.js",
                    "~/assets/js/wow.js",
                    "~/assets/js/lazyload.js",
                    "~/assets/js/scrollbar.js",
                    "~/assets/js/jClocksGMT.js",
                    "~/assets/js/jquery.rotate.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/js").Include(
            //        "~/assets/js/jquery.js",
            //        "~/assets/js/lazyload.js",
            //        "~/assets/js/owl.js",
            //        "~/assets/js/appear.js",
            //        "~/assets/js/wow.js",
            //        "~/assets/js/scrollbar.js",
            //        "~/assets/js/TweenMax.min.js",
            //        "~/assets/js/jquery.validate.min.js",
            //        "~/assets/js/contact.js",
            //        "~/assets/js/js.cookie.min.js",
            //        "~/assets/js/popper.min.js",
            //        "~/assets/js/isotope.js",
            //        "~/assets/js/swiper.min.js"
            //        ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
