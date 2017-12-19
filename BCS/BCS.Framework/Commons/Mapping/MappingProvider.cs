using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BCS.Framework.Commons.Mapping
{
    /// <author>Yazeed Hamdan</author>
    /// <summary>
    /// Entry Point for the Client code to map the properties
    /// </summary>
    internal static class MappingProvider
    {
        /// <summary>
        /// Map Properties between two objects
        /// </summary>
        /// <param name="entity">Entity received from the client code, <see cref="System.Object"/></param>
        /// <param name="LINQEntity">Entity retrieved from DB</param>
        /// <param name="LINQProperty"><see cref="System.Reflection.PropertyInfo"/> from LINQ entity
        /// retrieved from DB to be mapped</param>
        /// <remarks>This class will get the provider dynamically and will map the properties
        /// using that provider, so if you want to implement your own provider, you dont
        /// have to modify anything in the code</remarks>
        public static void MapProperties(object entity, object LINQEntity, PropertyInfo LINQProperty)
        {
            IPropertyMappingProvider provider = null;

            //Get All Types in the current assembly which have MappingPropertyTypeNameAttribute defined
            Type[] currentProviders = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(MappingPropertyTypeNameAttribute),false).ToArray().Length > 0).ToArray();

            if (null != currentProviders && currentProviders.Length > 0)
            {
                //Get the provider type,first try to get from its type
                //the mechanism used is to get the MappingPropertyTypeNameAttribute and compare
                //the string defined there with the LINQProperty type
                Type providerType = currentProviders.Where(p =>
                    (p.GetCustomAttributes(typeof(MappingPropertyTypeNameAttribute), false).ToArray()[0]
                    as MappingPropertyTypeNameAttribute).PropertyTypeName == LINQProperty.PropertyType.ToString()).SingleOrDefault();

                //if no provider found,Try to get it from comparing LINQproperty base type with MappingPropertyTypeNameAttribute
                if(null == providerType)
                    providerType = currentProviders.Where(p =>
                    (p.GetCustomAttributes(typeof(MappingPropertyTypeNameAttribute), false).ToArray()[0]
                    as MappingPropertyTypeNameAttribute).PropertyTypeName == LINQProperty.PropertyType.BaseType.ToString()).SingleOrDefault();
                
                if (null != providerType)
                {
                    //Call the provider factory to get our instance
                    provider = ProviderFactory.CreatePropertyMappingProvider(providerType);
                    //Map Properties
                    provider.MapProperties(entity, LINQEntity, LINQProperty);
                }
            }
        }
    }
}
