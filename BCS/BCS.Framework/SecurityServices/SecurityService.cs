//******************************************************************************
//Description: Contains Actions and functions for SecurityService
//Remarks: SecurityService
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BCS.Framework.Constants;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Utils;
using BCS.Framework.Singleton;
using BCS.Framework.Utilities;
using BCS.Framework.Web;

namespace BCS.Framework.SecurityServices
{
    public static class SecurityService
    {
        public static bool Login(string username, string password,bool rememberAccount)
        {
            Logout();
            if (Authenticate.IsAuthenticated(username, password))
            {
                FormsAuthentication.Initialize();

                var ticket = new FormsAuthenticationTicket(1, Constant.BCS_CURRENT_USER + username + Constant.BCS_MEMBER_SEPARATOR + password , DateTime.Now,
                    DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout), rememberAccount, "",
                    FormsAuthentication.FormsCookiePath);

                string encrypetedTicket = FormsAuthentication.Encrypt(ticket);

                if (!FormsAuthentication.CookiesSupported)
                {
                    //If the authentication ticket is specified not to use cookie, set it in the URL
                    FormsAuthentication.SetAuthCookie(encrypetedTicket, false);
                }
                else
                {
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypetedTicket) { HttpOnly = true ,Path = "/",Expires = ticket.Expiration,Shareable = true};
                    HttpContext.Current.Response.Cookies.Add(authCookie);
                }

                // Write cookie Login Infomation
                if (rememberAccount)
                {
                    var cookie = new HttpCookie(Constant.BCS_LOGIN_INFO);
                    cookie.Values.Add(Constant.BCS_LOGIN_USERNAME, SecurityMethod.Base64Encode(username));
                    cookie.Values.Add(Constant.BCS_LOGIN_PASSWORD, SecurityMethod.Base64Encode(password));
                    cookie.Expires = DateTime.Now.AddDays(30);
                    
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }

                return true;
            }
            return false;
        }

        public static void Logout()
        {
            // Logout
            FormsAuthentication.SignOut();

            //Clear session
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();

            //Clears out Session
            HttpContext.Current.Response.Cookies.Clear();

            // clear authentication cookie
            HttpContext.Current.Response.Cookies.Remove(Constant.BCS_LOGIN_INFO);
            HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            var cookieLoginInfo = HttpContext.Current.Response.Cookies[Constant.BCS_LOGIN_INFO];
            if (cookieLoginInfo != null)
            {
                cookieLoginInfo.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookieLoginInfo);
            }
        }
    }
}
