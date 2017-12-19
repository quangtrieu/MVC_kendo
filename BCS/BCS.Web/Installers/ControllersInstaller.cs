using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using System.Web.Mvc;
using BCS.Web.Controllers;
using BCS.Web.Models;
using Notification = Kendo.Mvc.UI.Notification;


namespace BCS.Web.Installers
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Register all controller to IoC
            container.Register(AllTypes.FromThisAssembly()
                                   .BasedOn<IController>()
                                   .If(Castle.MicroKernel.Registration.Component.IsInSameNamespaceAs<HomeController>())
                                   .If(t => t.Name.EndsWith("Controller"))
                                   .Configure(c => c.LifestyleTransient()));

            //Register Webservice
            //string seisanWebserviceAddress = ConfigurationManager.AppSettings["SeisanWebserviceAddress"];
            //container.Register(Component.For<IProductionOrdersWebService>()
            //    .AsWcfClient(WcfEndpoint.BoundTo(new BasicHttpBinding())
            //    .At(seisanWebserviceAddress)));
            CreateModelMapping();
        }

        private void CreateModelMapping()
        {
            //Mapper.CreateMap<NotificationModel, Notification>();
        }
    }
}