using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BCS.Framework.Commons.Mapping
{
    ///<author>Yazeed Hamdan</author>
    /// <summary>
    /// Responsible for mapping String Values between both properties
    /// </summary>
    [MappingPropertyTypeName("System.String")]
    internal class StringPropertyMappingProvider : PropertyMappingProviderBase
    {
        /// <summary>
        /// Map String Values
        /// </summary>
        /// <param name="entity">Entity received from the client code, <see cref="System.Object"/></param>
        /// <param name="LINQEntity">Entity retrieved from DB</param>
        /// <param name="LINQProperty"><see cref="System.Reflection.PropertyInfo"/> from LINQ entity
        /// retrieved from DB to be mapped</param>
        public override void MapProperties(object entity, object LINQEntity, PropertyInfo LINQProperty)
        {
            base.MapProperties(entity, LINQEntity, LINQProperty);
        }
    }
}
