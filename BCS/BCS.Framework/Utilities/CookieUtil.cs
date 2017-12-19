
using System.Web;

namespace BCS.Framework.Utilities
{
    public class CookieUtil
    {
        public static string Get(string cookieName)
        {
            HttpContext context = HttpContext.Current;
            if (context.Request.Cookies[cookieName] != null)
            {
                var httpCookie = context.Request.Cookies[cookieName];
                if (httpCookie != null) return httpCookie.Value;
            }
            return string.Empty;
        }
        
        public static void Set(string cookieName, string value)
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(cookieName, value);
            context.Response.Cookies.Add(cookie);
        }
        
        public static void Set(string cookieName, string value, long expires)
        {
            var context = HttpContext.Current;
            var cookie = new HttpCookie(cookieName, value);
            cookie.Expires.AddSeconds(expires);
            context.Response.Cookies.Add(cookie);
        }
    }
}
