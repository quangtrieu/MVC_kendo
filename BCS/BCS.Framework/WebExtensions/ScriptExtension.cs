using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BCS.Framework.WebExtensions
{
    /// <summary>
    /// Script provide method to register scripts and css resources
    /// </summary>
    public static class ScriptExtension
    {
        /// <summary>
        /// Resources the specified helper.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static ResourceRegistration Resource(this HtmlHelper helper)
        {
            return new ResourceRegistration(helper);
        }

        /*
        --- From each view / partial view ---
        <% Html.RegisterScriptInclude(Url.Content("../../Scripts/jquery-ui-1.7.2.custom.js")); %>
        
        --- From the main Master/View ---
        <%= Html.RenderScripts() %>
         */
        private static SortedList<int, string> GetRegisteredScriptIncludes()
        {
            var registeredScriptIncludes = HttpContext.Current.Items["RegisteredScriptIncludes"] as SortedList<int, string>;

            if (registeredScriptIncludes == null)
            {
                registeredScriptIncludes = new SortedList<int, string>();
                System.Web.HttpContext.Current.Items["RegisteredScriptIncludes"] = registeredScriptIncludes;
            }

            return registeredScriptIncludes;
        }

        /// <summary>
        /// Register script source file
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="script"></param>
        public static void RegisterScriptInclude(this HtmlHelper htmlhelper, string script)
        {
            var registeredScriptIncludes = GetRegisteredScriptIncludes();
            if (!registeredScriptIncludes.ContainsValue(script))
            {
                registeredScriptIncludes.Add(registeredScriptIncludes.Count, script);
            }
        }

        /// <summary>
        /// Render script include 
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <returns></returns>
        public static MvcHtmlString RenderScripts(this HtmlHelper htmlhelper)
        {
            var registeredScriptIncludes = GetRegisteredScriptIncludes();
            var scripts = new StringBuilder();
            foreach (string script in registeredScriptIncludes.Values)
            {
                scripts.AppendLine("<script src='" + script + "' type='text/javascript'></script>");
            }
            return new MvcHtmlString(scripts.ToString());
        }
    }
}