﻿using System.Collections;
using System.Web;
using BCS.Framework.Configuration;

namespace BCS.Framework.EF
{
    public class SingletonPerRequest
    {
        /// <summary>
        /// Gets the object per request.
        /// </summary>
        /// <value>
        /// The object per request.
        /// </value>
        public static Hashtable ObjectPerRequest
        {
            get
            {
                return (HttpContext.Current.Items[Config.GetConfigByKey("ObjectPerRequest")] ??
                    (HttpContext.Current.Items[Config.GetConfigByKey("ObjectPerRequest")] =
                    new Hashtable())) as Hashtable;

            }
        }
    }
}
