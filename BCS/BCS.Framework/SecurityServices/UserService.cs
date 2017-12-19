//******************************************************************************
//Description: Contains Actions and functions for UserService
//Remarks: UserService
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Entity;
using BCS.Framework.Singleton;

namespace BCS.Framework.SecurityServices
{
    public class UserService
    {
        /// <summary>
        /// Get user by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static UserInfo GetUser(string userName)
        {
            return new UserInfo();
        }

        /// <summary>
        /// Validation UserName & Password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsAuthenticated(string userName, string password)
        {
            var reval = false;

            var ctx = SingletonIpl.GetInstance<DataProvider>();

            return reval;
        }

        /// <summary>
        /// Validation UserName, Password & restaurantCode
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="resCode"></param>
        /// <returns></returns>
        public static bool IsAuthenticated(string userName, string password, string resCode)
        {
            var reval = false;
            var ctx = SingletonIpl.GetInstance<DataProvider>();
            return reval;
        }

        /// <summary>
        /// Check Restaurant code is Restaurant User
        /// </summary>
        /// <param name="resCode"></param>
        /// <returns></returns>
        public static bool IsExistRestaurantOfUser(string resCode)
        {
            var reval = false;
            var ctx = SingletonIpl.GetInstance<DataProvider>();

            return reval;
        }
    }
}
