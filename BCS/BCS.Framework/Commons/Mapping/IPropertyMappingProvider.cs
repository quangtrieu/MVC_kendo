using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BCS.Framework.Commons.Mapping
{
    /// <summary>
    /// Common interface for all Property Mapping Providers
    /// </summary>
    internal interface IPropertyMappingProvider
    {
        /// <summary>
        /// Responsible for mapping the two properties
        /// </summary>
        /// <param name="entity">Entity received from the client code, <see cref="System.Object"/></param>
        /// <param name="LINQEntity">Entity retrieved from DB</param>
        /// <param name="LINQProperty"><see cref="System.Reflection.PropertyInfo"/> from LINQ entity
        /// retrieved from DB to be mapped</param>
        void MapProperties(object entity, object LINQEntity, PropertyInfo LINQProperty);
    }
}
