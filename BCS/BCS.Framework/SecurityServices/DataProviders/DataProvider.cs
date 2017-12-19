//******************************************************************************
//Description: Contains functions for DataProvider
//Remarks: DataProvider
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using BCS.Framework.SecurityServices.Entity;
using BCS.Framework.SecurityServices.Utils;
using BCS.Framework.Singleton;

namespace BCS.Framework.SecurityServices.DataProviders
{
    public class DataProvider
    {
        /// <summary>
        /// Get user by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserInfo GetUser(string userName, string password)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();
                var user = ctx.GetUser(userName, SecurityMethod.MD5Encrypt(password));
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public UserInfo GetUserByEmail(string email)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();

                return ctx.GetUserByEmail(email);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserInfo GetUserByUserName(string userName)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();

                return ctx.GetUserByUserName(userName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Validation UserName & Password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsAuthenticated(string userName, string password)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();

                return ctx.IsAuthenticated(userName, SecurityMethod.MD5Encrypt(password));

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// IsExistEmailOfUser
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsExistEmailOfUser(string email)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();

                return ctx.IsExistEmailOfUser(email);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// check exist mail in db by UserId
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsExistEmailOfUserId(long userId, string email )
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();

                return ctx.IsExistEmailOfUserId(userId, email);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /* Using Reset Password*/
        /// <summary>
        /// ResetExpireDate
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forgotExpired"></param>
        /// <param name="fogotCode"></param>
        public void ResetExpireDate(long userId, DateTime? forgotExpired, Guid? fogotCode)
        {
            var ctx = SingletonIpl.GetInstance<SqlDataProvider>();
            ctx.ResetExpireDate(userId, forgotExpired, fogotCode);
        }

        /// <summary>
        /// Get user by forgotCode
        /// </summary>
        /// <param name="forgotCode"></param>
        /// <returns></returns>
        public UserInfo GetUser(Guid forgotCode)
        {
            try
            {
                var ctx = SingletonIpl.GetInstance<SqlDataProvider>();
                return ctx.GetUser(forgotCode);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        public void ResetPassword(long userId, string password)
        {
            var ctx = SingletonIpl.GetInstance<SqlDataProvider>();
            ctx.ResetPassword(userId, SecurityMethod.MD5Encrypt(password));
        }
    }
}
