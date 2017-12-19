using BCS.Framework.Constants;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Entity;
using BCS.Framework.Singleton;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BCS.Framework.Web
{
    [SetaAuthorize]
    [SessionExpire]
    public class BaseController : Controller
    {
        /// <summary>
        /// Current user information
        /// </summary>
        public UserInfo CurrentUser
        {
            get
            {
                if (Session[Constant.BCS_CURRENT_USER] == null)
                {
                    var wd = System.Web.HttpContext.Current.User.Identity as FormsIdentity;

                    if (wd != null)
                    {
                        if (wd.IsAuthenticated && wd.Name.IndexOf(Constant.BCS_CURRENT_USER, StringComparison.Ordinal) > -1 && wd.Name.IndexOf(Constant.BCS_MEMBER_SEPARATOR, StringComparison.Ordinal) > -1)
                        {
                            var authens = Regex.Split(wd.Name.Replace(Constant.BCS_CURRENT_USER, string.Empty),
                                 Constant.BCS_MEMBER_SEPARATOR, RegexOptions.None);

                            if (authens.Length >= 3)
                            {
                                var ctx = SingletonIpl.GetInstance<DataProvider>();
                                UserInfo userInfo = ctx.GetUser(authens[0], authens[1]);

                                Session[Constant.BCS_CURRENT_USER] = userInfo;
                            }
                        }
                    }
                    return (UserInfo)Session[Constant.BCS_CURRENT_USER];
                }
                else
                {
                    return (UserInfo)Session[Constant.BCS_CURRENT_USER];
                }
            }
            set
            {
                Session[Constant.BCS_CURRENT_USER] = value;
            }
        }

        /// <summary>
        /// Support user information
        /// </summary>
        //public UserInfo SupportUser
        //{
        //    get
        //    {
        //        return (UserInfo)Session[Constant.BCS_SUPPORT_USER];
        //    }
        //    set
        //    {
        //        Session[Constant.BCS_SUPPORT_USER] = value;
        //    }
        //}

        /// <summary>
        /// Disable client cache
        /// </summary>
        protected void DisablePageCaching()
        {
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }
    }
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
    }
}
