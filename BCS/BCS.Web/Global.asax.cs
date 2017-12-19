using BoC.Web.Mvc.PrecompiledViews;
using Castle.Windsor.Installer;
using BCS.Framework;
using BCS.Web.Installers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BCS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ApplicationPartRegistry.Register(typeof(Provider).Assembly);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BootstrapContainer();
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);

            // If we're an ajax request, and doing a 302, then we actually need to do a 401                  
            if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

        private static void BootstrapContainer()
        {
            Provider.Container.Install(FromAssembly.This());
            ControllerBuilder.Current.SetControllerFactory(Provider.ControllerFactory);
        }
    }
}
