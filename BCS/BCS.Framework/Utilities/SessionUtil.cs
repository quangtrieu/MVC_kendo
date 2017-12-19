using System.Web;

namespace BCS.Framework.Utilities
{
    public static class SessionUtil
    {
        public static void SetValue(string name, string value)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session[name] == null)
            {
                context.Session.Add(name, value);
            }
            else
            {
                context.Session[name] = value;
            }
        }

        public static string GetValue(string name)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session[name] != null)
            {
                return context.Session[name].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static void Remove(string name)
        {
            HttpContext context = HttpContext.Current;
            context.Session.Remove(name);
        }
    }
}
