using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BCS.Framework.SecurityServices;
using BCS.Framework.Utilities;

namespace BCS.Framework.Web
{
    /// <summary>
    /// Global error handler for application
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute is AllowMultiple = true and users might want to override behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SetaHandleErrorAttribute : HandleErrorAttribute
    {
        public SetaHandleErrorAttribute()
        {
            //Logger.Instance.Info();
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
            {
                return;
            }

            //**************
            // Grab the username from the session. returns null or the username
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                //clear the session
                if (filterContext.HttpContext.Session != null) 
                    filterContext.HttpContext.Session.Abandon();
                
                SecurityService.Logout();
                //redirect to the login page if not already going there
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Security", action = "Login" }));
            }
            //If the user is authenticated, compare the usernames in the session and forms auth cookie
            //WebSecurity.Initialized is true
            else if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                //Do the usernames match?
                if (filterContext.HttpContext.Session.CurrentUser() != null)
                {
                    //If not, log the user out and clear their session
                    SecurityService.Logout();
                    if (filterContext.HttpContext.Session != null) 
                        filterContext.HttpContext.Session.Abandon();
                    //redirect to the login page
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Security", action = "Login" }));
                }
            }
            //If the user is not authenticated, but the session contains a username
            //Security.Initialized is true
            //Security.IsAuthenticated is false
            else if (filterContext.HttpContext.Session.CurrentUser() != null)
            {
                //log the user out (just in case) and clear the session
                SecurityService.Logout();
                if (filterContext.HttpContext.Session != null)
                {
                    //Clear session
                    filterContext.HttpContext.Session.Clear();
                    filterContext.HttpContext.Session.Abandon();
                    filterContext.HttpContext.Session.Abandon();

                    //Clears out Session
                    filterContext.HttpContext.Response.Cookies.Clear();

                    // clear authentication cookie
                    filterContext.HttpContext.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    filterContext.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

                    HttpCookie cookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (cookie != null)
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        filterContext.HttpContext.Response.Cookies.Add(cookie);
                    }
                }

                //redirect to the login page
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Security", action = "Login" }));
            }
            //**************

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information. 
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            Exception exception = filterContext.Exception;

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method), 
            // ignore it.


            int httpCode = new HttpException(null, exception).GetHttpCode();
            if (httpCode != 500)
            {
                if (httpCode == 401)
                {
                    HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, "Error", "AccessDenied");
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "AccessDenied",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model)
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 401;
                }

                if (httpCode == 404)
                {
                    HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, "Error", "NotFound");
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "Not found",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model)
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 404;
                }
                return;
            }



            if (!ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            HandleErrorInfo model1 = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            filterContext.Result = new ViewResult
            {
                ViewName = View,
                MasterName = Master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model1),
                TempData = filterContext.Controller.TempData
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            // Certain versions of IIS will sometimes use their own error page when 
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

        }

        //public override void OnException(ExceptionContext filterContext)
        //{
        //    if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
        //    {
        //        return;
        //    }

        //    if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
        //    {
        //        return;
        //    }

        //    if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
        //    {
        //        return;
        //    }

        //    // if the request is AJAX return JSON else view.
        //    if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        filterContext.Result = new JsonResult
        //        {
        //            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //            Data = new
        //            {
        //                error = true,
        //                message = filterContext.Exception.Message
        //            }
        //        };
        //    }
        //    else
        //    {
        //        var controllerName = (string)filterContext.RouteData.Values["controller"];
        //        var actionName = (string)filterContext.RouteData.Values["action"];
        //        var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

        //        filterContext.Result = new ViewResult
        //        {
        //            ViewName = View,
        //            MasterName = Master,
        //            ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
        //            TempData = filterContext.Controller.TempData
        //        };
        //    }

        //    // log the error using log4net.
        //    //_logger.Error(filterContext.Exception.Message, filterContext.Exception);

        //    filterContext.ExceptionHandled = true;
        //    filterContext.HttpContext.Response.Clear();
        //    filterContext.HttpContext.Response.StatusCode = 500;

        //    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        //}
    }
}