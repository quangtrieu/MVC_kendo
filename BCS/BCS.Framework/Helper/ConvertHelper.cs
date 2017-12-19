using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace BCS.Common.Helper
{
    public class ConvertHelper
    {
        public static List<object> ConvertAll<T>(IList<T> fromObjects, System.Type
                                                                  toType)
        {
            if (fromObjects != null)
            {
                var test = Activator.CreateInstance(toType);
                var list = new List<object>();

                foreach (object obj in fromObjects)
                {
                    list.Add(Convert(obj, toType));
                }

                return list;
            }
            return null;
        }

        public static object Convert<T>(T fromObject, System.Type toType)
        {
            if (fromObject == null) return null;
            object returnObject = Activator.CreateInstance(toType);

            PropertyInfo[] infos = returnObject.GetType().GetProperties();
            foreach (PropertyInfo property in infos)
            {
                PropertyMap[] attributes =
                    (PropertyMap[])property.GetCustomAttributes(typeof
                                                              (PropertyMap), false);
                if (!attributes.Any())
                {
                    attributes = new PropertyMap[1];
                    attributes[0] = new PropertyMap(property.Name);
                }
                if (attributes.Length > 0)
                {
                    PropertyInfo fromProperty =
                        fromObject.GetType().
                                   GetProperty(attributes[0].ValueFromProperty);

                    if (fromProperty != null && attributes[0].FromType == null)
                    {
                        property.SetValue(returnObject,
                                          fromProperty.GetValue(fromObject, null), null);
                    }
                    else if (fromProperty != null)
                    {
                        //if (fromProperty.PropertyType.IsArray)
                        if (fromProperty.PropertyType.Name.ToLower().Contains("list"))
                        {
                            var res = ((IList)fromProperty.GetValue(fromObject, null)).Cast<T>().ToList();
                            var newvalue = ConvertAll(res, attributes[0].ToType);
                            property.SetValue(returnObject, newvalue, null);
                        }
                        else
                        {
                            property.SetValue(returnObject,
                                              Convert(fromProperty.GetValue(fromObject,
                                                                            null), property.PropertyType), null);
                        }
                    }
                }
            }

            return returnObject;
        }
        public static List<T2> ConvertList<T1, T2>(IList<T1> fromObjects)
        {
            if (fromObjects != null)
            {
                var list = Activator.CreateInstance<List<T2>>();
                foreach (var obj in fromObjects)
                {
                    list.Add(ConvertObj<T1, T2>(obj));
                }

                return list;
            }
            return null;
        }

        public static T2 ConvertObj<T1, T2>(T1 fromObject)
        {
            var returnObject = Activator.CreateInstance<T2>();

            PropertyInfo[] infos = returnObject.GetType().GetProperties();
            foreach (PropertyInfo property in infos)
            {
                PropertyMap[] attributes =
                    (PropertyMap[])property.GetCustomAttributes(typeof
                                                              (PropertyMap), false);
                if (!attributes.Any())
                {
                    attributes = new PropertyMap[1];
                    attributes[0] = new PropertyMap(property.Name);
                }
                if (attributes.Length > 0)
                {
                    PropertyInfo fromProperty =
                        fromObject.GetType().
                                   GetProperty(attributes[0].ValueFromProperty);

                    if (fromProperty != null && attributes[0].FromType == null)
                    {

                        property.SetValue(returnObject,
                                          fromProperty.GetValue(fromObject, null), null);
                    }
                    else if (fromProperty != null)
                    {
                        //if (fromProperty.PropertyType.IsArray)
                        if (fromProperty.PropertyType.Name.ToLower().Contains("list"))
                        {
                            var res = ((IList)fromProperty.GetValue(fromObject, null)).Cast<T1>().ToList();
                            var newvalue = ConvertList<T1,T2>(res);
                            property.SetValue(returnObject, newvalue, null);
                        }
                        else
                        {
                            property.SetValue(returnObject,
                                              Convert(fromProperty.GetValue(fromObject,
                                                                            null), property.PropertyType), null);
                        }
                    }
                }
            }

            return returnObject;
        }

        public static string ObjectToStringJson(object json)
        {
            return JsonConvert.SerializeObject(json, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Reuse,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                    //Formatting = Newtonsoft.Json.Formatting.Indented,

                });
        }
        public static string ObjectToJson(object json)
        {
            return JsonConvert.SerializeObject(json);
        }

    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyMap : Attribute
    {
        private string _propertyName = string.Empty;
        private System.Type _fromType = null;
        private System.Type _toType = null;

        public PropertyMap(string propertyName)
        {
            this._propertyName = propertyName;
        }

        public PropertyMap(string propertyName, System.Type
                                                                   fromType)
        {
            this._propertyName = propertyName;
            this._fromType = fromType;
        }

        public PropertyMap(string propertyName, System.Type fromType,
                                          System.Type toType)
        {
            this._propertyName = propertyName;
            this._fromType = fromType;
            this._toType = toType;
        }

        public string ValueFromProperty
        {
            get { return this._propertyName; }
        }

        public System.Type FromType
        {
            get { return this._fromType; }
        }

        public System.Type ToType
        {
            get { return this._toType; }
        }
    }
}
