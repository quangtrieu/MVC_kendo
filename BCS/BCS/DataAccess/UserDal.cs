using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Data;
using BCS.Framework.Helper;
using BCS.Framework.SecurityServices.Utils;
using BCS.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BCS.DataAccess
{
    public class UserDal : BaseDal<ADOProvider>, IDataAccess<User>, IUser
    {
        public User Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get user by id
                var result = UnitOfWork.Procedure<User>("acc_User_GetByUserId", new { UserId = id }).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public IList<User> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(User obj, int userID)
        {
            try
            {
                if (obj.UserId == 0)
                {
                    obj.Password = SecurityMethod.MD5Encrypt(obj.Password);
                }

                var param = new DynamicParameters();
                param.Add("@XML", XmlHelper.SerializeXML<User>(obj));
                param.Add("@UserId", userID);
                param.Add("@NewUserId", obj.UserId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save user
                UnitOfWork.ProcedureExecute("acc_User_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@NewUserId");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public User GetUserByTokenId(string id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get user by token
                var user = UnitOfWork.Procedure<User>("acc_User_GetByToken", new { TokenId = id }).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public IList<User> GetUserByRestCode(int userID, string restCode)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get user by token
                var result = UnitOfWork.Procedure<User>("acc_User_GetByRestCode", new { UserId = userID, RestCode = restCode }).ToList();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public IList<User> GetUserByRole(int userID, int systemId, int roleId, int systemAdminUserId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get user by token
                var result = UnitOfWork.Procedure<User>("acc_User_GetByRoleId", new { UserId = userID, SystemId = systemId, RoleId = roleId, SystemAdminUserId = systemAdminUserId }).ToList();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Method: check duplicate user name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckDuplicateUserName(int userId, string userName)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@UserName", userName);
                param.Add("@Result", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store check duplicate user name
                UnitOfWork.ProcedureExecute("acc_User_CheckDuplicateUserName", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.GetDataOutput<bool>("@Result");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckDuplicateEmail(int userId, string email)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@Email", email);
                param.Add("@Result", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store check duplicate user name
                UnitOfWork.ProcedureExecute("acc_User_CheckDuplicateEmail", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.GetDataOutput<bool>("@Result");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<User> GetSSPMember(int userID, int systemId, int roleId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get user by token
                var result = UnitOfWork.Procedure<User>("acc_User_GetSSPMember", new { UserId = userID, SystemId = systemId, RoleId = roleId }).ToList();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
