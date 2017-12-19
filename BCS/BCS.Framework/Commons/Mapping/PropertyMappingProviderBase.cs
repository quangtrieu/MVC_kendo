using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BCS.Framework.Commons.Mapping
{
    ///<author>Yazeed Hamdan</author>
    /// <summary>
    /// Responsible for mapping between both properties
    /// </summary>
    internal class PropertyMappingProviderBase : IPropertyMappingProvider
    {
        #region IPropertyMappingProvider Members
        /// <summary>
        /// Encapsulates the common functionality of mapping two properties using <see cref="System.Reflection"/>
        /// </summary>
        /// <param name="entity">Entity received from the client code, <see cref="System.Object"/></param>
        /// <param name="LINQEntity">Entity retrieved from DB</param>
        /// <param name="LINQProperty"><see cref="System.Reflection.PropertyInfo"/> from LINQ entity
        /// retrieved from DB to be mapped</param>
        /// <remarks>If you want to create a new Provider, just inherit from this class
        /// and have MappingPropertyTypeNameAttribute set to the type you are providing the mapping
        /// against</remarks>
        public virtual void MapProperties(object entity, object LINQEntity, PropertyInfo LINQProperty)
        {
            object propertyValue = null;
            //Get Property from entity
            PropertyInfo entityProperty = entity.GetType().GetProperty(LINQProperty.Name);
            //Get Value from the property
            if (null != entityProperty)
                propertyValue = entityProperty.GetValue(entity, null);
            //Set LinqEntity to the value retrieved from the entity
            if (null != propertyValue)
                LINQProperty.SetValue(LINQEntity, propertyValue, null);

        }

        #endregion

       
    }
}
