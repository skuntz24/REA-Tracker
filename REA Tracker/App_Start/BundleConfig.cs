using System.Web;
using System.Web.Optimization;

namespace REA_Tracker
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Getting Css location from DB
            const string default_theme = "~/Content/mvpready-admin-slate.css";
            QVICommonIntranet.Database.REATrackerDB db = new QVICommonIntranet.Database.REATrackerDB();
            string theme = default_theme;
            try
            {
                string command = "SELECT VALUE FROM REA_APPLICATION WHERE VARIABLE='THEME'";
                object value = db.ProcessScalarCommand(command);
                if (value != null)
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath((string)value))))
                    {
                        theme = (string)value;
                    }
                }
            }
            catch
            {
                theme = default_theme;
            }
            finally
            {
                db.CloseConnection();
                db = null;
            }



            //Turn this off when publishing to 64.128.119.75
            //BundleTable.EnableOptimizations = false;
            BundleTable.EnableOptimizations = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/mvpready-core.js",
                      "~/Scripts/mvpready-admin.js",
                      "~/Scripts/Magnific/jquery.magnific-popup.js",
                      "~/Scripts/jquery-ui-1.12.1.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/jquery-ui-1.10.4.custom.css",
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/magnific-popup.css",
                      theme,
                      "~/Content/Site.css"

                      //"~/fonts/Open_Sans/OpenSans.css", //DMF CHANGE
                      //"~/fonts/Oswald/Oswald.css" //DMF CHANGE
                      ));

            bundles.Add(new StyleBundle("~/fonts/css").Include(
                      "~/fonts/OpenSans.css", //DMF CHANGE
                      "~/fonts/Oswald.css" //DMF CHANGE
                      ));

            bundles.Add(new ScriptBundle("~/bundles/personal").Include(
                        "~/Scripts/SelectList.js",
                        "~/Scripts/BuildRelease.js",
                        "~/Scripts/Products.js",
                        "~/Scripts/IssueManager.js",
                        "~/Scripts/ProductHierarchy.js",
                        "~/Scripts/ReportManager.js",
                        "~/Scripts/tablesorter/jquery.tablesorter.combined.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/DnD").Include("~/Scripts/CustomDnD.js"));
            bundles.Add(new ScriptBundle("~/bundles/NewREA").Include("~/Scripts/NewREA.js"));
            bundles.Add(new ScriptBundle("~/bundles/REASearch").Include("~/Scripts/REASearch.js"));
            bundles.Add(new ScriptBundle("~/bundles/BroswerLogging").Include("~/Scripts/BrowserLogging.js"));
            bundles.Add(new ScriptBundle("~/bundles/CodeReview").Include("~/Scripts/CodeReview.js"));
            /*
            bundles.Add(new StyleBundle("~/Content/IG").Include(
                     "~/css/structure/infragistics.css",
                     "~/css/themes/infragistics/infragistics.theme.css"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/IG").Include(
                "~/Scripts/IG/infragistics.core.js",
               "~/Scripts/IG/infragistics.lob.js",
                "~/Scripts/IG/infragistics.dv.js"
                      ));
             * */
        }
    }
}
