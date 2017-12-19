using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.util;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using RestSharp;
using BCS.Framework.Constants;
using BCS.Framework.Helper.Session;
using BCS.Framework.SecurityServices;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Entity;
using BCS.Framework.Singleton;
using BCS.Framework.Utilities;

namespace BCS.Framework.Web
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Unsealed so that subclassed types can set properties in the default constructor or override our behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SetaAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly object _typeId = new object();

        private string _users;
        private string[] _usersSplit = new string[0];

        public override object TypeId
        {
            get { return _typeId; }
        }

        public new string Users
        {
            get { return _users ?? String.Empty; }
            set
            {
                _users = value;
                _usersSplit = SplitString(value);
            }
        }


        // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var user = httpContext.Session.CurrentUser();

            // Check User is Authorize CurrentUser
            if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.UserName, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            // check role user use controller
            var path = httpContext.Request.Path;
            if (user.RoleId != 3 && (path.Contains("Dashboard") ||
                path.Contains("CategoriesSetting") ||
                path.Contains("Budget") ||
                path.Contains("LinkRestaurant")))
            {
                return false;
            }


            return true;
        }

       

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            // Check User Login
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                SecurityService.Logout();
                filterContext.Result = new RedirectToRouteResult("Default",new RouteValueDictionary(new { controller = "Security", action = "Login" }));
                return;
            }


            //If Authorize Action OK
            if (AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                string guid = Guid.NewGuid().ToString();
                filterContext.Result = new RedirectToRouteResult("Default",
                    new RouteValueDictionary(new { controller = "Home", action = "ViewAccessDenied", id = guid }));
            }

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
        }

        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}
