using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.Framework.Commons.Mapping
{
    /// <author>Yazeed Hamdan</author>
    /// <summary>
    /// Attribute specified on a <see cref="IPropertyMappingProvider"/> indicating the type of property
    /// that it maps
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class MappingPropertyTypeNameAttribute : System.Attribute
    {
        private string _propertyTypeName;

        public MappingPropertyTypeNameAttribute(string propertyTypeName)
        {
            _propertyTypeName = propertyTypeName;
        }

        public string PropertyTypeName
        {
            get
            {
                return _propertyTypeName;
            }
            
        }
      
    }
}
