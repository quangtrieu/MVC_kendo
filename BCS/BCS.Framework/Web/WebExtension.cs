using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using BCS.Framework.Constants;
using BCS.Framework.SecurityServices;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Entity;
using BCS.Framework.Singleton;

namespace BCS.Framework.Web
{
    /// <summary>
    /// Web extension 
    /// </summary>
    public static class WebExtension
    {
        /// <summary>
        /// Get current user info from session
        /// </summary>
        /// <param name="session">Http Session</param>
        /// <returns>User Info class</returns>
        public static UserInfo CurrentUser(this HttpSessionStateBase session)
        {
            var wd = HttpContext.Current.User.Identity;

            if (wd == null || wd.Name == null)
                return null;


            if (session[Constant.BCS_CURRENT_USER] == null)
            {
                if (wd.IsAuthenticated && wd.Name.IndexOf(Constant.BCS_CURRENT_USER, StringComparison.Ordinal) > -1 && wd.Name.IndexOf(Constant.BCS_MEMBER_SEPARATOR,StringComparison.Ordinal)>-1)
                {
                    var ctx = SingletonIpl.GetInstance<DataProvider>();
                    var authens = Regex.Split(wd.Name.Replace(Constant.BCS_CURRENT_USER, string.Empty),
                        Constant.BCS_MEMBER_SEPARATOR, RegexOptions.None);

                    if (authens.Length >= 2)
                    {
                        UserInfo userInfo = ctx.GetUser(authens[0], authens[1]);

                        session[Constant.BCS_CURRENT_USER] = userInfo;
                        HttpContext.Current.Items[Constant.BCS_CURRENT_USER] = userInfo;
                        return userInfo;    
                    }
                }
            }
            else
            {
                var userInfo = (UserInfo)session[Constant.BCS_CURRENT_USER];
                if (userInfo != null && userInfo.SupportUser == null)
                {
                    if (wd.IsAuthenticated && wd.Name.IndexOf(Constant.BCS_CURRENT_USER, StringComparison.Ordinal) > -1 &&
                        wd.Name.IndexOf(Constant.BCS_MEMBER_SEPARATOR, StringComparison.Ordinal) > -1)
                    {
                        var authens = Regex.Split(wd.Name.Replace(Constant.BCS_CURRENT_USER, string.Empty),
                            Constant.BCS_MEMBER_SEPARATOR, RegexOptions.None);

                        if (authens.Length >= 2 && !userInfo.UserName.Equals(authens[0]))
                        {
                            var ctx = SingletonIpl.GetInstance<DataProvider>();
                            userInfo = ctx.GetUser(authens[0], authens[1]);
                            session[Constant.BCS_CURRENT_USER] = userInfo;
                        }
                    }
                }
                
            }

           return session[Constant.BCS_CURRENT_USER] as UserInfo;
        }

        /// <summary>
        /// Check user is authenticate
        /// </summary>
        /// <param name="session">Http Session</param>
        /// <returns>True if user is authenticate</returns>
        public static bool IsAuthenticate(this HttpSessionStateBase session)
        {
            var wd = HttpContext.Current.User.Identity as WindowsIdentity;

            return (wd != null && wd.User != null);
        }

        /// <summary>
        /// Get current user info from session
        /// </summary>
        /// <param name="session">Http Session</param>
        /// <returns>User Info class</returns>
        public static UserInfo CurrentUser(this HttpSessionState session)
        {
            var wd = HttpContext.Current.User.Identity;

            if (wd == null || wd.Name == null)
                return null;


            if (session[Constant.BCS_CURRENT_USER] == null)
            {
                if (wd.IsAuthenticated && wd.Name.IndexOf(Constant.BCS_CURRENT_USER, StringComparison.Ordinal) > -1 && wd.Name.IndexOf(Constant.BCS_MEMBER_SEPARATOR, StringComparison.Ordinal) > -1)
                {
                    var ctx = SingletonIpl.GetInstance<DataProvider>();
                    var authens = Regex.Split(wd.Name.Replace(Constant.BCS_CURRENT_USER, string.Empty),
                        Constant.BCS_MEMBER_SEPARATOR, RegexOptions.None);

                    if (authens.Length >= 2)
                    {
                        UserInfo userInfo = ctx.GetUser(authens[0], authens[1]);

                        session[Constant.BCS_CURRENT_USER] = userInfo;
                        HttpContext.Current.Items[Constant.BCS_CURRENT_USER] = userInfo;
                        return userInfo;
                    }
                }
            }
            else
            {
                var userInfo = (UserInfo)session[Constant.BCS_CURRENT_USER];
                if (wd.IsAuthenticated && wd.Name.IndexOf(Constant.BCS_CURRENT_USER, StringComparison.Ordinal) > -1 &&
                    wd.Name.IndexOf(Constant.BCS_MEMBER_SEPARATOR, StringComparison.Ordinal) > -1)
                {
                    var authens = Regex.Split(wd.Name.Replace(Constant.BCS_CURRENT_USER, string.Empty),
                       Constant.BCS_MEMBER_SEPARATOR, RegexOptions.None);

                    if (authens.Length >= 2 && !userInfo.UserName.Equals(authens[0]))
                    {
                        //userInfo = UserService.GetBySId(wd.User.ToString());
                        var ctx = SingletonIpl.GetInstance<DataProvider>();
                        userInfo = ctx.GetUser(authens[0], authens[1]);
                        session[Constant.BCS_CURRENT_USER] = userInfo;
                    }
                }

            }

            return session[Constant.BCS_CURRENT_USER] as UserInfo;
        }

        /// <summary>
        /// Check user is authenticate
        /// </summary>
        /// <param name="session">Http Session</param>
        /// <returns>True if user is authenticate</returns>
        public static bool IsAuthenticate(this HttpSessionState session)
        {
            var wd = HttpContext.Current.User.Identity as WindowsIdentity;

            return (wd != null && wd.User != null);
        }

    }
}