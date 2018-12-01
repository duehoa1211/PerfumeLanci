using System.Web;
using System.Web.Optimization;

namespace Application
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
                      "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/fontawesome/all.min.css",
                      "~/Content/site.css",
                      "~/Content/style.css"));

            bundles.Add(new StyleBundle("~/Admin/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/fontawesome/all.min.css",
                      "~/Content/dataTables.bootstrap4.css",
                      "~/Content/sb-admin.min.css"
                ));
            bundles.Add(new ScriptBundle("~/Admin/Script/js").Include(
                      "~/Scripts/dataTables.bootstrap4.js",
                      "~/Scripts/sb-admin.js"
                ));

            bundles.Add(new ScriptBundle("~/ckeditor").Include(
                "~/Scripts/ckeditor/ckeditor.js"
                ));
        }
    }
}
