using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DocumentFormat.OpenXml.Drawing;
using BCS.Framework.Constants;
using BCS.Framework.SecurityServices;
using BCS.Framework.SecurityServices.Utils;

namespace BCS.Framework.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public  class SessionExpireAttribute : ActionFilterAttribute
    {
        /* Occurs before the controller action is executed
         * Verifies one of two sitations:
         *   1. If the user is authenticated, the username in the session matches the username in the forms authentication token
         *   2. If the user does not have a forms authentication token, their session should not include any identity information, like a username
         * If any of these cases are violated, then the user will be logged out, their session will be destoryed, and they will be redirected to the login page
         * The following conditions will allow the user to reach the controller action:
         *   1. They do not have a forms auth token, and their session does not contain identity information
         *   2. They have a forms auth token, their session contains an identity, and the usernames match in both the forms auth token and the session
         */
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session[Constant.BCS_CURRENT_USER] == null)
            {
                FormsAuthentication.SignOut();
                SecurityService.Logout();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Login" }, { "controller", "Security" } });
                return;
            } 

            //Grab the username from the session. returns null or the username
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                //clear the session
                if (filterContext.HttpContext.Session != null)
                    filterContext.HttpContext.Session.Abandon();

                FormsAuthentication.SignOut();
                SecurityService.Logout();

                //redirect to the login page if not already going there
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Login" }, { "controller", "Security" } });
            }

            base.OnActionExecuting(filterContext);
        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    HttpContext ctx = HttpContext.Current;

        //    // If the browser session or authentication session has expired...
        //    if (ctx.Session.CurrentUser() == null || !filterContext.HttpContext.Request.IsAuthenticated)
        //    {
        //        //if (filterContext.HttpContext.Request.IsAjaxRequest())
        //        //{
        //        //    // For AJAX requests, we're overriding the returned JSON result with a simple string,
        //        //    // indicating to the calling JavaScript code that a redirect should be performed.
        //        //    filterContext.Result = new JsonResult {Data = "_Logon_"};
        //        //}
        //        //else
        //        //{
        //        //    // For round-trip posts, we're forcing a redirect to Home/TimeoutRedirect/, which
        //        //    // simply displays a temporary 5 second notification that they have timed out, and
        //        //    // will, in turn, redirect to the logon page.
        //        //    filterContext.Result = new RedirectToRouteResult(
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"Controller", "Home"},
        //        //            {"Action", "TimeoutRedirect"}
        //        //        });
        //        //}

        //        filterContext.Result = new RedirectToRouteResult("Default",  new RouteValueDictionary(new { controller = "Security", action = "Login" }));
        //    }

        //    base.OnActionExecuting(filterContext);
        //}
    }
}