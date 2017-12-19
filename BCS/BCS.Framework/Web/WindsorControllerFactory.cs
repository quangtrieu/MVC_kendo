using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace BCS.Framework.Web
{
    /// <summary>
    /// Register mvc controller to factory throuh via IOC Container
    /// </summary>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                return (IController) kernel.Resolve(controllerType);
            }
            throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}