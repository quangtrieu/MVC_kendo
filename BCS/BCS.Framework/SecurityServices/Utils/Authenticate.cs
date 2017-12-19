//******************************************************************************
//Description: Contains Actions and functions for Authenticate
//Remarks: Authenticate
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using BCS.Framework.Constants;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.Singleton;

namespace BCS.Framework.SecurityServices.Utils
{
    public static class Authenticate
    {
        /// <summary>
        /// Authenticated
        /// </summary>
        /// <param name="username">AccountName</param>
        /// <param name="pwd">Password</param>
        /// <param name="resCode"></param>
        /// <returns>true|false</returns>
        public static bool IsAuthenticated(string username, string pwd)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<DataProvider>();
                if (!ctx.IsAuthenticated(username, pwd))
                {
                    return false;
                }
                //// Check user exist on SSP system
                //HttpContext.Current.Items[Constant.SSP_CURRENT_USER] = ctx.GetUser(username, pwd, resCode);
                //if (HttpContext.Current.Items[Constant.SSP_CURRENT_USER] == null)
                //{
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
