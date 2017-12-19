using System.Web.Mvc;

namespace BCS.Framework.WebExtensions
{
    /// <summary>
    /// Create Add style extension for HtmlHelper
    /// </summary>
    public static class StyleExtension
    {
        /// <summary>
        /// Add css style link to page
        /// </summary>
        /// <param name="html"></param>
        /// <param name="styleUrl"></param>
        /// <returns></returns>
        public static MvcHtmlString AddStyle(this HtmlHelper html, string styleUrl)
        {
            string styleTag = string.Format("<link rel='stylesheet' type='text/css' href='{0}' />", styleUrl);
            return MvcHtmlString.Create(styleTag);
        }
    }
}