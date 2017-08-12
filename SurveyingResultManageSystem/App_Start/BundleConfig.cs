using System.Web;
using System.Web.Optimization;

namespace SurveyingResultManageSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            //这里自定义文件，最好一个一个的定义
            bundles.Add(new StyleBundle("~/Content/style/FileManager.css").Include("~/Content/style/FileManager.css"));
            bundles.Add(new StyleBundle("~/Content/style/MapManager.css").Include("~/Content/style/MapManager.css"));
            bundles.Add(new StyleBundle("~/Content/style/Layout.css").Include("~/Content/style/Layout.css"));
            bundles.Add(new StyleBundle("~/Content/style/Login.css").Include("~/Content/style/Login.css"));
            bundles.Add(new StyleBundle("~/Content/style/MoreLogInfo.css").Include("~/Content/style/MoreLogInfo.css"));
            bundles.Add(new StyleBundle("~/Content/style/SetPasswordsIndex.css").Include("~/Content/style/SetPasswordsIndex.css"));
            //Scripts

        }
    }
}
