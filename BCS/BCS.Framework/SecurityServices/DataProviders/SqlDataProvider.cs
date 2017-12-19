//******************************************************************************
//Description: Contains functions for SqlDataProvider
//Remarks: SqlDataProvider
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Data;
using System.Linq;
using Dapper;
using BCS.Framework.Commons;
using BCS.Framework.Data;
using BCS.Framework.SecurityServices.Entity;

namespace BCS.Framework.SecurityServices.DataProviders
{
    public class SqlDataProvider : BaseDal<ADOProvider>
    {
        /// <summary>
        /// Get user by userName
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public UserInfo GetUserByEmail(string email)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                var user = UnitOfWork.Procedure<UserInfo>("sec_Get_UserByEmail", new { Email = email }).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UserInfo GetUserByUserName(string userName)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false); 
                var user = UnitOfWork.Procedure<UserInfo>("sec_Get_UserByUserName", new { UserName = userName }).FirstOrDefault();
                UnitOfWork.ConnectionString = null;
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get GetUser by UserName, Password & restaurantCode
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserInfo GetUser(string userName, string password)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserName", userName);
                param.Add("@PassWord", password);
                param.Add("@Result", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);
                var user = UnitOfWork.Procedure<UserInfo>("sec_Get_UserAuthenticated", param).FirstOrDefault();
                UnitOfWork.ConnectionString = null;
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Validation UserName & Password & Restaurant
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsAuthenticated(string userName, string password)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserName", userName);
                param.Add("@PassWord", password);
                param.Add("@Result", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);
                UnitOfWork.Procedure<UserInfo>("sec_Get_UserAuthenticated", param);
                UnitOfWork.ConnectionString = null;

                return param.GetDataOutput<bool>("@Result");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check validation email & restaurant
        /// Return 1: Exist email & restaurant with User 
        /// Return 0: Not exist email & restaurant with User
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsExistEmailOfUser(string email)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Email", email);
                param.Add("@Result", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);
                UnitOfWork.Procedure<UserInfo>("sec_IsExistEmailOfUser", param);
                UnitOfWork.ConnectionString = null;

                return param.GetDataOutput<bool>("@Result");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check validation email & restaurant by UserId
        /// Return 1: Exist email & restaurant with UserId 
        /// Return 0: Not exist email & restaurant with UserId
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsExistEmailOfUserId(long userId, string email)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@Email", email);
                param.Add("@Result", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);
                UnitOfWork.Procedure<UserInfo>("sec_IsExistEmailOfUserId", param);
                UnitOfWork.ConnectionString = null;

                return param.GetDataOutput<bool>("@Result");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ResetExpireDate
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forgotExpired"></param>
        /// <param name="fogotCode"></param>
        public void ResetExpireDate(long userId, DateTime? forgotExpired, Guid? fogotCode)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserID", userId);
                param.Add("@ForgotExpired", forgotExpired);
                param.Add("@ForgotCode", fogotCode);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);
                UnitOfWork.Procedure<UserInfo>("sec_ResetExpireDate", param);
                UnitOfWork.ConnectionString = null;
            }
            catch (Exception)
            {
            }
        }

        public UserInfo GetUser(Guid fogotCode)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);
                var user = UnitOfWork.Procedure<UserInfo>("sec_GetUserForgotByCode", new { @ForgotCode = fogotCode }).FirstOrDefault();
                UnitOfWork.ConnectionString = null;

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// ResetExpireDate
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        public void ResetPassword(long userId, string password)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserID", userId);
                param.Add("@PassWord", password);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);
                UnitOfWork.Procedure<UserInfo>("sec_ResetPassword", param);
                UnitOfWork.ConnectionString = null;
            }
            catch (Exception)
            {
            }
        }
    }
}
