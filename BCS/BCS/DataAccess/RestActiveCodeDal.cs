using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Data;
using BCS.Framework.Helper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BCS.DataAccess
{
    public class RestActiveCodeDal : BaseDal<ADOProvider>, IDataAccess<RestActiveCode>
    {
        /// <summary>
        /// Method get rest active code by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RestActiveCode Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get rest active code by id
                var result = UnitOfWork.Procedure<RestActiveCode>("acc_RestActiveCode_GetById", new { RestActiveCodeId = id }).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Method get rest active code by token id
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public RestActiveCode GetByToken(string tokenId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get rest active code by token
                var result = UnitOfWork.Procedure<RestActiveCode>("acc_RestActiveCode_GetByToken", new { TokenId = tokenId }).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Get rest active code by param
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalRecord"></param>
        /// <returns></returns>
        public IList<RestActiveCode> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert/update
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int Save(RestActiveCode obj, int userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@XML", XmlHelper.SerializeXML<RestActiveCode>(obj));
                param.Add("@RestActiveCodeId", obj.RestActiveCodeId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save rest active code
                UnitOfWork.ProcedureExecute("acc_RestActiveCode_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@RestActiveCodeId");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        /// <summary>
        /// Delete rest active code by id
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method get rest active code list by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<RestActiveCode> GetRestCodeByUserId(int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get rest active code by token
                var result = UnitOfWork.Procedure<RestActiveCode>("acc_RestActiveCode_GetByUserId", new { UserId = userId }).ToList();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Get rest active code by user id and rest code
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="restCode"></param>
        /// <returns></returns>
        public RestActiveCode GetByUserIdAndRestCode(int userId, string restCode)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@RestCode", restCode);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store save rest active code
                var result = UnitOfWork.Procedure<RestActiveCode>("acc_RestActiveCode_GetByUserIdAndRestCode", param).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Set default rest active code by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="restCode"></param>
        /// <returns></returns>
        public bool UpdateDefaultRestCodeByUser(int userId, string restCode)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@RestCode", restCode);
                param.Add("@Result", null, DbType.Boolean, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save rest active code
                UnitOfWork.ProcedureExecute("acc_RestActiveCode_UpdateDefaultRestCodeByUser", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<bool>("@Result");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Reset not default rest active code by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ResetNotDefaultRestCodeByUserId(int userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@Result", null, DbType.Boolean, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save rest active code
                UnitOfWork.ProcedureExecute("acc_RestActiveCode_ResetNotDefaultRestByUserId", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<bool>("@Result");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
