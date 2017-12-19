using BCS.Commons;
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
    public class CategoryDal : BaseDal<ADOProvider>, IDataAccess<Category>
    {
        public Category Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<Category>("set_Category_GetById", new { CategoryId = id }).FirstOrDefault();

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

        public IList<Category> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(Category obj, int userID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userID);
                param.Add("@XML", XmlHelper.SerializeXML<Category>(obj));
                param.Add("@CategoryId", obj.CategoryId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save user
                UnitOfWork.ProcedureExecute("set_Category_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@CategoryId");
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

        public IList<Category> GetCategoryByUserId(int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by user id
                var result = UnitOfWork.Procedure<Category>("set_Category_GetByUserId", new { UserId = userId }).ToList();

                // check category default is not exists call insert to db
                if (result == null || result.Count == 0)
                {
                    // call action insert data default
                    this.CreateDefaultCategorySetting(userId);

                    // call store get category by user id
                    result = UnitOfWork.Procedure<Category>("set_Category_GetByUserId", new { UserId = userId }).ToList();

                    // reset prime cost flag category by name: the exception of Owner’s Salary and Administration
                    var payrollParent = result.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT);
                    var categoryList = result.Where(s => s.ParentCategoryId == payrollParent.CategoryId && (s.CategoryName.Equals("Administration") || s.CategoryName.Equals("Owner's Salary")));
                    foreach (var item in categoryList)
                    {
                        // reset prime cost value is false
                        item.IsPrimeCost = false;

                        // call action save change
                        this.Save(item, userId);
                    }
                }

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

        public IList<Category> GetCategoryByParentId(int userId, int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<Category>("set_Category_GetByParentId", new { UserId = userId, ParentCategoryId = id }).ToList();

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

        public IList<Category> GetCategoryByParentName(string parentName, int userID)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<Category>("set_Category_GetByParentName", new { UserId = userID, CategoryName = parentName }).ToList();

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

        public IList<Category> GetDefaultSectionByUser(int userId, string sectionCode)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store get category by id
                var result = UnitOfWork.Procedure<Category>("set_Category_GetDefaultSectionByUser", new { UserId = userId, DataCode = sectionCode }).ToList();

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

        public bool AddCategoryDefaultByUser(int userId, int parentCategoryId, bool isPrimeCost, bool isTaxCost, string dataCode)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store get category by id
                UnitOfWork.ProcedureExecute("set_Category_AddCategoryDefaultByUser", new { UserId = userId, ParentCategoryId = parentCategoryId, IsPrimeCost = isPrimeCost, IsTaxCost = isTaxCost, DataCode = dataCode });

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool DeleteAllCategoryByUser(int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store get category by id
                UnitOfWork.ProcedureExecute("set_Category_DeleteAllCategoryDefaultByUser", new { UserId = userId });

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// check category default is not exists call insert to db
        /// </summary>
        private void CreateDefaultCategorySetting(int userId)
        {
            // 1. check section default is not exists, after insert to current user.
            var sectionResult = this.GetDefaultSectionByUser(userId, BCSCommonData.CATEGORY_DEFAULT_CODE);

            // 2. check sales category default is not exists, after insert to current user.
            var salesSectionId = sectionResult.FirstOrDefault(s => s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT).CategoryId;
            var salesResult = this.AddCategoryDefaultByUser(userId, salesSectionId, false, false, BCSCommonData.SALES_CATEGORY_DEFAULT_ITEM_CODE);

            // 3. check cogs category default is not exists, after insert to current user.
            var cogsSectionId = sectionResult.FirstOrDefault(s => s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT).CategoryId;
            var cogsResult = this.AddCategoryDefaultByUser(userId, cogsSectionId, true, false, BCSCommonData.COGS_CATEGORY_DEFAULT_ITEM_CODE);

            // 4. check payroll expenses category default is not exists, after insert to current user.
            var payrollSectionId = sectionResult.FirstOrDefault(s => s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT).CategoryId;
            var payrollResult = this.AddCategoryDefaultByUser(userId, payrollSectionId, true, false, BCSCommonData.PAYROLL_CATEGORY_DEFAULT_ITEM_CODE);

            // 5. check payroll before tax category default is not exists, after insert to current user.
            var payrollIsTaxResult = this.AddCategoryDefaultByUser(userId, payrollSectionId, true, true, BCSCommonData.PAYROLL_IS_TAX_CATEGORY_DEFAULT_ITEM_CODE);

            // 6. check payroll expenses category default is not exists, after insert to current user.
            var operationSectionId = sectionResult.FirstOrDefault(s => s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_OPERATION_EXPENSES_TEXT).CategoryId;
            var operationResult = this.AddCategoryDefaultByUser(userId, operationSectionId, false, false, BCSCommonData.OPERATION_CATEGORY_DEFAULT_ITEM_CODE);

            // 7. check profit loss category default is not exists, after insert to current user.
            var profitSectionId = sectionResult.FirstOrDefault(s => s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_PROFIT_LOSS_TEXT).CategoryId;
            var profitResult = this.AddCategoryDefaultByUser(userId, profitSectionId, false, false, BCSCommonData.PROFIT_LOSS_ITEM_CODE);
        }

        /// <summary>
        /// delete all category children by parentCategoryId sales and cogs section
        /// </summary>
        public bool DeleteAllCategoryChildrenByParentId(int parentCategoryId, int userId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store get category by id
                UnitOfWork.ProcedureExecute("set_Category_DeleteAllCategoryChildrenByParentId", new { ParentCategoryId = parentCategoryId, UserId = userId });

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}
