using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BCS.Framework.Helper.Session;

namespace BCS.Framework.Web
{
    /// <summary>
    /// Filter Authorize: Check session
    /// </summary>
    public class FilterAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var skipAuthorize = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                                 || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (!skipAuthorize)
            {
                var user = filterContext.HttpContext.Session.CurrentUser();
                if (HttpContext.Current == null || !HttpContext.Current.User.Identity.IsAuthenticated && user == null || user.UserId <= 0)
                {
                    FormsAuthentication.SignOut();
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
            }

             if (filterContext == null)
          {
              throw new ArgumentNullException( "filterContext" );
          }

            base.OnActionExecuting(filterContext);
        }
    }

}
