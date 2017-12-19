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
    public class BudgetDal : BaseDal<ADOProvider>, IDataAccess<Budget>
    {
        public Budget Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by id
                var result = UnitOfWork.Procedure<Budget>("das_GetBudgetById", new { BudgetId = id }).FirstOrDefault();

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

        public IList<Budget> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(Budget obj, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@XML", XmlHelper.SerializeXML<Budget>(obj));
                param.Add("@BudgetId", obj.BudgetId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save budget
                UnitOfWork.ProcedureExecute("das_SaveBudget", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@BudgetId");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public bool Delete(int objId, int userID)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store delete budget by id
                var result = UnitOfWork.ProcedureExecute("das_DeleteBudgetById", new { BudgetId = objId, UserId = userID });

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public IList<Budget> GetBudgetByUserId(int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by user id
                var result = UnitOfWork.Procedure<Budget>("das_GetBudgetByUserId", new { UserId = userId }).ToList();

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

        public int CloneBudget(int objId, string budgetName, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@BudgetId", objId);
                param.Add("@BudgetName", budgetName);
                param.Add("@NewBugetId", 0, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save budget
                UnitOfWork.ProcedureExecute("das_Budget_CloneNewBudgetById", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@NewBugetId");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Get all budget deleted by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<Budget> GetBudgetDeletedByUserId(int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by user id
                var result = UnitOfWork.Procedure<Budget>("das_Budgets_GetBudgetDeletedByUserId", new { UserId = userId }).ToList();

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
    }
}
