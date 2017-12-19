//******************************************************************************
//Description: Contains Actions and functions for SecurityController
//Remarks: SecurityController
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Web.Mvc;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using BCS.Framework.Controllers;

namespace BCS.Framework
{
    /// <summary>
    /// Framework service installer
    /// </summary>
    public class FrameworkInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Install all framework service 
        /// </summary>
        /// <param name="container">IOC container</param>
        /// <param name="store">Configuration store</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
        
            //Register Framework Controller 
            container.Register(AllTypes.FromThisAssembly()
                                .BasedOn<IController>()
                                .If(Component.IsInSameNamespaceAs<SecurityController>())
                                .If(t => t.Name.EndsWith("Controller", StringComparison.CurrentCultureIgnoreCase))
                                .Configure(c => c.LifestyleTransient()));

            //AutoMapper.Mapper.CreateMap<RegionModel, Region>();
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing(LoggerImplementation.Log4net).WithConfig("log4net.config"));
        }
    }
}