using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;
using BCS.Framework.Configuration;

namespace BCS.Web
{

    public class BCSBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    public class CssFixRewriteUrlTransform : IItemTransform
    {
        private static string ConvertUrlsToAbsolute(string baseUrl, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }
            var regex = new Regex("url\\(['\"]?(?<url>[^)]+?)['\"]?\\)");
            return regex.Replace(content, match => string.Concat("url(", RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value), ")"));
        }

        public string Process(string includedVirtualPath, string input)
        {
            if (includedVirtualPath == null)
            {
                throw new ArgumentNullException("includedVirtualPath");
            }
            var directory = VirtualPathUtility.GetDirectory(includedVirtualPath);
            return ConvertUrlsToAbsolute(directory, input);

            //return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }

        private static string RebaseUrlToAbsolute(string baseUrl, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(baseUrl) || url.StartsWith("/", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = string.Concat(baseUrl, "/");
            }
            return VirtualPathUtility.ToAbsolute(string.Concat(baseUrl, url));
        }
    }

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var rewriteUrlTransform = new CssFixRewriteUrlTransform();

            #region Assets

            bundles.Add(new StyleBundle("~/Assets/css/plugin")
                .Include("~/Assets/plugin/bootstrap/css/bootstrap.min.css", rewriteUrlTransform)
                .Include("~/Assets/plugin/kendoui/css/kendo.common-bootstrap.min.css", rewriteUrlTransform)
                .Include("~/Assets/plugin/kendoui/css/kendo.bootstrap.min.css", rewriteUrlTransform)
                //.Include("~/Assets/plugin/kendoui/css/kendo.rtl.min.css", rewriteUrlTransform)
                .Include("~/Assets/plugin/font-awesome/css/font-awesome.min.css", rewriteUrlTransform)
                );
            bundles.Add(new StyleBundle("~/Assets/css/styles")
                .Include("~/Assets/css/base/main.css", rewriteUrlTransform)
                .Include("~/Assets/css/base/popup.css", rewriteUrlTransform)
                .Include("~/Assets/css/base/notification.css", rewriteUrlTransform)
                //.Include("~/Assets/css/base/responsive.css", rewriteUrlTransform)
                );

            bundles.Add(new ScriptBundle("~/Assets/js/core")
                .Include("~/Assets/plugin/jquery/js/jquery-{version}.js")
                .Include("~/Assets/plugin/jquery.form/jquery.form.min.js")
                );
            bundles.Add(new ScriptBundle("~/Assets/js/plugin")
                .Include("~/Assets/plugin/bootstrap/js/bootstrap.min.js")
                .Include("~/Assets/plugin/kendoui/js/kendo.all.min.js")
                .Include("~/Assets/plugin/kendoui/js/kendo.aspnetmvc.min.js")
                .Include("~/Assets/plugin/noty/jquery.noty.js")
                .Include("~/Assets/plugin/noty/layouts/center.js")
                .Include("~/Assets/plugin/noty/layouts/topRight.js")
                .Include("~/Assets/plugin/noty/themes/default.js")
                );
            bundles.Add(new ScriptBundle("~/Assets/js/validation")
                .Include("~/Assets/plugin/validation/jquery.validate*")
                );
            bundles.Add(new ScriptBundle("~/Assets/js/setajs")
                .Include("~/Assets/js/base/setaJs.js")
            );
            bundles.Add(new ScriptBundle("~/Assets/js/seta")
                .Include("~/Assets/js/base/base.js") // using in all page
                .Include("~/Assets/js/base/respond.src.js") // using in all page
                );

            #endregion

            BundleTable.EnableOptimizations = Convert.ToBoolean(Config.GetConfigByKey("EnableOptimizations"));
            foreach (var bundle in BundleTable.Bundles)
            {
                bundle.Orderer = new BCSBundleOrderer();
            }


            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
        }
    }
}
