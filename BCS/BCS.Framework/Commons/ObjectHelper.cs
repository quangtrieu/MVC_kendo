using System.Collections.Generic;
using System.Reflection;

namespace BCS.Framework.Commons
{
    internal static class ObjectHelper
    {
        public static IDictionary<string, object> TurnObjectIntoDictionary(object data)
        {
            const BindingFlags attr = BindingFlags.Public | BindingFlags.Instance;

            var dict = new Dictionary<string, object>();

            if (data == null) return dict;

            foreach (var property in data.GetType().GetProperties(attr))
            {
                if (property.CanRead)
                {
                    dict.Add(property.Name.ToLower(), property.GetValue(data, null));
                }
            }

            return dict;
        }
    }
}