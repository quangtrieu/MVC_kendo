using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Data;
using BCS.Framework.Helper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.DataAccess
{
    public class BudgetItemDal : BaseDal<ADOProvider>, IDataAccess<BudgetItem>
    {
        public BudgetItem Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by id
                var result = UnitOfWork.Procedure<BudgetItem>("das_BudgetItem_GetById", new { BudgetItemId = id }).FirstOrDefault();

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

        public IList<BudgetItem> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(BudgetItem obj, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@XML", XmlHelper.SerializeXML<BudgetItem>(obj));
                param.Add("@BudgetItemId", obj.BudgetItemId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save budget
                UnitOfWork.ProcedureExecute("das_BudgetItem_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@BudgetItemId");
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

        public IList<BudgetItem> GetBudgetItemByBudgetTabId(int budgetId, int budgetTabId, string budgetItemRow, int userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@BudgetId", budgetId);
                param.Add("@BudgetTabId", budgetTabId);
                param.Add("@BudgetItemRow", budgetItemRow);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by user id
                var result = UnitOfWork.Procedure<BudgetItem>("das_BudgetItem_GetByBudgetTabId", param).ToList();

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


        public void AddDefaultValueByBudgetTab(int budgetId, int budgetTabId, string budgetItemRow, int userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@BudgetId", budgetId);
                param.Add("@BudgetTabId", budgetTabId);
                param.Add("@BudgetItemRow", budgetItemRow);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save budget
                UnitOfWork.ProcedureExecute("das_BudgetItem_AddDefaultByBudgetTab", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Delete all budget item by category setting id
        /// </summary>
        /// <param name="categorySettingId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void DeleteByCategorySettingId(int categorySettingId, int userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@CategorySettingId", categorySettingId);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save budget
                UnitOfWork.ProcedureExecute("das_BudgetItem_DeleteByCategorySettingId", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
