using System.Web.Mvc;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using BCS.Framework.Web;

namespace BCS.Framework
{
    /// <summary>
    /// IOC Provider
    /// </summary>
    public static class Provider
    {
        private static readonly IWindsorContainer container;
        private static readonly DefaultControllerFactory controllerFactory;

        static Provider()
        {
            container = new WindsorContainer();

            container.AddFacility<WcfFacility>();

            controllerFactory = new WindsorControllerFactory(container.Kernel);

            container.Install(FromAssembly.Containing(typeof(FrameworkInstaller)));
        }

        public static IWindsorContainer Container
        {
            get { return container; }
        }

        /// <summary>
        /// Resolve component from container
        /// </summary>
        /// <typeparam name="T">interface of component</typeparam>
        /// <returns>implement of component</returns>
        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        /// <summary>
        /// Resolve component from container
        /// </summary>
        /// <typeparam name="T">interface of component</typeparam>
        /// <returns>implement of component</returns>
        public static T Resolve<T>(string key)
        {
            return container.Resolve<T>(key);
        }
        
        public static DefaultControllerFactory ControllerFactory
        {
            get { return controllerFactory; }
        }
    }
}