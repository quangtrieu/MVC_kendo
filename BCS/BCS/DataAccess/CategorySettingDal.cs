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
    public class CategorySettingDal : BaseDal<ADOProvider>, IDataAccess<CategorySetting>
    {
        public CategorySetting Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<CategorySetting>("set_CategorySetting_GetById", new { CategorySettingId = id }).FirstOrDefault();

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

        public IList<CategorySetting> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(CategorySetting obj, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@XML", XmlHelper.SerializeXML<CategorySetting>(obj));
                param.Add("@CategorySettingId", obj.CategorySettingId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save user
                UnitOfWork.ProcedureExecute("set_CategorySetting_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@CategorySettingId");
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

        public IList<CategorySetting> GetCategorySettingByBudgetId(int id, int userID)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<CategorySetting>("set_CategorySetting_GetByBudgetId", new { UserId = userID, BudgetId = id }).ToList();

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

        public IList<CategorySetting> GetCategorySettingByParentId(int id, int budgetId, int userID)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<CategorySetting>("set_CategorySetting_GetByParentCategoryId", new { UserId = userID, BudgetId = budgetId, ParentCategoryId = id }).ToList();

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

        public bool CheckDuplicateCategorySetting(CategorySetting categorySetting, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@BudgetId", categorySetting.BudgetId);
                param.Add("@CategorySettingId", categorySetting.CategorySettingId);
                param.Add("@ParentCategoryId", categorySetting.ParentCategoryId);
                param.Add("@CategoryName", categorySetting.CategoryName);
                param.Add("@Result", categorySetting.CategorySettingId, DbType.Boolean, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store save user
                UnitOfWork.ProcedureExecute("set_CategorySetting_CheckDuplicate", param);

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

        public IList<CategorySetting> GetCategorySettingByParentName(string categoryName, int budgetId, int userID)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<CategorySetting>("set_CategorySetting_GetByParentName", new { UserId = userID, BudgetId = budgetId, CategoryName = categoryName }).ToList();

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

        public bool DeleteByBudgetId(int budgetId, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@BudgetId", budgetId);
                param.Add("@Result", null, DbType.Boolean, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save user
                UnitOfWork.ProcedureExecute("set_CategorySetting_DeleteByBudgetId", param);

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

        public bool DeleteByParentCategoryId(int budgetId, int parentCategoryId, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@BudgetId", budgetId);
                param.Add("@ParentCategoryId", parentCategoryId);
                param.Add("@UserId", userID);
                param.Add("@Result", null, DbType.Boolean, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save user
                UnitOfWork.ProcedureExecute("set_CategorySetting_DeleteByParentCategoryId", param);

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
