using System;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.Mvc;
using BCS.Framework.Web;

namespace BCS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new SetaHandleErrorAttribute());
        }
    }
}
