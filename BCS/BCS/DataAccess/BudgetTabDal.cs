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
    public class BudgetTabDal : BaseDal<ADOProvider>, IDataAccess<BudgetTab>
    {
        public BudgetTab Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by id
                var result = UnitOfWork.Procedure<BudgetTab>("das_BudgetTab_GetById", new { BudgetTabId = id }).FirstOrDefault();

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

        public IList<BudgetTab> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(BudgetTab obj, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@XML", XmlHelper.SerializeXML<BudgetTab>(obj));
                param.Add("@BudgetTabId", obj.BudgetTabId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save budget tab
                UnitOfWork.ProcedureExecute("das_BudgetTab_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@BudgetTabId");
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

        public IList<BudgetTab> GetBudgetTabByBudgetId(int budgetId, int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by user id
                var result = UnitOfWork.Procedure<BudgetTab>("das_BudgetTab_GetByBudgetId", new { BudgetId = budgetId, UserId = userId }).ToList();

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

        public bool DeleteBudgetTabByBudgetId(int budgetId, int userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@BudgetId", budgetId);
                param.Add("@Result", null, DbType.Boolean, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store delete budget tab
                UnitOfWork.ProcedureExecute("das_BudgetTab_DeleteByBudgetId", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<bool>("@Result");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
