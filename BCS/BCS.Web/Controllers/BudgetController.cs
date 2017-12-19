using AutoMapper;
using BCS.BusinessLogic;
using BCS.Commons;
using BCS.Entity;
using BCS.Framework.Singleton;
using BCS.Framework.Web;
using BCS.Web.Models;
using iTextSharp.text;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BCS.Web.Controllers
{
    public class BudgetController : BaseController
    {
        private readonly string BUDGET_TAB_NAME_PERIODIC = "Period budget - ";
        private readonly string BUDGET_HEADER_NAME_PERIODIC = "Period ";
        private readonly string OLEDB_Provider = BCS.Framework.Utilities.Utils.GetSetting<string>("OLEDB_Provider", string.Empty);
        private readonly string FORMAT_CURRENCY = "$#,##0.00_);[Red]($#,##0.00)";
        private readonly string FORMAT_PERCENT = "0.00%";//"0.00_)%;[Red](0.00)%";

        // GET: Budget
        public ActionResult Index()
        {
            return View();
        }

        // GET: Budget/NewBudget
        public ActionResult NewBudget(string budgetName)
        {
            // Get rest code default by user
            string restCode = null; ;
            var restActiveCodeByUser = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId);
            if (restActiveCodeByUser.Count > 0 && restActiveCodeByUser.Any(p => p.IsDefault == true))
            {
                restCode = restActiveCodeByUser.FirstOrDefault(p => p.IsDefault == true).RestCode;
            }

            // date time now
            var dateTime = DateTime.Now;

            // create new budget
            Budget budget = new Budget()
            {
                BudgetName = budgetName,
                RestCode = restCode,
                StartDate = null,
                EndDate = null,
                BudgetLengthType = BCSCommonData.BUDGET_TYPE_MONTH,
                BudgetLengthStart = new DateTime(dateTime.Year, 1, 1),
                FiscalYearStartOn = new DateTime(dateTime.Year, 1, 1),
                BudgetLength = BCSCommonData.BUDGET_MONTH_LENGTH_ONE_YEAR,
                ActualNumbersFlg = false,
                VarianceFlg = false,
                InputMethodId = BCSCommonData.INPUT_METHOD_PERCENTAGET,
                Sales = 0,
                SalesPercent = 0,
                COGS = 0,
                COGSPercent = 0,
                GrossProfit = 0,
                GrossProfitPercent = 0,
                PayrollExpenses = 0,
                PayrollExpensesPercent = 0,
                OperatingExpenses = 0,
                OperatingExpensesPercent = 0,
                OperatingProfit = 0,
                OperatingProfitPercent = 0,
                PrimeCost = 0,
                PrimeCostPercent = 0,
                NetProfitLoss = 0,
                NetProfitLossPercent = 0,
                BreakEvenPoint = 0,
                BreakEvenPointPercent = 0,
                DeletedFlg = false,
                CreatedUserId = CurrentUser.UserId,
                CreatedDate = DateTime.Now,
                UpdatedUserId = CurrentUser.UserId,
                UpdatedDate = DateTime.Now
            };

            // call action add new budget
            var budgetId = SingletonIpl.GetInstance<BudgetBll>().Save(budget, CurrentUser.UserId);

            // return result after update budget
            return Json(new { Status = (budgetId > 0), BudgetId = budgetId, RestCode = restCode }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeBudgetName(int? id, string budgetName)
        {
            bool status = false;
            string message = string.Empty;
            try
            {
                // Get budget by id
                var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
                budget.BudgetName = budgetName;
                budget.UpdatedDate = DateTime.Now;
                budget.UpdatedUserId = CurrentUser.UserId;
                
                // call action change budget
                var budgetId = SingletonIpl.GetInstance<BudgetBll>().Save(budget, CurrentUser.UserId);
                if (budgetId > 0)
                {
                    status = true;
                    message = "Change Budget Name successfully.";
                }
                else
                {
                    message = "Change Budget Name unsuccessfully.";
                }
            }
            catch (Exception)
            {
                message = "Change Budget Name unsuccessfully.";
            }
            // return result after update budget
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CloneBudget(int id, string budgetName)
        {
            bool status = false;
            string message = string.Empty;
            int budgetId = 0;
            try
            {
                // call action clone budget
                budgetId = SingletonIpl.GetInstance<BudgetBll>().CloneBudget(id, budgetName, CurrentUser.UserId);
                if (budgetId > 0)
                {
                    status = true;
                    message = "Clone Budget successfully.";
                }
                else
                {
                    message = "Clone Budget unsuccessfully.";
                }
            }
            catch (Exception)
            {
                message = "Clone Budget unsuccessfully.";
            }
            return Json(new { Status = status, Message = message, BudgetId = budgetId }, JsonRequestBehavior.AllowGet);
        }

        #region Category Setting by Budget

        /// <summary>
        /// Method: view screen category setting by budget
        /// </summary>
        /// <returns></returns>
        public ActionResult CategorySetting(int? id)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null || budget.BudgetId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get flag show/hidden help restaurant site
            var currentHelpSetting = SingletonIpl.GetInstance<HelpSettingBll>().GetByUserIdAndHelpSettingDataId(CurrentUser.UserId, BCSCommonData.HELP_SETTING_CATEGORY_SETTING);
            ViewBag.ShowOrHiddenHelpSetting = currentHelpSetting != null ? currentHelpSetting.IsHidden.Value : false;

            // get all setting by budget id
            var result = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(id.Value, CurrentUser.UserId).ToList();
            if (result == null || result.Count == 0)
            {
                // get default categories setting by user
                var defaultCategoryList = SingletonIpl.GetInstance<CategoryBll>().GetCategoryByUserId(CurrentUser.UserId).ToList();

                // add new categories setting of budget by user
                List<int> categorySettingByBudget = new List<int>();

                // sales category mapping
                IDictionary<string, int> salesCategoryMapping = new Dictionary<string, int>();

                // the first: get parent categories and insert to db
                var parentCategory = defaultCategoryList.Where(p => p.ParentCategoryId == 0);
                foreach (var parent in parentCategory)
                {
                    int oldParentCategoryId = parent.CategoryId;

                    Mapper.CreateMap<Category, CategorySetting>();
                    var parentCategorySetting = Mapper.Map<Category, CategorySetting>(parent);

                    // add budget id to category setting
                    parentCategorySetting.BudgetId = id;

                    // save new category setting by budget
                    var parentCategorySettingId = SingletonIpl.GetInstance<CategorySettingBll>().Save(parentCategorySetting, CurrentUser.UserId);
                    categorySettingByBudget.Add(parentCategorySettingId);

                    // the seconrd: get childen categories by parent category id
                    var childenCategory = defaultCategoryList.Where(p => p.ParentCategoryId == oldParentCategoryId);
                    foreach (var childen in childenCategory)
                    {
                        Mapper.CreateMap<Category, CategorySetting>();
                        var childenCategorySetting = Mapper.Map<Category, CategorySetting>(childen);

                        // add new parent category setting id to category setting
                        childenCategorySetting.ParentCategoryId = parentCategorySettingId;

                        // add budget id to category setting
                        childenCategorySetting.BudgetId = id;

                        // set sales category ref id to children category of cogs section
                        if (parent.CategoryName.Equals(BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT))
                        {
                            childenCategorySetting.SalesCategoryRefId = salesCategoryMapping[childen.CategoryName];
                        }

                        // save new category setting by budget
                        var childenCategorySettingId = SingletonIpl.GetInstance<CategorySettingBll>().Save(childenCategorySetting, CurrentUser.UserId);
                        categorySettingByBudget.Add(childenCategorySettingId);

                        // add sales category children to map
                        if (parent.CategoryName.Equals(BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT))
                        {
                            salesCategoryMapping.Add(childen.CategoryName, childenCategorySettingId);
                        }
                    }
                }

                if (defaultCategoryList.Count != categorySettingByBudget.Count)
                {
                    // throw exception
                }

                // get all category setting by budget id
                result = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(id.Value, CurrentUser.UserId).ToList();
            }

            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            ViewBag.CategorySettingList = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(result);

            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
             // get rest name rest code and token id by user id
            var restCodebyUserId = SingletonIpl.GetInstance<RestActiveCodeBll>().GetByUserIdAndRestCode(CurrentUser.UserId, budget.RestCode);
            if (restCodebyUserId != null)
            {
                ViewBag.RestCode = budget.RestCode;
                ViewBag.RestName = restCodebyUserId.RestName;
            }

            model.EditFlg = (budget.CreatedUserId == CurrentUser.UserId);
            return View(model);
        }

        public ActionResult ViewEditCategorySetting(int id)
        {
            // Get category setting by id
            var result = SingletonIpl.GetInstance<CategorySettingBll>().Get(id);
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            CategorySettingModel model = Mapper.Map<CategorySetting, CategorySettingModel>(result);

            // set edit flag
            ViewBag.EditFlag = (model.CreatedUserId == CurrentUser.UserId);

            return PartialView("_ViewEditCategorySetting", model);
        }

        public ActionResult GetCategorySettingByParentId(int id, int budgetId, bool isTaxFlag)
        {
            // Get childen categories by parent category id
            var result = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByParentId(id, budgetId, CurrentUser.UserId).Where(s => s.IsTaxCost == isTaxFlag).OrderBy(s=>s.SortOrder).ToList();
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            var data = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(result);

            return Json(new DataSourceResult { Data = data, Total = data.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCategorySetting(List<CategorySettingModel> listObj, int budgetId, string parentCategoryName)
        {
            var categorySettingBll = SingletonIpl.GetInstance<CategorySettingBll>();
            var budgetItemBll = SingletonIpl.GetInstance<BudgetItemBll>();

            // init local variable
            int currentUserId = CurrentUser.UserId;
            DateTime dateTimeNow = DateTime.Now;

            // check parent category name is "Sales/COGS", call update two section
            int salesParentCategoryId = 0, cogsParentCategoryId = 0;
            IList<CategorySetting> salesCategoryList = null, cogsCategoryList = null;
            if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName || BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT == parentCategoryName)
            {
                // get all category on parent category is "Sales"
                salesCategoryList = categorySettingBll.GetCategorySettingByParentName(BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT, budgetId, currentUserId);
                salesParentCategoryId = salesCategoryList.FirstOrDefault(p => p.ParentCategoryId.Value == 0).CategorySettingId;

                // get all category on parent category is "COGS"
                cogsCategoryList = categorySettingBll.GetCategorySettingByParentName(BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT, budgetId, currentUserId);
                cogsParentCategoryId = cogsCategoryList.FirstOrDefault(p => p.ParentCategoryId.Value == 0).CategorySettingId;
            }

            List<int> resultList = new List<int>();

            // convert model to entity
            Mapper.CreateMap<CategorySettingModel, CategorySetting>();
            var categorySettingList = Mapper.Map<List<CategorySettingModel>, List<CategorySetting>>(listObj);

            #region Add New Category From Input List
            // add new category from input list
            var addCategorySettingList = categorySettingList.Where(s => s.CategorySettingId == 0);
            foreach (var item in addCategorySettingList)
            {
                // check add sales category reference before
                var salesCategorySettingId = 0;
                bool isPrimeCost = item.IsPrimeCost.HasValue ? item.IsPrimeCost.Value : false;
                if (BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT == parentCategoryName)
                {
                    // add new category
                    CategorySetting salesCategoryReference = new CategorySetting();
                    Mapper.CreateMap<CategorySetting, CategorySetting>();
                    Mapper.Map<CategorySetting, CategorySetting>(item, salesCategoryReference);
                    salesCategoryReference.ParentCategoryId = salesParentCategoryId;

                    // set default is percentage
                    salesCategoryReference.IsPercentage = true;

                    // call method save category reference
                    salesCategorySettingId = categorySettingBll.Save(salesCategoryReference, currentUserId);

                    // set default is prime cost to COGS category
                    isPrimeCost = true;
                }

                // set sales category reference id
                item.SalesCategoryRefId = salesCategorySettingId;

                // set is prime cost flag
                item.IsPrimeCost = isPrimeCost;

                // call action add new category
                item.UpdatedUserId = currentUserId;
                item.UpdatedDate = dateTimeNow;
                var categorySettingIdNew = categorySettingBll.Save(item, currentUserId);
                if (categorySettingIdNew > 0)
                {
                    resultList.Add(categorySettingIdNew);
                }

                // check add cogs category reference after
                if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName)
                {
                    // add new category
                    CategorySetting cogsCategoryReference = new CategorySetting();
                    Mapper.CreateMap<CategorySetting, CategorySetting>();
                    Mapper.Map<CategorySetting, CategorySetting>(item, cogsCategoryReference);
                    cogsCategoryReference.ParentCategoryId = cogsParentCategoryId;
                    cogsCategoryReference.SalesCategoryRefId = categorySettingIdNew;

                    // set default is percentage
                    cogsCategoryReference.IsPercentage = true;

                    // set default is prime cost to COGS category
                    cogsCategoryReference.IsPrimeCost = true;

                    // call method save category reference
                    categorySettingBll.Save(cogsCategoryReference, currentUserId);
                }
            }
            #endregion

            #region Delete all category from input list
            // delete all category from input list
            List<int> updatedRefCategoryIdList = new List<int>();
            var deleteCategorySettingList = categorySettingList.Where(s => s.DeletedFlg == true);
            foreach (var item in deleteCategorySettingList)
            {
                // call action delete category
                item.UpdatedUserId = currentUserId;
                item.UpdatedDate = dateTimeNow;
                item.DeletedFlg = true;

                // call action delete current category
                var categorySettingIdUpdated = categorySettingBll.Save(item, currentUserId);
                if (categorySettingIdUpdated > 0)
                {
                    resultList.Add(categorySettingIdUpdated);
                }

                // call delete budget item by category setting
                budgetItemBll.DeleteByCategorySettingId(item.CategorySettingId, currentUserId);

                // delete category reference by name
                if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName)
                {
                    updatedRefCategoryIdList.Add(categorySettingIdUpdated);

                    // get category name old
                    var salesCategoryNameOld = salesCategoryList.FirstOrDefault(s => s.CategorySettingId == item.CategorySettingId).CategoryName;

                    // update category
                    var cogsCategoryReference = cogsCategoryList.FirstOrDefault(p => p.CategoryName == salesCategoryNameOld);
                    if (cogsCategoryReference != null)
                    {
                        // set update item
                        cogsCategoryReference.CategoryName = item.CategoryName;
                        cogsCategoryReference.SortOrder = item.SortOrder;
                        cogsCategoryReference.DeletedFlg = true;

                        // set common update item
                        cogsCategoryReference.UpdatedDate = dateTimeNow;
                        cogsCategoryReference.UpdatedUserId = currentUserId;

                        // call action delete category reference
                        categorySettingBll.Save(cogsCategoryReference, currentUserId);

                        // call delete budget item by category setting
                        budgetItemBll.DeleteByCategorySettingId(cogsCategoryReference.CategorySettingId, currentUserId);
                    }

                    // get cogs category reference to sales category deleted
                    var categoryRefList = cogsCategoryList.Where(p => p.SalesCategoryRefId == item.CategorySettingId);
                    if (categoryRefList != null)
                    {
                        // reset to default Total Sales all category reference on cogs category
                        foreach (var categoryRefItem in categoryRefList)
                        {
                            // change reference id to sales total
                            categoryRefItem.SalesCategoryRefId = 0;
                            categoryRefItem.UpdatedUserId = currentUserId;
                            categoryRefItem.UpdatedDate = dateTimeNow;

                            // call method save category reference
                            categorySettingBll.Save(categoryRefItem, currentUserId);
                        }
                    }
                }
                // update ref id to category reference
                else if (BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT == parentCategoryName)
                {
                    // get category name old
                    var cogsCategoryNameOld = cogsCategoryList.FirstOrDefault(s => s.CategorySettingId == item.CategorySettingId).CategoryName;

                    // get cogs category reference name from sales category
                    var salesCategoryRefName = salesCategoryList.FirstOrDefault(s => s.CategoryName == cogsCategoryNameOld);
                    if (salesCategoryRefName != null)
                    {
                        // set update item
                        salesCategoryRefName.CategoryName = item.CategoryName;
                        salesCategoryRefName.SortOrder = item.SortOrder;
                        salesCategoryRefName.DeletedFlg = true;

                        // set deleted flag is true
                        salesCategoryRefName.UpdatedDate = dateTimeNow;
                        salesCategoryRefName.UpdatedUserId = currentUserId;

                        // call action delete cogs category reference
                        categorySettingBll.Save(salesCategoryRefName, currentUserId);
                        updatedRefCategoryIdList.Add(salesCategoryRefName.CategorySettingId);

                        // call delete budget item by category setting
                        budgetItemBll.DeleteByCategorySettingId(salesCategoryRefName.CategorySettingId, currentUserId);
                    }

                    // get cogs category reference to sales category deleted
                    var cogsCategoryRefList = cogsCategoryList.Where(p => p.SalesCategoryRefId == salesCategoryRefName.CategorySettingId);
                    if (cogsCategoryRefList != null)
                    {
                        // reset to default Total Sales all category reference on cogs category
                        foreach (var categoryRefItem in cogsCategoryRefList)
                        {
                            if (!deleteCategorySettingList.Any(s => s.CategorySettingId == categoryRefItem.CategorySettingId))
                            {
                                // change reference id to sales total
                                categoryRefItem.SalesCategoryRefId = 0;
                                categoryRefItem.UpdatedUserId = currentUserId;
                                categoryRefItem.UpdatedDate = dateTimeNow;

                                // call method save category reference
                                categorySettingBll.Save(categoryRefItem, currentUserId);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Update all category from input list
            // update all category from input list
            var updateCategorySettingList = categorySettingList.Where(s => s.CategorySettingId != 0 && s.DeletedFlg == false);
            foreach (var item in updateCategorySettingList)
            {
                // set common item update
                item.UpdatedUserId = currentUserId;
                item.UpdatedDate = dateTimeNow;

                // reset id refrence if exists deleted list
                if (item.SalesCategoryRefId > 0 && updatedRefCategoryIdList.Any(categoryRefId => categoryRefId == item.SalesCategoryRefId))
                {
                    item.SalesCategoryRefId = 0;
                }

                // call action update category
                var categorySettingIdUpdated = categorySettingBll.Save(item, currentUserId);
                if (categorySettingIdUpdated > 0)
                {
                    resultList.Add(categorySettingIdUpdated);
                }

                // check update category reference
                if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName || BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT == parentCategoryName)
                {
                    // get category name old
                    var categoryNameOld = (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName ? salesCategoryList : cogsCategoryList).FirstOrDefault(s => s.CategorySettingId == item.CategorySettingId).CategoryName;

                    // update category
                    var categoryReference = (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName ? cogsCategoryList : salesCategoryList).FirstOrDefault(s => s.CategoryName == categoryNameOld);
                    if (categoryReference != null)
                    {
                        // set edit item from screen
                        categoryReference.CategoryName = item.CategoryName;
                        categoryReference.SortOrder = item.SortOrder;
                        categoryReference.DeletedFlg = item.DeletedFlg;

                        // reset id refrence if exists deleted list
                        if (categoryReference.SalesCategoryRefId > 0 && updatedRefCategoryIdList.Any(categoryRefId => categoryRefId == categoryReference.SalesCategoryRefId))
                        {
                            categoryReference.SalesCategoryRefId = 0;
                        }

                        // set common item update
                        categoryReference.UpdatedDate = dateTimeNow;
                        categoryReference.UpdatedUserId = currentUserId;

                        // call method save category reference
                        categorySettingBll.Save(categoryReference, currentUserId);
                    }
                }

            }
            #endregion

            return Json(new { Status = (resultList.Count == categorySettingList.Count) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckDuplicateCategorySetting(CategorySettingModel categorySetting)
        {
            Mapper.CreateMap<CategorySettingModel, CategorySetting>();
            var obj = Mapper.Map<CategorySettingModel, CategorySetting>(categorySetting);

            var result = SingletonIpl.GetInstance<CategorySettingBll>().CheckDuplicateCategorySetting(obj, CurrentUser.UserId);

            return Json(new { Status = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RestoreCategories(int budgetId)
        {
            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(budgetId);
            if (budget == null || budget.BudgetId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // check permission
            if (budget.CreatedUserId != CurrentUser.UserId)
            {
                return Json(new { Status = false, Message = "Cannot restore Default Categories Settings." }, JsonRequestBehavior.AllowGet);
            }

            // call method delete all category setting by budget id
            var result = SingletonIpl.GetInstance<CategorySettingBll>().DeleteByBudgetId(budgetId, CurrentUser.UserId);
            return Json(new { Status = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewTemplateCategory()
        {
            // get all template file name by folder path
            var pathFolder = Server.MapPath("~/App_Data/TemplateCategory/");
            DirectoryInfo dic = new DirectoryInfo(pathFolder);
            FileInfo[] Files = dic.GetFiles("*.*");

            var fileNameList = new List<SelectListItem>();
            foreach (FileInfo file in Files)
            {
                fileNameList.Add(new SelectListItem()
                {
                    Text = file.Name.Replace('_', ' ').Replace(file.Extension, ""),
                    Value = file.Name
                });
            }
            ViewBag.FileNameList = fileNameList;

            return PartialView("_ViewTemplateCategory");
        }

        /// <summary>
        /// Method: get data table in file by name
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult GetDataFromTemplate([DataSourceRequest] DataSourceRequest request, string fileName)
        {
            // get all template file name by folder path
            var filePath = Path.Combine(Server.MapPath("~/App_Data/TemplateCategory"), JsonConvert.DeserializeObject<string>(fileName));

            // get data table from file path
            System.Data.DataTable dataTable = this.GetDataTableInFile(filePath);

            // return result data table in file
            return Json(dataTable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: load category by template to budget
        /// </summary>
        /// <param name="sectionList"></param>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public ActionResult SaveCategorySettingByTemplate(List<SectionModel> sectionList, int budgetId)
        {
            bool status = false;
            string message = string.Empty;

            try
            {
                // create instance category setting bll
                var categorySettingBll = SingletonIpl.GetInstance<CategorySettingBll>();

                // 1. Loop all section
                foreach (var section in sectionList)
                {
                    bool isTaxCost = section.SectionName.Contains("(Tax)");

                    // 1.1. Get parent category id by section name and user id
                    string parentCategoryName = isTaxCost ? section.SectionName.Replace(" (Tax)", "") : section.SectionName;
                    var parentCategory = categorySettingBll.GetCategorySettingByParentName(parentCategoryName, budgetId, CurrentUser.UserId).FirstOrDefault(s => s.ParentCategoryId == 0);

                    // 1.2. Delete all children category by parent category id
                    if (!isTaxCost)
                    {
                        categorySettingBll.DeleteByParentCategoryId(budgetId, parentCategory.CategorySettingId, CurrentUser.UserId);
                    }

                    // 1.3. Loop all category from template
                    if (section.CategoryBySection != null)
                    {
                        foreach (var category in section.CategoryBySection)
                        {
                            CategorySetting obj = new CategorySetting()
                            {
                                CategoryName = category.CategoryName,
                                SortOrder = category.SortOrder,
                                ParentCategoryId = parentCategory.CategorySettingId,
                                BudgetId = budgetId,
                                IsSelected = false,
                                IsPrimeCost = false,
                                IsPercentage = true,
                                IsTaxCost = isTaxCost,
                                DeletedFlg = false,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                CreatedUserId = CurrentUser.UserId,
                                UpdatedUserId = CurrentUser.UserId
                            };

                            // call save to table in db
                            categorySettingBll.Save(obj, CurrentUser.UserId);
                        }
                    }
                }

                // 2. Set flag and message update successfully.
                status = true;
                message = "Load category by template successfully.";
            }
            catch
            {
                message = "System Exception: Please contact system administrator.";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// function reader sheet in file and return data table
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>dataTable</returns>
        private DataTable GetDataTableInFile(string path)
        {
            try {
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    var hasHeader = true;
                    pck.Load(new System.IO.FileInfo(path).OpenRead());
                    var ws = pck.Workbook.Worksheets.First();
                    DataTable tbl = new DataTable();
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }

                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    return tbl;
                }
            }
            catch {
                return new DataTable();
            }
        }

        #endregion

        #region Link To You Restaurant by Budget

        /// <summary>
        /// Method : View screen link to your restaurant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewLinkToYourRestaurant(int? id)
        {
            // check parameter input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null || budget.BudgetId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get flag show/hidden help restaurant site
            var currentHelpSetting = SingletonIpl.GetInstance<HelpSettingBll>().GetByUserIdAndHelpSettingDataId(CurrentUser.UserId, BCSCommonData.HELP_SETTING_LINK_TO_YOUR_RESTAURANT);
            ViewBag.ShowOrHiddenHelpSetting = currentHelpSetting != null ? currentHelpSetting.IsHidden.Value : false;

            // get rest code by current user
            var result = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).ToList();
            Mapper.CreateMap<RestActiveCode, RestActiveCodeModel>();
            var restList = Mapper.Map<List<RestActiveCode>, List<RestActiveCodeModel>>(result);
            ViewBag.RestaurantList = restList;

            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            model.EditFlg = (budget.CreatedUserId == CurrentUser.UserId);
            return View(model);
        }

        /// <summary>
        /// Method update link rest code to budget
        /// </summary>
        /// <param name="restCode"></param>
        /// <returns></returns>
        public JsonResult UpdateLinkRestCode(string restCode, int budgetId)
        {
            bool status = false;
            string message = string.Empty;

            // check rest code is not exists by user
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(budgetId);
            if (budget == null)
            {
                message = "This Budget is not eixsts.";
            }
            else
            {
                // update link rest code
                budget.RestCode = restCode.Equals("NotDefault") ? null : restCode;
                budget.UpdatedDate = DateTime.Now;
                budget.UpdatedUserId = CurrentUser.UserId;
                var result = SingletonIpl.GetInstance<BudgetBll>().Save(budget, CurrentUser.UserId);

                if (result == budget.BudgetId)
                {
                    status = true;
                    message = "Successful.";
                }
                else
                {
                    message = "Unsuccessful.";
                }
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Budget Length by Budget

        /// <summary>
        /// Method: open view screen budget length by budget
        /// </summary>
        /// <returns></returns>
        public ActionResult BudgetLength(int? id)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get flag show/hidden help restaurant site
            var currentHelpSetting = SingletonIpl.GetInstance<HelpSettingBll>().GetByUserIdAndHelpSettingDataId(CurrentUser.UserId, BCSCommonData.HELP_SETTING_BUDGET_LENGTH);
            ViewBag.ShowOrHiddenSetBudLength = currentHelpSetting != null ? currentHelpSetting.IsHidden.Value : false;

            // get common_Data by datacode
            var BudgetType = SingletonIpl.GetInstance<CommonBll>().GetCommonDataByCode(BCSCommonData.BUDGET_TYPE_CODE);
            ViewBag.BudgetType = BudgetType.ToList();

            string budgetLengthCode = budget.BudgetLengthType == BCSCommonData.BUDGET_TYPE_MONTH ? BCSCommonData.BUDGET_MONTH_LENGTH_CODE : BCSCommonData.BUDGET_PRERIODIC_LENGTH_CODE;
            var BudgetMonthLenth = SingletonIpl.GetInstance<CommonBll>().GetCommonDataByCode(budgetLengthCode);
            ViewBag.BudgetMonthLenth = BudgetMonthLenth.ToList();

            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            model.EditFlg = (budget.CreatedUserId == CurrentUser.UserId);
            return View(model);
        }

        /// <summary>
        /// Method change budget type
        /// </summary>
        /// <param name="budgetType"></param>
        /// <returns></returns>
        public ActionResult ChangeBudgetType(int budgetId, int budgetType, bool confirmDelete)
        {
            bool status = false;
            string message = string.Empty;

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(budgetId);
            if (budget == null)
            {
                message = "This Budget is not exists.";
                return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
            }

            // check budget tab existed in db
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
            if (budgetTabList != null && budgetTabList.Count > 0)
            {
                if (confirmDelete)
                {
                    // call method delete old data
                    var deleteFlg = SingletonIpl.GetInstance<BudgetTabBll>().DeleteBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
                }
                else
                {
                    message = "Are you sure you want to change budget type?<br/>Budget numbers in current budget will be lost.";
                    return Json(new { Status = status, Confirm = true, Message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            // call method update
            budget.BudgetLengthType = budgetType;
            budget.BudgetLengthStart = new DateTime(DateTime.Now.Year, 1, 1);
            budget.FiscalYearStartOn = new DateTime(DateTime.Now.Year, 1, 1);
            budget.BudgetLength = (budgetType == BCSCommonData.BUDGET_TYPE_MONTH) ? BCSCommonData.BUDGET_MONTH_LENGTH_ONE_YEAR : BCSCommonData.BUDGET_PRERIODIC_LENGTH_ONE_YEAR;
            budget.UpdatedDate = DateTime.Now;
            budget.UpdatedUserId = CurrentUser.UserId;
            var result = SingletonIpl.GetInstance<BudgetBll>().Save(budget, CurrentUser.UserId);
            if (result == budgetId)
            {
                status = true;
                message = "Successful.";
            }
            else
            {
                message = "Unsuccessful.";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // Save Budget Length
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="budgetType"></param>
        /// <param name="startDate"></param>
        /// <param name="fiscalYearStartOn"></param>
        /// <param name="budgetMonthLenth"></param>
        /// <returns></returns>
        public ActionResult SaveChangeBudget(int budgetId, int budgetType, DateTime startDate, DateTime? fiscalYearStartOn, int budgetMonthLenth, bool confirmDelete)
        {
            bool status = false;
            string message = string.Empty;

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(budgetId);
            if (budget == null)
            {
                message = "This Budget is not exists.";
                return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
            }

            // check budget tab existed in db
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
            if (budgetTabList != null && budgetTabList.Count > 0)
            {
                if (confirmDelete)
                {
                    // call method delete old data
                    var deleteFlg = SingletonIpl.GetInstance<BudgetTabBll>().DeleteBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
                }
                else
                {
                    message = "Are you sure you want to change budget type?<br/>Budget numbers in current budget will be lost.";
                    return Json(new { Status = status, Confirm = true, Message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            // clear budget data
            budget.BudgetLengthType = budgetType;
            budget.BudgetLengthStart = startDate;
            budget.FiscalYearStartOn = fiscalYearStartOn;
            budget.BudgetLength = budgetMonthLenth;

            // clear data profit loss
            budget.Sales = 0;
            budget.SalesPercent = 0;
            budget.COGS = 0;
            budget.COGSPercent = 0;
            budget.GrossProfit = 0;
            budget.GrossProfitPercent = 0;
            budget.PayrollExpenses = 0;
            budget.PayrollExpensesPercent = 0;
            budget.OperatingProfit = 0;
            budget.OperatingProfitPercent = 0;
            budget.PrimeCost = 0;
            budget.PrimeCostPercent = 0;
            budget.OperatingExpenses = 0;
            budget.OperatingExpensesPercent = 0;
            budget.NetProfitLoss = 0;
            budget.NetProfitLossPercent = 0;
            budget.BreakEvenPoint = 0;
            budget.BreakEvenPointPercent = 0;

            // set common item update
            budget.UpdatedDate = DateTime.Now;
            budget.UpdatedUserId = CurrentUser.UserId;

            var result = SingletonIpl.GetInstance<BudgetBll>().Save(budget, CurrentUser.UserId);
            if (result == budgetId)
            {
                status = true;
                message = "Successful.";
            }
            else
            {
                message = "Unsuccessful.";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Input Method by Budget

        /// <summary>
        /// Method: open view screen Input Method by budget
        /// </summary>
        /// <returns></returns>
        public ActionResult InputMethod(int? id)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get flag show/hidden help input method screen
            var currentHelpSetting = SingletonIpl.GetInstance<HelpSettingBll>().GetByUserIdAndHelpSettingDataId(CurrentUser.UserId, BCSCommonData.HELP_SETTING_INPUT_METHOD);
            ViewBag.ShowOrHiddenHelpSettingInputMethod = currentHelpSetting != null ? currentHelpSetting.IsHidden.Value : false;

            // get common_Data by datacode
            var InputMethod = SingletonIpl.GetInstance<CommonBll>().GetCommonDataByCode(BCSCommonData.INPUT_METHOD_CODE);
            ViewBag.InputMethod = InputMethod.ToList();

            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            model.EditFlg = (budget.CreatedUserId == CurrentUser.UserId);
            return View(model);
        }

        /// <summary>
        /// // Save Input Method
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="budgetType"></param>
        /// <param name="startDate"></param>
        /// <param name="fiscalYearStartOn"></param>
        /// <param name="budgetMonthLenth"></param>
        /// <returns></returns>
        public JsonResult SaveInputMethodBudget(int budgetId, int inputMethod, bool actualNumbersFlg, bool varianceFlg)
        {
            bool status = false;
            string message = string.Empty;

            var budgets = SingletonIpl.GetInstance<BudgetBll>().Get(budgetId);
            if (budgets == null)
            {
                message = "This Budget is not exists.";
            }
            else
            {
                budgets.InputMethodId = inputMethod;
                budgets.ActualNumbersFlg = actualNumbersFlg;
                budgets.VarianceFlg = varianceFlg;
                budgets.UpdatedDate = DateTime.Now;
                budgets.UpdatedUserId = CurrentUser.UserId;

                // call method update budget
                var result = SingletonIpl.GetInstance<BudgetBll>().Save(budgets, CurrentUser.UserId);
                if (result == budgetId)
                {
                    status = true;
                    message = "Successfully!";
                }
                else
                {
                    message = "Unsuccessful!";
                }
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region View Good Job category setting by Budget

        /// <summary>
        /// Method: open view screen Good Job by budget
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewGoodJobCategorySetting(int? id)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            return View(model);
        }
        #endregion

        #region View Taget & Sales by Budget

        /// <summary>
        /// Method view target & sales page by budget id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewTagetAndSales(int? id, string section, string budgetTabIndex)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get flag show/hidden help restaurant site
            var currentHelpSetting = SingletonIpl.GetInstance<HelpSettingBll>().GetByUserIdAndHelpSettingDataId(CurrentUser.UserId, BCSCommonData.HELP_SETTING_TARGET_SALES);
            ViewBag.ShowOrHiddenHelpSetting = currentHelpSetting != null ? currentHelpSetting.IsHidden.Value : false;

            // get common_Data by datacode
            var InputMethod = SingletonIpl.GetInstance<CommonBll>().GetCommonDataByCode(BCSCommonData.INPUT_METHOD_CODE);
            ViewBag.InputMethod = InputMethod.ToList();

            // set budget tab model to model
            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);

            // set section view name
            model.Section = string.IsNullOrEmpty(section) ? BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT : section;
            model.BudgetTabIndex = string.IsNullOrEmpty(budgetTabIndex) ? "1" : budgetTabIndex;

            // get input from session exists
            if (Session["BUDGET_InputMethodId_" + budget.BudgetId] != null)
            {
                model.InputMethodId = Convert.ToInt32(Session["BUDGET_InputMethodId_" + budget.BudgetId]);
                Session.Remove("BUDGET_InputMethodId_" + budget.BudgetId);

                model.ActualNumbersFlg = Convert.ToBoolean(Session["BUDGET_ActualNumbersFlg_" + budget.BudgetId]);
                Session.Remove("BUDGET_ActualNumbersFlg_" + budget.BudgetId);

                model.VarianceFlg = Convert.ToBoolean(Session["BUDGET_VarianceFlg_" + budget.BudgetId]);
                Session.Remove("BUDGET_VarianceFlg_" + budget.BudgetId);
            }

            // get category setting by budget id
            var categorySettingList = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budget.BudgetId, CurrentUser.UserId);
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            model.CategorySettingModelList = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(categorySettingList.ToList());

            // get all budget tab by budget id
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);

            // check eixts budget tab.
            if (budgetTabList == null || budgetTabList.Count == 0)
            {
                // budget tab is not exists call method add new default to database
                this.AddDefaultValueToBudget(budget);

                // get all budget tab by budget id
                budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
            }

            // get data from session
            var budgetTabListSession = Session["BUDGET_" + budget.BudgetId] as List<BudgetTabModel>;
            Session.Remove("BUDGET_" + budget.BudgetId);

            Mapper.CreateMap<BudgetTab, BudgetTabModel>();
            var budgetTabModelList = Mapper.Map<List<BudgetTab>, List<BudgetTabModel>>(budgetTabList.ToList());

            // get budget item row by budget tab
            foreach (var budgetTabModel in budgetTabModelList)
            {
                // add default value to budget item
                StringBuilder budgetItemRow = new StringBuilder();
                for (int i = 0; i < budgetTabModel.HeaderColumnList.Count; i++)
                {
                    budgetItemRow.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, 0, 0, 0, 0, 0);
                }

                var dataItemByBudgetTab = SingletonIpl.GetInstance<BudgetItemBll>().GetBudgetItemByBudgetTabId(budgetTabModel.BudgetId, budgetTabModel.BudgetTabId, budgetItemRow.ToString(), CurrentUser.UserId);
                Mapper.CreateMap<BudgetItem, BudgetItemModel>();
                budgetTabModel.BudgetItemModelList = Mapper.Map<List<BudgetItem>, List<BudgetItemModel>>(dataItemByBudgetTab.ToList());

                if (budgetTabListSession != null)
                {
                    // get tab by tab id from session
                    var tabInSession = budgetTabListSession.FirstOrDefault(s => s.BudgetTabId == budgetTabModel.BudgetTabId);

                    // set data annualsales
                    budgetTabModel.AnnualSales = tabInSession.AnnualSales;

                    // set data target from session
                    budgetTabModel.TargetColumn = tabInSession.TargetColumn;

                    // add old data budget item model list to new model load from db
                    foreach (BudgetItemModel item in budgetTabModel.BudgetItemModelList)
                    {
                        // compare data by category setting id, load data from session
                        var budgetItemListInSession = tabInSession.BudgetItemModelList.FirstOrDefault(s => s.CategorySettingId == item.CategorySettingId);
                        if (budgetItemListInSession != null)
                        {
                            item.BudgetItemRow = budgetItemListInSession.BudgetItemRow;

                            // update sales reference category id
                            if (budgetItemListInSession.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT)
                            {
                                var categorySettingRef = categorySettingList.FirstOrDefault(s => s.CategorySettingId == budgetItemListInSession.CategorySettingId);
                                budgetItemListInSession.SalesCategoryRefId = categorySettingRef.SalesCategoryRefId;
                            }
                        }
                    }
                }
            }

            // set budget tab list to model
            model.BudgetTabModelList = budgetTabModelList;

            // set role edit flag
            model.EditFlg = (CurrentUser.UserId == model.CreatedUserId);

            return View(model);
        }

        /// <summary>
        /// Method change status prime cost is category setting id
        /// </summary>
        /// <param name="categorySettingId"></param>
        /// <returns></returns>
        public ActionResult ChangeIsPrimeCost(int categorySettingId)
        {
            bool status = true;
            string message = "Save change successfully!";

            try
            {
                // get category setting by category setting id
                var categorySetting = SingletonIpl.GetInstance<CategorySettingBll>().Get(categorySettingId);

                // change status prime cost
                categorySetting.IsPrimeCost = !categorySetting.IsPrimeCost;

                // call method save category setting
                SingletonIpl.GetInstance<CategorySettingBll>().Save(categorySetting, CurrentUser.UserId);
            }
            catch
            {
                status = false;
                message = "Save change unsuccessful!";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: save copy item budget to session
        /// </summary>
        /// <param name="budgetTabModelList"></param>
        /// <param name="budgetId"></param>
        /// <param name="inputMethod"></param>
        /// <param name="actualNumbersFlg"></param>
        /// <param name="varianceFlg"></param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>
        public ActionResult SaveBudgetDetailToSession(List<BudgetTabModel> budgetTabModelList, int budgetId, int inputMethod, bool actualNumbersFlg, bool varianceFlg)
        {
            bool status = true;
            string message = "change successfully!";

            try
            {
                // save data budget model list to session
                Session["BUDGET_" + budgetId] = budgetTabModelList;
                Session["BUDGET_InputMethodId_" + budgetId] = inputMethod;
                Session["BUDGET_ActualNumbersFlg_" + budgetId] = actualNumbersFlg;
                Session["BUDGET_VarianceFlg_" + budgetId] = varianceFlg;
            }
            catch
            {
                status = false;
                message = "change unsuccessful!";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get all budget tab by budget id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetTabList(int id)
        {
            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get all budget tab by budget id
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);

            // check eixts budget tab.
            if (budgetTabList == null || budgetTabList.Count == 0)
            {
                // budget tab is not exists call method add new default to database
                this.AddDefaultValueToBudget(budget);

                // get all budget tab by budget id
                budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
            }

            // return budget tab list
            return Json(budgetTabList.Select(s => new { BudgetTabId = s.BudgetTabId, TabName = s.TabName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeSalesCategoryRefId(int id, int cogsCategoryId, int salesCategoryId)
        {
            // init status and message result
            string message = string.Empty;
            bool status = false;

            try
            {
                // 1. Get COGS category by budget id and category id
                var categorySettingBll = SingletonIpl.GetInstance<CategorySettingBll>();
                var updateCategorySetting = categorySettingBll.Get(cogsCategoryId);

                // 2. Change sales category ref id
                updateCategorySetting.SalesCategoryRefId = salesCategoryId;

                // 3. Call action save change object
                var updateCategorySettingId = categorySettingBll.Save(updateCategorySetting, CurrentUser.UserId);
                if (updateCategorySettingId > 0)
                {
                    // set status after success
                    status = true;
                    message = "Change reference data sales to cogs!";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // return result
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method get budget tab name by budget
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        private IDictionary<string, List<string>> GetBudgetTabNameAndHeaderByBudget(Budget budget)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            int count = 0;
            if (budget.BudgetLengthType.Value == BCSCommonData.BUDGET_TYPE_MONTH)
            {
                switch (budget.BudgetLength.Value)
                {
                    case BCSCommonData.BUDGET_MONTH_LENGTH_ONE_MONTH:
                        count = 1;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_THREE_MONTH:
                        count = 3;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_SIX_MONTH:
                        count = 6;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_NINE_MONTH:
                        count = 9;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_ONE_YEAR:
                        count = 12;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_TWO_YEAR:
                        count = 24;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_THREE_YEAR:
                        count = 36;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_FOUR_YEAR:
                        count = 48;
                        break;
                    case BCSCommonData.BUDGET_MONTH_LENGTH_FIVE_YEAR:
                        count = 60;
                        break;
                    default:
                        break;
                }

                int fiscalYearStartOnMonth = budget.FiscalYearStartOn.Value.Month;
                for (int i = 0; i < count; i++)
                {
                    var item = budget.BudgetLengthStart.Value.AddMonths(i);
                    string tabName = string.Empty;
                    List<string> header = new List<string>();

                    if (item.Month < fiscalYearStartOnMonth)
                    {
                        // the first year old sales
                        tabName = (fiscalYearStartOnMonth == 1) ? (item.Year - 1).ToString() : string.Format("{0} - {1}", (item.Year - 1), item.Year);
                    }
                    else
                    {
                        // the this year
                        tabName = (fiscalYearStartOnMonth == 1) ? item.Year.ToString() : string.Format("{0} - {1}", item.Year, item.Year + 1);
                    }

                    if (result.ContainsKey(tabName))
                    {
                        header = result[tabName];
                    }

                    header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, item.ToString("MMMM")));
                    result[tabName] = header;
                }
            }
            else
            {
                string tabName = BUDGET_TAB_NAME_PERIODIC + budget.BudgetLengthStart.Value.Year;
                List<string> header = new List<string>();

                if (budget.BudgetLength.Value <= 213)
                {
                    for (int i = 0; i < (budget.BudgetLength.Value - 200); i++)
                    {
                        header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                    }
                    result[tabName] = header;
                }
                else if (budget.BudgetLength.Value > BCSCommonData.BUDGET_PRERIODIC_LENGTH_FIVE_YEAR)
                {
                    for (int i = 0; i < 52; i++)
                    {
                        header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                    }
                    result[tabName] = header;
                }
                else
                {
                    // add tab one year
                    for (int i = 0; i < 13; i++)
                    {
                        header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                    }
                    result[tabName] = header;

                    if (budget.BudgetLength.Value > BCSCommonData.BUDGET_PRERIODIC_LENGTH_ONE_YEAR)
                    {
                        // add tab two year
                        tabName = BUDGET_TAB_NAME_PERIODIC + (budget.BudgetLengthStart.Value.Year + 1);
                        header.Clear();
                        for (int i = 0; i < 13; i++)
                        {
                            header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                        }
                        result[tabName] = header;

                        if (budget.BudgetLength > BCSCommonData.BUDGET_PRERIODIC_LENGTH_TWO_YEAR)
                        {
                            // add tab three year
                            tabName = BUDGET_TAB_NAME_PERIODIC + (budget.BudgetLengthStart.Value.Year + 2);
                            header.Clear();
                            for (int i = 0; i < 13; i++)
                            {
                                header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                            }
                            result[tabName] = header;

                            if (budget.BudgetLength > BCSCommonData.BUDGET_PRERIODIC_LENGTH_THREE_YEAR)
                            {
                                // add tab four year
                                tabName = BUDGET_TAB_NAME_PERIODIC + (budget.BudgetLengthStart.Value.Year + 3);
                                header.Clear();
                                for (int i = 0; i < 13; i++)
                                {
                                    header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                                }
                                result[tabName] = header;

                                if (budget.BudgetLength > BCSCommonData.BUDGET_PRERIODIC_LENGTH_FOUR_YEAR)
                                {
                                    // add tab five year
                                    tabName = BUDGET_TAB_NAME_PERIODIC + (budget.BudgetLengthStart.Value.Year + 4);
                                    header.Clear();
                                    for (int i = 0; i < 13; i++)
                                    {
                                        header.Add(string.Format(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, BUDGET_HEADER_NAME_PERIODIC + (i + 1)));
                                    }
                                    result[tabName] = header;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// budget tab is not exists call method add new default to database
        /// </summary>
        /// <param name="budget"></param>
        private void AddDefaultValueToBudget(Budget budget)
        {
            // case budget tab list is null, call action insert budget tab
            IDictionary<string, List<string>> budgetTabNameList = this.GetBudgetTabNameAndHeaderByBudget(budget);
            int tabIndex = 0;
            foreach (var item in budgetTabNameList)
            {
                tabIndex++;
                StringBuilder headerColumn = new StringBuilder();
                StringBuilder targetColumn = new StringBuilder();
                foreach (string header in item.Value)
                {
                    headerColumn.Append(header);
                    targetColumn.AppendFormat(BCSCommonData.BUDGET_DETAIL_HEADER_TARGET_FORMAT, 0, 0);
                }

                BudgetTab budgetTab = new BudgetTab()
                {
                    BudgetId = budget.BudgetId,
                    TabIndex = tabIndex,
                    TabName = item.Key,
                    AnnualSales = 0,
                    HeaderColumn = headerColumn.ToString(),
                    TargetColumn = targetColumn.ToString(),
                    DeletedFlg = false,
                };

                var budgetTabId = SingletonIpl.GetInstance<BudgetTabBll>().Save(budgetTab, CurrentUser.UserId);

                // add default value to budget item
                StringBuilder budgetItemRow = new StringBuilder();
                for (int i = 0; i < item.Value.Count; i++)
                {
                    budgetItemRow.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, 0, 0, 0, 0, 0);
                }

                SingletonIpl.GetInstance<BudgetItemBll>().AddDefaultValueByBudgetTab(budget.BudgetId, budgetTabId, budgetItemRow.ToString(), CurrentUser.UserId);
            }
        }

        #endregion

        #region Review page

        /// <summary>
        /// Method: view page budget detail review.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReviewBudgetDetail(int? id)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get all budget tab by budget id
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);

            // check eixts budget tab.
            if (budgetTabList == null || budgetTabList.Count == 0)
            {
                // budget tab is not exists call method add new default to database
                this.AddDefaultValueToBudget(budget);

                // get all budget tab by budget id
                budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
            }

            Mapper.CreateMap<BudgetTab, BudgetTabModel>();
            var budgetTabModelList = Mapper.Map<List<BudgetTab>, List<BudgetTabModel>>(budgetTabList.ToList());

            // get budget item row by budget tab
            foreach (var budgetTabModel in budgetTabModelList)
            {
                // add default value to budget item
                StringBuilder budgetItemRow = new StringBuilder();
                for (int i = 0; i < budgetTabModel.HeaderColumnList.Count; i++)
                {
                    budgetItemRow.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, 0, 0, 0, 0, 0);
                }

                var dataItemByBudgetTab = SingletonIpl.GetInstance<BudgetItemBll>().GetBudgetItemByBudgetTabId(budgetTabModel.BudgetId, budgetTabModel.BudgetTabId, budgetItemRow.ToString(), CurrentUser.UserId);
                Mapper.CreateMap<BudgetItem, BudgetItemModel>();
                budgetTabModel.BudgetItemModelList = Mapper.Map<List<BudgetItem>, List<BudgetItemModel>>(dataItemByBudgetTab.ToList());
            }

            // set budget tab model to model
            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            model.BudgetTabModelList = budgetTabModelList;

            // get category setting by budget id
            var categorySettingList = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budget.BudgetId, CurrentUser.UserId);
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            model.CategorySettingModelList = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(categorySettingList.ToList());

            // set section view name
            model.Section = BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT;

            // set role edit flag
            model.EditFlg = (CurrentUser.UserId == model.CreatedUserId);

            return View(model);
        }

        #endregion

        #region common action save budget detail all section

        /// <summary>
        /// Method: save change data by section
        /// </summary>
        /// <param name="budgetTabModelList"></param>
        /// <returns></returns>
        public ActionResult SaveBudgetDetails(List<BudgetTabModel> budgetTabModelList, int budgetId, int inputMethod, bool actualNumbersFlg, bool varianceFlg)
        {
            bool status = true;
            string message = "Save change successfully!";

            try
            {
                // call method update budget item
                foreach (var budgetTab in budgetTabModelList)
                {
                    // call method get budget tab by id
                    var budgetTabUpdate = SingletonIpl.GetInstance<BudgetTabBll>().Get(budgetTab.BudgetTabId);

                    // set data update
                    budgetTabUpdate.AnnualSales = budgetTab.AnnualSales;
                    budgetTabUpdate.TargetColumn = budgetTab.TargetColumn;
                    budgetTabUpdate.UpdatedDate = DateTime.Now;
                    budgetTabUpdate.UpdatedUserId = CurrentUser.UserId;

                    // call method save budget tab
                    SingletonIpl.GetInstance<BudgetTabBll>().Save(budgetTabUpdate, CurrentUser.UserId);

                    foreach (var budgetItem in budgetTab.BudgetItemModelList)
                    {
                        // call method get budget item by id
                        var budgetItemUpdate = SingletonIpl.GetInstance<BudgetItemBll>().Get(budgetItem.BudgetItemId);

                        // set data update
                        budgetItemUpdate.BudgetItemRow = budgetItem.BudgetItemRow;
                        budgetItemUpdate.UpdatedDate = DateTime.Now;
                        budgetItemUpdate.UpdatedUserId = CurrentUser.UserId;

                        // call method save update budget item
                        SingletonIpl.GetInstance<BudgetItemBll>().Save(budgetItemUpdate, CurrentUser.UserId);
                    }

                    if (budgetTab.TabIndex == 1)
                    {
                        this.UpdateProfitLossToBudget(budgetId, inputMethod, actualNumbersFlg, varianceFlg, budgetTab);
                    }
                }
            }
            catch
            {
                status = false;
                message = "Save change unsuccessful!";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: calculate profit loss data set to budget item in dashboad
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="inputMethod"></param>
        /// <param name="actualNumbersFlg"></param>
        /// <param name="varianceFlg"></param>
        /// <param name="budgetTab"></param>
        private void UpdateProfitLossToBudget(int budgetId, int inputMethod, bool actualNumbersFlg, bool varianceFlg, BudgetTabModel budgetTab)
        {
            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(budgetId);
            budget.InputMethodId = inputMethod;
            budget.ActualNumbersFlg = actualNumbersFlg;
            budget.VarianceFlg = varianceFlg;

            var dataTable = budgetTab.ProfitLossDataTable;
            DataRow salesRow = dataTable.Rows[0];
            budget.Sales = Convert.ToDecimal(salesRow["GrandTotal_ProjectionSales"]);
            budget.SalesPercent = Convert.ToDecimal(salesRow["GrandTotal_ProjectionPercent"]);

            DataRow cogsRow = dataTable.Rows[1];
            budget.COGS = Convert.ToDecimal(cogsRow["GrandTotal_ProjectionSales"]);
            budget.COGSPercent = Convert.ToDecimal(cogsRow["GrandTotal_ProjectionPercent"]);

            DataRow grossProfitRow = dataTable.Rows[2];
            budget.GrossProfit = Convert.ToDecimal(grossProfitRow["GrandTotal_ProjectionSales"]);
            budget.GrossProfitPercent = Convert.ToDecimal(grossProfitRow["GrandTotal_ProjectionPercent"]);

            DataRow payrollRow = dataTable.Rows[3];
            budget.PayrollExpenses = Convert.ToDecimal(payrollRow["GrandTotal_ProjectionSales"]);
            budget.PayrollExpensesPercent = Convert.ToDecimal(payrollRow["GrandTotal_ProjectionPercent"]);

            DataRow operatingProfitRow = dataTable.Rows[4];
            budget.OperatingProfit = Convert.ToDecimal(operatingProfitRow["GrandTotal_ProjectionSales"]);
            budget.OperatingProfitPercent = Convert.ToDecimal(operatingProfitRow["GrandTotal_ProjectionPercent"]);

            DataRow primeCostRow = dataTable.Rows[5];
            budget.PrimeCost = Convert.ToDecimal(primeCostRow["GrandTotal_ProjectionSales"]);
            budget.PrimeCostPercent = Convert.ToDecimal(primeCostRow["GrandTotal_ProjectionPercent"]);

            DataRow operationExpensesRow = dataTable.Rows[6];
            budget.OperatingExpenses = Convert.ToDecimal(operationExpensesRow["GrandTotal_ProjectionSales"]);
            budget.OperatingExpensesPercent = Convert.ToDecimal(operationExpensesRow["GrandTotal_ProjectionPercent"]);

            DataRow netProfitRow = dataTable.Rows[7];
            budget.NetProfitLoss = Convert.ToDecimal(netProfitRow["GrandTotal_ProjectionSales"]);
            budget.NetProfitLossPercent = Convert.ToDecimal(netProfitRow["GrandTotal_ProjectionPercent"]);

            //DataRow netProfitRuningRow = dataTable.Rows[8];
            //budget.Net = Convert.ToDecimal(netProfitRuningRow["GrandTotal_ProjectionSales"]);
            //budget.NetProfitLossPercent = Convert.ToDecimal(netProfitRuningRow["GrandTotal_ProjectionPercent"]);

            DataRow bepRow = dataTable.Rows[9];
            budget.BreakEvenPoint = Convert.ToDecimal(bepRow["GrandTotal_ProjectionSales"]);
            budget.BreakEvenPointPercent = Convert.ToDecimal(bepRow["GrandTotal_ProjectionPercent"]);

            // call method save budget
            SingletonIpl.GetInstance<BudgetBll>().Save(budget, CurrentUser.UserId);
        }

        #endregion

        #region View import actual number by Budget

        /// <summary>
        /// Method: open view screen import actual number by budget
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportActualNumber(int? id, int? budgetTabId, string headerName, string redirectPage)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get all budget tab by budget id
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);

            // check eixts budget tab.
            if (budgetTabList == null || budgetTabList.Count == 0)
            {
                // budget tab is not exists call method add new default to database
                this.AddDefaultValueToBudget(budget);

                // get all budget tab by budget id
                budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
            }

            Mapper.CreateMap<BudgetTab, BudgetTabModel>();
            var budgetTabModelList = Mapper.Map<List<BudgetTab>, List<BudgetTabModel>>(budgetTabList.ToList());

            // get budget item row by budget tab
            foreach (var budgetTabModel in budgetTabModelList)
            {
                // add default value to budget item
                StringBuilder budgetItemRow = new StringBuilder();
                for (int i = 0; i < budgetTabModel.HeaderColumnList.Count; i++)
                {
                    budgetItemRow.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, 0, 0, 0, 0, 0);
                }

                var dataItemByBudgetTab = SingletonIpl.GetInstance<BudgetItemBll>().GetBudgetItemByBudgetTabId(budgetTabModel.BudgetId, budgetTabModel.BudgetTabId, budgetItemRow.ToString(), CurrentUser.UserId);
                Mapper.CreateMap<BudgetItem, BudgetItemModel>();
                budgetTabModel.BudgetItemModelList = Mapper.Map<List<BudgetItem>, List<BudgetItemModel>>(dataItemByBudgetTab.ToList());
            }

            // set budget tab model to model
            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            model.BudgetTabModelList = budgetTabModelList;

            // get category setting by budget id
            var categorySettingList = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budget.BudgetId, CurrentUser.UserId);
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            model.CategorySettingModelList = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(categorySettingList.ToList());
            ViewBag.budgetTabId = budgetTabId;
            ViewBag.HeaderName = headerName;
            ViewBag.RedirectPage = redirectPage;

            // get rest name rest code and token id by user id
            if (budget.RestCode != null && !string.IsNullOrEmpty(budget.RestCode))
            {
                var restCodebyUserId = SingletonIpl.GetInstance<RestActiveCodeBll>().GetByUserIdAndRestCode(CurrentUser.UserId, budget.RestCode);
                ViewBag.RestName = restCodebyUserId.RestName;
            }

            // set role edit flag
            model.EditFlg = (CurrentUser.UserId == model.CreatedUserId);

            return View(model);
        }

        /// <summary>
        /// action upload file template
        /// </summary>
        /// <param name="uploadTemplateFile"></param>
        /// <returns></returns>
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            string physicalPath = string.Empty;

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                        file.SaveAs(physicalPath);
                        var listSheet = GetSheetNames(physicalPath);
                        return Json(new { Status = (listSheet.Count > 0), data = listSheet.ToList(), fileName = fileName }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        if (!string.IsNullOrEmpty(physicalPath))
                        {
                            if (System.IO.File.Exists(physicalPath))
                                System.IO.File.Delete(physicalPath); //here is the error
                        }
                    }
                    finally
                    {
                        file.InputStream.Close();
                        file.InputStream.Dispose();
                        GC.Collect();
                    }
                }
            }
            return Content("");
        }

        /// <summary>
        /// process function mapping excel
        /// </summary>
        /// <param name="budgetId">budgetId</param>
        /// <returns>dtcolumn</returns>
        public ActionResult Mapping(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                // get budget by id
                var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
                if(budget == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // expried date
                var expriedDate = DateTime.Now.AddDays(-1);

                MappingModel model = new MappingModel();

                // set budget id to model
                model.BudgetId = budget.BudgetId;

                // set budget name to model
                model.BudgetName = budget.BudgetName;

                // get file name from cookie
                if (Request.Cookies["fileName"] != null)
                {
                    model.FileName = HttpUtility.UrlDecode(Request.Cookies["fileName"].Value);
                    Request.Cookies["fileName"].Expires = expriedDate;
                }

                // get header name from cookie
                if (Request.Cookies["headerName"] != null)
                {
                    model.HeaderName = HttpUtility.UrlDecode(Request.Cookies["headerName"].Value);
                    Request.Cookies["headerName"].Expires = expriedDate;
                }

                // get sheet index from cookie
                if (Request.Cookies["sheetIndex"] != null)
                {
                    model.SheetIndex = int.Parse(Request.Cookies["sheetIndex"].Value);
                    Request.Cookies["sheetIndex"].Expires = expriedDate;
                }

                // get first column from cookie
                if (Request.Cookies["firstValue"] != null)
                {
                    model.FirstValue = HttpUtility.UrlDecode(Request.Cookies["firstValue"].Value);
                    Request.Cookies["firstValue"].Expires = expriedDate;
                }

                if (!string.IsNullOrEmpty(model.FirstValue))
                {
                    ViewBag.ValueColumn1 = model.FirstValue.Replace("Column ", "");
                    ViewBag.Value1 = model.FirstValue.Replace("Column ", "Value (Col ");
                }

                // get second column from cookie
                if (Request.Cookies["secondValue"] != null)
                {
                    model.SecondValue = HttpUtility.UrlDecode(Request.Cookies["secondValue"].Value);
                    Request.Cookies["secondValue"].Expires = expriedDate;
                }

                if (!string.IsNullOrEmpty(model.SecondValue))
                {
                    ViewBag.ValueColumn2 = model.SecondValue.Replace("Column ", "");
                    ViewBag.Value2 = model.SecondValue.Replace("Column ", "Value (Col ");
                }

                // get function calculate from cookie
                if (Request.Cookies["functionCalculate"] != null)
                {
                    model.FuncCalculate = Request.Cookies["functionCalculate"].Value;
                    Request.Cookies["functionCalculate"].Expires = expriedDate;
                }

                // get budget tab id from cookie
                if (Request.Cookies["budgetTabId"] != null)
                {
                    model.BudgetTabId = int.Parse(Request.Cookies["budgetTabId"].Value);
                    Request.Cookies["budgetTabId"].Expires = expriedDate;
                }

                // get redirect page from cookie
                if (Request.Cookies["RedirectPage"] != null)
                {
                    model.RedirectPage = Request.Cookies["RedirectPage"].Value;
                    Request.Cookies["RedirectPage"].Expires = expriedDate;
                }

                // get all budget tab by budget id
                var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
                Mapper.CreateMap<BudgetTab, BudgetTabModel>();
                var budgetTabModelList = Mapper.Map<List<BudgetTab>, List<BudgetTabModel>>(budgetTabList.ToList());
                var budgetTab = budgetTabModelList.FirstOrDefault();
                ViewBag.HeaderNameList = budgetTab.HeaderColumnList;

                // get all category setting by budget id
                List<CategorySetting> budgetCategorySetting = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budget.BudgetId, CurrentUser.UserId).ToList();
                ViewBag.CategorySettingDataByBudget = budgetCategorySetting.OrderBy(c => c.SortOrder);

                // get budget section
                var BudgetSection = budgetCategorySetting.Where(p => p.ParentCategoryId == 0 && p.CategoryName != BCSCommonData.CATEGORY_DEFAULT_PROFIT_LOSS_TEXT);
                ViewBag.BudgetSectionData = BudgetSection.Select(s => new { CategoryName = s.CategoryName, ParentCategoryId = s.CategorySettingId });

                // get pyhsical path by file name
                var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), model.FileName);

                // get data table by sheet index
                model.DataTableBySheet = this.GetTable(physicalPath, model.SheetIndex);

                // get column list in data table of sheet
                List<string> listColumn = new List<string>();
                foreach (DataColumn column in model.DataTableBySheet.Columns)
                {
                    listColumn.Add("Column " + column.ColumnName);
                }
                ViewBag.ActualNumberColumn = listColumn.ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// process function cancel mapping
        /// </summary>
        public ActionResult Cancel()
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// This method retrieves the excel sheet names from 
        /// an excel workbook.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <returns>String[]</returns>
        private Dictionary<string, int> GetSheetNames(string filename)
        {
            var pck = new OfficeOpenXml.ExcelPackage();
            pck.Load(new System.IO.FileInfo(filename).OpenRead());
            try
            {
                Dictionary<string, int> sheetsList = new Dictionary<string, int>();
                foreach (var excelWorksheet in pck.Workbook.Worksheets)
                {
                    var ws = excelWorksheet;
                    object[,] valueArray = ws.Cells.GetValue<object[,]>();
                    if (valueArray != null)
                    {
                        sheetsList.Add(excelWorksheet.Name, excelWorksheet.Index);
                    }
                }
                return sheetsList;
            }
            catch
            {
                pck.Stream.Close();
                pck.Dispose();
                return null;
            }
            finally
            {
                pck.Stream.Close();
                pck.Dispose();
            }
        }

        /// <summary>
        /// function reader sheet name in file and return data table
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>dataTable</returns>
        private DataTable GetTable(string filename, int sheetIndex)
        {
            var pck = new OfficeOpenXml.ExcelPackage();
            pck.Load(new System.IO.FileInfo(filename).OpenRead());
            var ws = pck.Workbook.Worksheets[sheetIndex];
            DataTable table = new DataTable();
            decimal value;
            bool hasHeader = true;
            try
            {
                object[,] valueArray = ws.Cells.GetValue<object[,]>();
                var start = ws.Dimension.Start;
                var end = ws.Dimension.End;
                int rowCell = start.Row;
                for (int col = start.Column; col <= end.Column; col++)
                {
                    object cellValue = ws.Cells[rowCell, col].Value != null ? ws.Cells[rowCell, col].Value : string.Format("{0}", ws.Cells[1, col].Address.Replace("1", ""));
                    if (decimal.TryParse(cellValue.ToString(), out value))
                    {
                        hasHeader = false;
                    }
                    table.Columns.Add(ws.Cells[rowCell, col].Address.Replace(rowCell.ToString(), ""));
                }
                for (int row = hasHeader ? 1 : 0; row <= valueArray.GetUpperBound(0); row++)
                {
                    bool allNulls = true;
                    System.Data.DataRow dataRow = table.NewRow();
                    for (int col = 0; col <= valueArray.GetUpperBound(1); col++)
                    {
                        if (valueArray[row, col] != null)
                        {
                            dataRow[col] = valueArray[row, col].ToString();
                            allNulls = false;
                        }
                    }
                    if (!allNulls)
                        table.Rows.Add(dataRow);
                }
                return table;
            }
            catch
            {
                return table;
            }
            finally
            {
                pck.Dispose();
            }
        }

        /// <summary>
        /// // Save mapping budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public JsonResult SaveMappingBudget(int? id, int budgetTabId, string headerName, List<MappingBudgetModel> listItem)
        {
            bool status = false;
            string message = string.Empty;

            try
            {
                // get budget by id
                var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
                if (budget == null)
                {
                    message = "This Budget is not exists.";
                }
                // get all budget tab by budget id
                var budgetTab = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId).FirstOrDefault(tb => tb.BudgetTabId.Equals(budgetTabId));
                Mapper.CreateMap<BudgetTab, BudgetTabModel>();
                var budgetTabModel = Mapper.Map<BudgetTab, BudgetTabModel>(budgetTab);
                int indexByHeader = budgetTabModel.HeaderColumnList.IndexOf(headerName);

                var dataItemByBudgetTab = SingletonIpl.GetInstance<BudgetItemBll>().GetBudgetItemByBudgetTabId(budgetTabModel.BudgetId, budgetTabModel.BudgetTabId, string.Empty, CurrentUser.UserId);
                Mapper.CreateMap<BudgetItem, BudgetItemModel>();
                var budgetItemModelList = Mapper.Map<List<BudgetItem>, List<BudgetItemModel>>(dataItemByBudgetTab.ToList());

                foreach (var obj in listItem)
                {
                    // get budget item model by category setting id
                    var budgetItemModel = budgetItemModelList.FirstOrDefault(p => p.CategorySettingId == obj.CategorySettingId);
                    if (budgetItemModel != null)
                    {
                        // set mearge data to item Actual Sales by index
                        var budgetItemList = budgetItemModel.BudgetItemList;
                        budgetItemList[indexByHeader].ActualSales = obj.MergedValue;

                        // reset data list to model
                        budgetItemModel.BudgetItemList = budgetItemList;

                        // get budget item by budget item id
                        var budgetItem = dataItemByBudgetTab.FirstOrDefault(p => p.BudgetItemId == budgetItemModel.BudgetItemId);

                        // update data to empty
                        budgetItem.BudgetItemRow = budgetItemModel.BudgetItemRow;
                        budgetItem.UpdatedDate = DateTime.Now;
                        budgetItem.UpdatedUserId = CurrentUser.UserId;

                        // call method save
                        SingletonIpl.GetInstance<BudgetItemBll>().Save(budgetItem, CurrentUser.UserId);
                    }
                }
                status = true;
                message = "Successfully!";
            }
            catch (Exception)
            {
                message = "unsuccessfully!";
            }
            // delete file before Successfully
            //var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), HttpUtility.UrlDecode(Request.Cookies["fileName"].Value));
            //FileInfo destination = new FileInfo(physicalPath);
            //if (destination.Exists)
            //{
            //    destination.Delete();
            //}
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Variance Report

        /// <summary>
        /// Method: show variance report setting
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public ActionResult ShowVarianceReportSetting(int? id, int tabId)
        {
            // check parament input
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }
 
            // set budget tab model to model
            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);
            model.BudgetTabModelList = new List<BudgetTabModel>();
            model.BudgetTabModelList.Add(this.GetDataByBudgetTabId(budget.BudgetId, tabId));

            // get category setting by budget id
            var categorySettingList = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budget.BudgetId, CurrentUser.UserId);
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            model.CategorySettingModelList = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(categorySettingList.ToList());

            // return view variance report setting
            return PartialView("_ViewVarianceReportSetting", model);
        }

        /// <summary>
        /// Method: show data variance report
        /// </summary>
        /// <param name="budgetTabModelList"></param>
        /// <returns></returns>
        public ActionResult ShowVarianceReport(int? id, int budgetTabId, string sectionName, bool isAllSection,
            List<BudgetTabModel> budgetTabModelList, List<int> categorySettingIdList, List<HeaderItemModel> headerColumnIndexList)
        {
            // get budget by id
            var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
            if (budget == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // convert budget to budget model
            Mapper.CreateMap<Budget, BudgetModel>();
            var model = Mapper.Map<Budget, BudgetModel>(budget);

            if (budgetTabModelList == null)
            {
                // add budget tab model from db to list
                budgetTabModelList = new List<BudgetTabModel>();
                budgetTabModelList.Add(this.GetDataByBudgetTabId(budget.BudgetId, budgetTabId));
            }

            // set budget tab model to model
            model.BudgetTabModelList = budgetTabModelList;

            // get category setting by budget id
            var categorySettingList = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budget.BudgetId, CurrentUser.UserId);
            Mapper.CreateMap<CategorySetting, CategorySettingModel>();
            model.CategorySettingModelList = Mapper.Map<List<CategorySetting>, List<CategorySettingModel>>(categorySettingList.ToList());

            // ViewBag.BudgetTabModel = budgetTabModel;
            ViewBag.categorySettingId = categorySettingIdList;
            ViewBag.HeaderColumnIndexList = headerColumnIndexList;
            ViewBag.isAllSection = isAllSection;
            ViewBag.sectionName = sectionName.Replace("_", " ");

            return PartialView("_ViewVarianceReport", model);
        }

        /// <summary>
        /// Method: export data variance to excel
        /// </summary>
        /// <param name="budgetTabModelList"></param>
        /// <returns></returns>
        public JsonResult ExportVarianceReport(int id, int inputMethod, bool isAllSection, bool isVarianceReport,
            List<BudgetTabModel> budgetTabModelList, List<int> categorySettingIdList, List<HeaderItemModel> headerColumnIndexList)
        {
            // get first tab
            var budgetTabModel = budgetTabModelList.FirstOrDefault();
            string year = budgetTabModel.TabName;
            if (budgetTabModel.TabName.IndexOf("Period budget") != -1)
            {
                year = budgetTabModel.TabName.Split('-')[1].Trim();
            }

            // create folder if not exists
            string uploadFolder = "~/Uploads/Budget";
            if (!System.IO.Directory.Exists(Server.MapPath(uploadFolder)))
                System.IO.Directory.CreateDirectory(Server.MapPath(uploadFolder));

            // create file name by path
            string fileName = isVarianceReport ? string.Format("Variance_Report_{0}.xlsx", year) : string.Format("Budget_Report_{0}.xlsx", year);
            string downloadFile = Path.Combine(Server.MapPath(uploadFolder), fileName);
            try
            {
                // delete file if exists, after create new file
                FileInfo destination = new FileInfo(downloadFile);
                if (destination.Exists)
                {
                    destination.Delete();
                    destination = new FileInfo(downloadFile);
                }

                // write data to excel file
                using (ExcelPackage pck = new ExcelPackage(destination))
                {
                    this.WriteDataToExcelFile(id, inputMethod, isAllSection, isVarianceReport, budgetTabModel, categorySettingIdList, headerColumnIndexList, pck);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json(new
            {
                status = true,
                Url = uploadFolder + "/" + fileName
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: common write budget tab to excel file
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="inputMethod"></param>
        /// <param name="isAllSection"></param>
        /// <param name="isVarianceReport"></param>
        /// <param name="budgetTab"></param>
        /// <param name="categorySettingIdList"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="pck"></param>
        private void WriteDataToExcelFile(int budgetId, int inputMethod, bool isAllSection, bool isVarianceReport,
            BudgetTabModel budgetTabModel, List<int> categorySettingIdList, List<HeaderItemModel> headerColumnIndexList, ExcelPackage pck)
        {
            // get category setting by budget id
            var categorySettingList = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(budgetId, CurrentUser.UserId);

            // get profit loss
            var profitLossParentCategory = categorySettingList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_PROFIT_LOSS_TEXT);

            // get sales category
            var salesParentCategory = categorySettingList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT);

            // get cogs category
            var cogsParentCategory = categorySettingList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT);

            // get payroll expenses category
            var payrollExpensesParentCategory = categorySettingList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT);

            // get operation category
            var operationParentCategory = categorySettingList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_OPERATION_EXPENSES_TEXT);

            // data table in tab
            List<BudgetItemModel> dataRowBySales, dataRowByCogs, dataRowByPayroll, dataRowByPayrollIsTax, dataRowOperation, dataRowProfitLoss;
            if (categorySettingIdList == null)
            {
                dataRowBySales = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == salesParentCategory.CategorySettingId).OrderBy(s => s.SortOrder).ToList();
                dataRowByCogs = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == cogsParentCategory.CategorySettingId).OrderBy(s => s.SortOrder).ToList();
                dataRowByPayroll = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == payrollExpensesParentCategory.CategorySettingId && s.IsTaxCost == false).OrderBy(s => s.SortOrder).ToList();
                dataRowByPayrollIsTax = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == payrollExpensesParentCategory.CategorySettingId && s.IsTaxCost == true).OrderBy(s => s.SortOrder).ToList();
                dataRowOperation = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == operationParentCategory.CategorySettingId).OrderBy(s => s.SortOrder).ToList();
                dataRowProfitLoss = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == profitLossParentCategory.CategorySettingId).OrderBy(s => s.SortOrder).ToList();
            }
            else
            {
                dataRowBySales = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == salesParentCategory.CategorySettingId && categorySettingIdList.Contains(s.CategorySettingId)).OrderBy(s => s.SortOrder).ToList();
                dataRowByCogs = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == cogsParentCategory.CategorySettingId && categorySettingIdList.Contains(s.CategorySettingId)).OrderBy(s => s.SortOrder).ToList();
                dataRowByPayroll = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == payrollExpensesParentCategory.CategorySettingId && s.IsTaxCost == false && categorySettingIdList.Contains(s.CategorySettingId)).OrderBy(s => s.SortOrder).ToList();
                dataRowByPayrollIsTax = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == payrollExpensesParentCategory.CategorySettingId && s.IsTaxCost == true && categorySettingIdList.Contains(s.CategorySettingId)).OrderBy(s => s.SortOrder).ToList();
                dataRowOperation = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == operationParentCategory.CategorySettingId && categorySettingIdList.Contains(s.CategorySettingId)).OrderBy(s => s.SortOrder).ToList();
                dataRowProfitLoss = budgetTabModel.BudgetItemModelList.Where(s => s.ParentCategoryId == profitLossParentCategory.CategorySettingId && categorySettingIdList.Contains(s.CategorySettingId)).OrderBy(s => s.SortOrder).ToList();
            }

            // init data table in tab
            System.Data.DataTable profitLossTableCalculate = budgetTabModel.ProfitLossDataTable;

            // calculate grand total projection sales by section sales
            decimal grandTotalProjectionSalesBySectionSales = 0;
            foreach (var item in headerColumnIndexList)
            {
                grandTotalProjectionSalesBySectionSales += budgetTabModel.SalesTotal[item.HeaderIndex].ProjectionSales;
            }

            string sheetName = isVarianceReport ? "Variance_Report" : "Budget_Report";
            ExcelWorksheet sheet = pck.Workbook.Worksheets.Add(sheetName);
            sheet.View.FreezePanes(7, 2);

            // hidden grid line
            sheet.View.ShowGridLines = false;

            // add header to file
            this.AddHeaderExcel(isVarianceReport, inputMethod, headerColumnIndexList, budgetTabModel, sheet);

            // init start row index is 7
            var startRow = 7;

            // mapping sales category for cogs
            IDictionary<int, int> dataRowIndexBySales = new Dictionary<int, int>();

            int salesTotalRowIndex = 0, cogsTotalRowIndex = 0, payrollTotalRowIndex = 0, operationTotalRowIndex = 0;

            // write data row by sales
            if (dataRowBySales.Count > 0)
            {
                this.WriteDataRowBySales(isVarianceReport, headerColumnIndexList, dataRowBySales, sheet, ref startRow, dataRowIndexBySales);
                salesTotalRowIndex = startRow;
                startRow++;
            }

            // write data row by cogs
            if (dataRowByCogs.Count > 0)
            {
                this.WriteDataRowByCogs(isVarianceReport, headerColumnIndexList, dataRowByCogs, sheet, ref startRow, dataRowIndexBySales);
                cogsTotalRowIndex = startRow;
                startRow++;
            }

            // mapping cogs category is prime cost
            List<int> dataRowIndexByIsPrimeCost = new List<int>();

            // write data row by payroll
            if (dataRowByPayroll.Count > 0 || dataRowByPayrollIsTax.Count > 0)
            {
                this.WriteDataRowByPayroll(isVarianceReport, headerColumnIndexList, dataRowByPayroll, dataRowByPayrollIsTax, sheet, ref startRow, dataRowIndexByIsPrimeCost);
                payrollTotalRowIndex = startRow;
                startRow++;
            }

            // write data row by operation
            if (dataRowOperation.Count > 0)
            {
                this.WriteDataRowByOperation(isVarianceReport, headerColumnIndexList, dataRowOperation, sheet, ref startRow);
                operationTotalRowIndex = startRow;
                startRow++;
            }

            // write data row by profit loss
            if (isAllSection || dataRowProfitLoss.Count > 0)
            {
                if (salesTotalRowIndex > 0)
                {
                    this.WriteDataRowByProfitLoss(isVarianceReport, headerColumnIndexList, budgetTabModel,
                        salesTotalRowIndex, cogsTotalRowIndex, payrollTotalRowIndex, operationTotalRowIndex,
                        sheet, ref startRow, dataRowIndexByIsPrimeCost);
                }
                else
                {
                    this.WriteDataRowByProfitLoss(isVarianceReport, headerColumnIndexList, dataRowProfitLoss, sheet, ref startRow);
                }
            }

            // set auto width
            // sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            //Convert into a string for the range.
            string ColumnString = "A1:" + sheet.Dimension.End.Address;

            // set font size
            sheet.Cells[ColumnString].Style.Font.Size = 10;

            // set default column width
            sheet.DefaultColWidth = 12;

            // call save
            pck.Save();
        }

        /// <summary>
        /// Method: write data in table profit loss to excel file
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="dataRowProfitLoss"></param>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        private void WriteDataRowByProfitLoss(bool isVarianceReport, List<HeaderItemModel> headerColumnIndexList,
           List<BudgetItemModel> dataRowProfitLoss,
           ExcelWorksheet sheet, ref int startRow)
        {
            // init start column index is 1
            var startColumn = 1;

            // init column size in budget item
            var columnInBudgetItem = isVarianceReport ? 6 : 2;

            // write sales section header row
            ExcelRange cell = sheet.Cells[startRow, startColumn, startRow, headerColumnIndexList.Count * columnInBudgetItem + (1 + columnInBudgetItem)];
            cell.Merge = true;
            this.AddValueFormatCell(cell, false, BCSCommonData.CATEGORY_DEFAULT_PROFIT_LOSS_TEXT, true, string.Empty, false);

            // next row index
            startRow++;

            // target sales total formula
            string targetSalesFormula = string.Empty;
            for (int i = 0; i < headerColumnIndexList.Count; i++)
            {
                targetSalesFormula += targetSalesFormula.Length == 0 ? "" : "+";
                targetSalesFormula += sheet.Cells[4, (i * columnInBudgetItem) + 2].Address;
            }

            // grand total formula
            string grandTotalSalesFormula = string.Empty;
            string grandTotalActualFormula = string.Empty;
            string grandTotalVarianceFormula = string.Empty;

            // init net profit loss row index
            var netProfitLossRowIndex = 0;

            #region Write sales row
            foreach (var itemDetail in dataRowProfitLoss)
            {
                if (itemDetail.CategoryName == "Net Profit/Loss")
                {
                    netProfitLossRowIndex = startRow;
                }

                // reset start column index;
                startColumn = 1;

                // write category children name
                ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cellCategoryList, false, itemDetail.CategoryName, false, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // loop budget item by header column index list
                foreach (var item in headerColumnIndexList)
                {
                    // get budget item detail by index
                    var budgetItem = itemDetail.BudgetItemList[item.HeaderIndex];

                    // Write data row by category is all column
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                    }

                    // get target cell address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // add  projectionsales and percent
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);
                    this.AddValueFormatCell(percentCell, true, string.Format("=({0} / {1})", salesCell.Address, targetCellAddress), false, FORMAT_PERCENT, true);

                    if (isVarianceReport)
                    {
                        // total actual sales address
                        var totalActualCellAddress = sheet.Cells[4, startColumn].Address;

                        // write actual sales
                        ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualSalesCell, false, budgetItem.ActualSales, false, FORMAT_CURRENCY, true);

                        // write actual percent
                        ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalActualCellAddress, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                        // write variance sales
                        ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write variance percent
                        ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                    }
                }

                // reset formula by running total net profit loss
                if (itemDetail.CategoryName == "Net Profit Running Total")
                {
                    if (grandTotalSalesFormula.Length > 3)
                        grandTotalSalesFormula = grandTotalSalesFormula.Substring(grandTotalSalesFormula.LastIndexOf("+") + 1);
                    if(grandTotalActualFormula.Length > 3)
                        grandTotalActualFormula = grandTotalActualFormula.Substring(grandTotalActualFormula.LastIndexOf("+") + 1);
                    if (grandTotalVarianceFormula.Length > 3)
                        grandTotalVarianceFormula = grandTotalVarianceFormula.Substring(grandTotalVarianceFormula.LastIndexOf("+") + 1);
                }

                // write grand total sales
                ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                // write grand total percent
                ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write grand total actual sales
                    ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("=IF(({1})>({0}), -({0}), {0})", grandTotalActualFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                    // write grand total actual percent
                    ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell.Address, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // write grand total variance sales
                    ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // write grand total variance percent
                    ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            #region Write total sales row
            // column index
            startColumn = 1;

            // TOTAL SALES
            ExcelRange totalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalSalesCell, false, "PROFIT/LOSS", true, string.Empty, true);

            // grand total formula
            grandTotalSalesFormula = string.Empty;
            grandTotalActualFormula = string.Empty;
            grandTotalVarianceFormula = string.Empty;

            // Write total row is all column
            foreach (var item in headerColumnIndexList)
            {
                grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                if (isVarianceReport)
                {
                    grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                    grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                    grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                    grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                }

                // get target cell address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // total Budgeted projection sales
                var salesFormula = string.Format("={0}", sheet.Cells[netProfitLossRowIndex, startColumn].Address);
                ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                // total Budgeted priojection percent
                ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    var actualFormula = string.Format("={0}", sheet.Cells[netProfitLossRowIndex, startColumn].Address);
                    ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                    // total actual percent
                    ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSalesCell, true, string.Format("={0}-{1}", totalActualSalesCell.Address, totalSaledCell.Address), true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }
            }

            // Write grand total is total row
            ExcelRange totalProjectionSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalProjectionSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

            //Total Budget percent
            ExcelRange totalPercentSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // total actual sales
                ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} > ({0}), -({0}), {0})", grandTotalActualFormula, totalProjectionSalesCell.Address), true, FORMAT_CURRENCY, true);

                //total actual percent
                ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalActualCell.Address), true, FORMAT_PERCENT, true);

                // total variance sales
                ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                // total variance percent
                ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion
        }

        /// <summary>
        /// Medthod: write data profit loss to excel file
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="budgetTab"></param>
        /// <param name="salesTotalRowIndex"></param>
        /// <param name="cogsTotalRowIndex"></param>
        /// <param name="payrollTotalRowIndex"></param>
        /// <param name="operationTotalRowIndex"></param>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        private void WriteDataRowByProfitLoss(bool isVarianceReport, List<HeaderItemModel> headerColumnIndexList, BudgetTabModel budgetTabModel,
            int salesTotalRowIndex, int cogsTotalRowIndex, int payrollTotalRowIndex, int operationTotalRowIndex,
            ExcelWorksheet sheet, ref int startRow, List<int> dataRowIndexByIsPrimeCost)
        {
            // write category parent name
            int startColumn = 1;
            ExcelRange cell = sheet.Cells[startRow, startColumn, startRow, headerColumnIndexList.Count * 6 + 7];
            cell.Merge = true;
            this.AddValueFormatCell(cell, false, BCSCommonData.CATEGORY_DEFAULT_PROFIT_LOSS_TEXT, true, string.Empty, false);

            // set total profit loss row index
            var totalProfitLossRowIndex = startRow + 11;

            // next row index
            startRow++;

            #region Write Sales
            // reset start column index;
            startColumn = 1;

            // write category children name
            ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(cellCategoryList, false, BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT, false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // write projection sales
                string salesCellAddress = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, string.Format("={0}", salesCellAddress), false, FORMAT_CURRENCY, true);

                // write projection percent
                string salesPercentCellAddress = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("={0}", salesPercentCellAddress), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write actual sales
                    string actualSalesCellAddress = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, string.Format("={0}", actualSalesCellAddress), false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, 1)", actualSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // write grand total sales
            string grandTotalSalesAddressBySales = sheet.Cells[salesTotalRowIndex, startColumn].Address;
            ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesAddressBySales), true, FORMAT_CURRENCY, true);

            // write grand total percent
            ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF({0} = 0, 0, 1)", grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write grand total actual sales
                string grandTotalActualBySalesAddress = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("={0}", grandTotalActualBySalesAddress), true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell.Address, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}-{1}", grandTotalActualSalesCell.Address, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }
            #endregion

            // next row index
            startRow++;

            #region Write COGS
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT, false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // write projection sales
                string salesAddressByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, string.Format("={0}", salesAddressByCogs), false, FORMAT_CURRENCY, true);

                // write projection percent
                string percentAddressByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("={0}", percentAddressByCogs), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write actual sales
                    string actualAddressByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, string.Format("={0}", actualAddressByCogs), false, FORMAT_CURRENCY, true);

                    // write actual percent
                    string actualPercentAddressByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("={0}", actualPercentAddressByCogs), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // write grand total sales
            string grandTotalSalesByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
            var cogsGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(cogsGrandTotalSalesCell, true, string.Format("={0}", grandTotalSalesByCogs), true, FORMAT_CURRENCY, true);

            // write grand total percent
            string grandTotalPercentByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
            var cogsGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(cogsGrandTotalPercentCell, true, string.Format("={0}", grandTotalPercentByCogs), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write grand total actual sales
                string grandTotalActualByCogs = sheet.Cells[cogsTotalRowIndex, startColumn].Address;
                var cogsGrandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cogsGrandTotalActualSalesCell, true, string.Format("={0}", grandTotalActualByCogs), true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var cogsGrandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cogsGrandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", cogsGrandTotalSalesCell.Address, cogsGrandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var cogsGrandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cogsGrandTotalVarianceSalesCell, true, string.Format("={0}-{1}", cogsGrandTotalActualSalesCell.Address, cogsGrandTotalSalesCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var cogsGrandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cogsGrandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", cogsGrandTotalSalesCell.Address, cogsGrandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }
            #endregion

            // next row index
            startRow++;

            #region Write Gross Profit
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Gross Profit", false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // get target address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // write projection sales
                string salesFormula = string.Format("={0}-{1}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, salesFormula, false, FORMAT_CURRENCY, true);

                // write projection percent
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write actual sales
                    string actualSalesAddress = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                    string actualSalesFormula = string.Format("={0}-{1}", actualSalesAddress, sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, actualSalesFormula, false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesAddress, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // Write grand total by row
            var grossGrandTotalSalesFormula = string.Format("={0} - {1}", grandTotalSalesAddressBySales, sheet.Cells[cogsTotalRowIndex, startColumn]);
            var grossGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grossGrandTotalSalesCell, true, grossGrandTotalSalesFormula, true, FORMAT_CURRENCY, true);

            // write grand total percent
            var grossGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grossGrandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, grossGrandTotalSalesCell), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write grand total actual sales
                var grossGrandTotalActualFormula = string.Format("={0} - {1}", sheet.Cells[salesTotalRowIndex, startColumn], sheet.Cells[cogsTotalRowIndex, startColumn]);
                var grossGrandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grossGrandTotalActualSalesCell, true, grossGrandTotalActualFormula, true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var grossGrandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grossGrandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grossGrandTotalSalesCell.Address, grossGrandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var grossGrandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grossGrandTotalVarianceSalesCell, true, string.Format("={0}-{1}", grossGrandTotalActualSalesCell.Address, grossGrandTotalSalesCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var grossGrandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grossGrandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grossGrandTotalSalesCell.Address, grossGrandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }
            #endregion

            // next row index
            startRow++;

            #region Write Payroll Expenses
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT, false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // write projection sales
                string salesAddresByPayroll = sheet.Cells[payrollTotalRowIndex, startColumn].Address;
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, string.Format("={0}", salesAddresByPayroll), false, FORMAT_CURRENCY, true);

                // write projection percent
                string percentAddressByPayroll = sheet.Cells[payrollTotalRowIndex, startColumn].Address;
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("={0}", percentAddressByPayroll), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // get actual sales of section sales address
                    string actualCellAddressOfSales = sheet.Cells[salesTotalRowIndex, startColumn].Address;

                    // write actual sales
                    string actualSalesFormula = sheet.Cells[payrollTotalRowIndex, startColumn].Address;
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, string.Format("={0}", actualSalesFormula), false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualCellAddressOfSales, actualSalesFormula), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // write garnd total sales
            var payrollGrandTotalSalesAddress = sheet.Cells[payrollTotalRowIndex, startColumn].Address;
            var payrollGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(payrollGrandTotalSalesCell, true, string.Format("={0}", payrollGrandTotalSalesAddress), true, FORMAT_CURRENCY, true);

            // write grand total percent
            var payrollGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(payrollGrandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, payrollGrandTotalSalesCell), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write grand total actual sales
                var payrollGrandTotalActualAddress = sheet.Cells[payrollTotalRowIndex, startColumn].Address;
                var payrollGrandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(payrollGrandTotalActualSalesCell, true, string.Format("={0}", payrollGrandTotalActualAddress), true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var payrollGrandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(payrollGrandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", payrollGrandTotalSalesCell.Address, payrollGrandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var payrollGrandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(payrollGrandTotalVarianceSalesCell, true, string.Format("={0}-{1}", payrollGrandTotalActualSalesCell.Address, payrollGrandTotalSalesCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var payrollGrandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(payrollGrandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", payrollGrandTotalSalesCell.Address, payrollGrandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion

            // next row index
            startRow++;

            #region Write Operating Profit
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Operating Profit", false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // get target address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // write projection sales
                string salesFormula = string.Format("={0}-{1}-{2}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address);
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, salesFormula, false, FORMAT_CURRENCY, true);

                // write projection percent
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write actual sales
                    string actualSalesAddress = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                    string actualSalesFormula = string.Format("={0}-{1}-{2}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address);
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, actualSalesFormula, false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesAddress, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // write grand total sales
            string operatingGrandTotalSalesFormula = string.Format("={0}-{1}-{2}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address);
            var operatingGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(operatingGrandTotalSalesCell, true, operatingGrandTotalSalesFormula, true, FORMAT_CURRENCY, true);

            // write grand total percent
            var operatingGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(operatingGrandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, operatingGrandTotalSalesCell), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write grand total actual sales
                var operatingGrandTotalActualFormula = string.Format("={0}-{1}-{2}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address);
                var operatingGrandTotalActualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operatingGrandTotalActualCell, true, operatingGrandTotalActualFormula, true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var operatingGrandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operatingGrandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", operatingGrandTotalSalesCell.Address, operatingGrandTotalActualCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var operatingGrandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operatingGrandTotalVarianceSalesCell, true, string.Format("={0}-{1}", operatingGrandTotalActualCell.Address, operatingGrandTotalSalesCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var operatingGrandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operatingGrandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", operatingGrandTotalSalesCell.Address, operatingGrandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion

            // next row index
            startRow++;

            #region Write Prime Cost
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Prime Cost", false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // get target address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // write projection sales
                string salesFormula = string.Format("={0}", sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                dataRowIndexByIsPrimeCost.ForEach(rowIndexIsPrimeCost => salesFormula += string.Format(" + {0}", sheet.Cells[rowIndexIsPrimeCost, startColumn].Address));

                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, salesFormula, false, FORMAT_CURRENCY, true);

                // write projection percent
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write actual sales
                    string actualSalesAddressOfSales = sheet.Cells[salesTotalRowIndex, startColumn].Address;
                    string actualSalesFormula = string.Format("={0}", sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                    dataRowIndexByIsPrimeCost.ForEach(rowIndexIsPrimeCost => actualSalesFormula += string.Format(" + {0}", sheet.Cells[rowIndexIsPrimeCost, startColumn].Address));

                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, actualSalesFormula, false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesAddressOfSales, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // grand total formula
            string salesPrimeCostFormula = string.Format("={0}", sheet.Cells[cogsTotalRowIndex, startColumn].Address);
            dataRowIndexByIsPrimeCost.ForEach(rowIndexIsPrimeCost => salesPrimeCostFormula += string.Format(" + {0}", sheet.Cells[rowIndexIsPrimeCost, startColumn].Address));

            // write grand total sales
            var salesPrimeCostCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(salesPrimeCostCell, true, salesPrimeCostFormula, true, FORMAT_CURRENCY, true);

            // write grand total percent
            var grandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, salesPrimeCostCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // actual formula
                var actualPrimeCostFormula = string.Format("={0}", sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                dataRowIndexByIsPrimeCost.ForEach(rowIndexIsPrimeCost => actualPrimeCostFormula += string.Format(" + {0}", sheet.Cells[rowIndexIsPrimeCost, startColumn].Address));

                // write grand total actual sales
                var actualPrimeCostCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(actualPrimeCostCell, true, actualPrimeCostFormula, true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var actualPrimeCostPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(actualPrimeCostPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesPrimeCostCell.Address, actualPrimeCostCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var variancePrimeCostSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(variancePrimeCostSalesCell, true, string.Format("={0}-{1}", actualPrimeCostCell.Address, salesPrimeCostCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var variancePrimeCostPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(variancePrimeCostPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesPrimeCostCell.Address, variancePrimeCostSalesCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion

            // next row index
            startRow++;

            #region Write Operating Expenses
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Operating Expenses", false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // write projection sales
                string salesFormula = string.Format("={0}", sheet.Cells[operationTotalRowIndex, startColumn].Address);
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, salesFormula, false, FORMAT_CURRENCY, true);

                // write projection percent
                string percentFormula = string.Format("={0}", sheet.Cells[operationTotalRowIndex, startColumn].Address);
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, percentFormula, false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // get actual sales of section sales address
                    string actualCellAddressOfSales = sheet.Cells[salesTotalRowIndex, startColumn].Address;

                    // write actual sales
                    string actualSalesFormula = string.Format("={0}", sheet.Cells[operationTotalRowIndex, startColumn].Address);
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, actualSalesFormula, false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualCellAddressOfSales, actualSalesCell), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // write grand total sales
            var salesAddressByOperation = string.Format("={0}", sheet.Cells[operationTotalRowIndex, startColumn].Address);
            var operationGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(operationGrandTotalSalesCell, true, salesAddressByOperation, true, FORMAT_CURRENCY, true);

            // write grand total percent
            var operationGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(operationGrandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, operationGrandTotalSalesCell), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // get grand total actual of section sales
                string grandTotalActualsAddressOfSales = sheet.Cells[salesTotalRowIndex, startColumn].Address;

                // write grand total actual sales
                var actualFormula = string.Format("={0}", sheet.Cells[operationTotalRowIndex, startColumn].Address);
                var operationGrandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operationGrandTotalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var operationGrandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operationGrandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalActualsAddressOfSales, operationGrandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var operationGrandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operationGrandTotalVarianceSalesCell, true, string.Format("={0}-{1}", operationGrandTotalActualSalesCell.Address, operationGrandTotalSalesCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var operationGrandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operationGrandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", operationGrandTotalSalesCell.Address, operationGrandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion

            // next row index
            startRow++;

            #region Write Net Profit/Loss
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], false, "PROFIT/LOSS", true, string.Empty, true);
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Net Profit/Loss", false, string.Empty, false);

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                // get target address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // write projection sales
                string salesFormula = string.Format("={0}-{1}-{2}-{3}", sheet.Cells[salesTotalRowIndex, startColumn], sheet.Cells[cogsTotalRowIndex, startColumn], sheet.Cells[payrollTotalRowIndex, startColumn], sheet.Cells[operationTotalRowIndex, startColumn]);
                ExcelRange salesCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(salesCell, true, salesFormula, false, FORMAT_CURRENCY, true);

                // write projection sales of Total Profit Lost Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", salesCell), true, FORMAT_CURRENCY, true);

                // next column index
                startColumn++;

                // write projection percent
                ExcelRange percentCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), false, FORMAT_PERCENT, true);

                // write projection percent of Total Profit Loss Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", percentCell), true, FORMAT_PERCENT, true);

                // next column index
                startColumn++;

                if (isVarianceReport)
                {
                    // get actual sales address of section sales
                    string actualSalesAddressOfSales = sheet.Cells[salesTotalRowIndex, startColumn].Address;

                    // write actual sales
                    string actualSalesFormula = string.Format("={0}-{1}-{2}-{3}", sheet.Cells[salesTotalRowIndex, startColumn], sheet.Cells[cogsTotalRowIndex, startColumn], sheet.Cells[payrollTotalRowIndex, startColumn], sheet.Cells[operationTotalRowIndex, startColumn]);
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn];
                    this.AddValueFormatCell(actualSalesCell, true, actualSalesFormula, false, FORMAT_CURRENCY, true);

                    // write actual sales of Total Profit Loss Row
                    if (totalProfitLossRowIndex > 0)
                        this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", actualSalesCell), true, FORMAT_CURRENCY, true);

                    // next column index
                    startColumn++;

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesAddressOfSales, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write actual percent of Total Profit Loss Row
                    if (totalProfitLossRowIndex > 0)
                        this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", actualPercentCell), true, FORMAT_PERCENT, true);

                    // next column index
                    startColumn++;

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance sales of Total Profit Loss Row
                    if (totalProfitLossRowIndex > 0)
                        this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", varianceSalesCell), true, FORMAT_CURRENCY, true);

                    // next column index
                    startColumn++;

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write variance sales of Total Profit Loss Row
                    if (totalProfitLossRowIndex > 0)
                        this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", variancePercentCell), true, FORMAT_PERCENT, true);

                    // next column index
                    startColumn++;
                }
            }

            // write grand total sales
            string salesNetProfitLossFormula = string.Format("={0}-{1}-{2}-{3}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address, sheet.Cells[operationTotalRowIndex, startColumn].Address);
            var salesNetProfitLossCell = sheet.Cells[startRow, startColumn];
            this.AddValueFormatCell(salesNetProfitLossCell, true, salesNetProfitLossFormula, true, FORMAT_CURRENCY, true);

            // write projection sales of Total Profit Lost Row
            if (totalProfitLossRowIndex > 0)
                this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", salesNetProfitLossCell.Address), true, FORMAT_CURRENCY, true);

            // next column index
            startColumn++;

            // write grand total percent
            var percentNetProfitLossCell = sheet.Cells[startRow, startColumn];
            this.AddValueFormatCell(percentNetProfitLossCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, salesNetProfitLossCell.Address), true, FORMAT_PERCENT, true);

            // write projection percent of Total Profit Loss Row
            if (totalProfitLossRowIndex > 0)
                this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", percentNetProfitLossCell.Address), true, FORMAT_PERCENT, true);

            // next column index
            startColumn++;

            if (isVarianceReport)
            {
                // write grand total actual sales
                var actualNetProfitLossFormula = string.Format("={0}-{1}-{2}-{3}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address, sheet.Cells[operationTotalRowIndex, startColumn].Address);
                var actualNetProfitLossCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(actualNetProfitLossCell, true, actualNetProfitLossFormula, true, FORMAT_CURRENCY, true);

                // write actual sales of Total Profit Loss Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", actualNetProfitLossCell.Address), true, FORMAT_CURRENCY, true);

                // next column index
                startColumn++;

                // write grand total actual percent
                var actualPercentNetProfitLossCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(actualPercentNetProfitLossCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesNetProfitLossCell.Address, actualNetProfitLossCell.Address), true, FORMAT_PERCENT, true);

                // write actual percent of Total Profit Loss Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", actualPercentNetProfitLossCell.Address), true, FORMAT_PERCENT, true);

                // next column index
                startColumn++;

                // write grand total variance sales
                var varianceSalesNetProfitLossCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(varianceSalesNetProfitLossCell, true, string.Format("={0}-{1}", actualNetProfitLossCell.Address, salesNetProfitLossCell.Address), true, FORMAT_CURRENCY, true);

                // write variance sales of Total Profit Loss Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", varianceSalesNetProfitLossCell), true, FORMAT_CURRENCY, true);

                // next column index
                startColumn++;

                // write grand total variance percent
                var variancePercentNetProfitLossCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(variancePercentNetProfitLossCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesNetProfitLossCell.Address, varianceSalesNetProfitLossCell.Address), true, FORMAT_PERCENT, true);

                // write variance sales of Total Profit Loss Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", variancePercentNetProfitLossCell), true, FORMAT_PERCENT, true);

                // next column index
                startColumn++;
            }

            #endregion

            // next row index
            startRow++;

            #region Write Net Profit Running Total
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Net Profit Running Total", false, string.Empty, false);

            string salesNetRunningFormula = string.Empty, actualNetRunningFormula = string.Empty;

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                salesNetRunningFormula += salesNetRunningFormula.Length == 0 ? "=" : "+";
                salesNetRunningFormula += sheet.Cells[startRow - 1, startColumn].Address;

                // get target address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // write projection sales
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, salesNetRunningFormula, false, FORMAT_CURRENCY, true);

                // write projection percent
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    actualNetRunningFormula += actualNetRunningFormula.Length == 0 ? "=" : "+";
                    actualNetRunningFormula += sheet.Cells[startRow - 1, startColumn + 2].Address;

                    // get actual sales address of section sales
                    string actualSalesAddressOfSales = sheet.Cells[salesTotalRowIndex, startColumn].Address;

                    // write actual sales
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, true, actualNetRunningFormula, false, FORMAT_CURRENCY, true);

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesAddressOfSales, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                }
            }

            // write grand total sales
            var salesNetRunningCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(salesNetRunningCell, true, salesNetRunningFormula, true, FORMAT_CURRENCY, true);

            // write grand total percent
            var percentNetRunningCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(percentNetRunningCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, salesNetRunningCell), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write grand total actual sales
                var actualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(actualCell, true, actualNetRunningFormula, true, FORMAT_CURRENCY, true);

                // write grand total actual percent
                var actualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesNetRunningCell.Address, actualCell.Address), true, FORMAT_PERCENT, true);

                // write grand total variance sales
                var varianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualCell.Address, salesNetRunningCell.Address), true, FORMAT_CURRENCY, true);

                // write grand total variance percent
                var variancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", salesNetRunningCell.Address, varianceSalesCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion

            // next row index
            startRow++;

            #region Write Breakeven Point
            // reset start column index;
            startColumn = 1;

            // write category children name
            this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "Breakeven Point", false, string.Empty, false);

            string bepGrandTotalFormula = string.Empty;

            // Write data row by category is all column
            foreach (var item in headerColumnIndexList)
            {
                bepGrandTotalFormula += bepGrandTotalFormula.Length == 0 ? "=" : "+";
                bepGrandTotalFormula += sheet.Cells[startRow, startColumn].Address;

                // get target address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // write projection sales
                ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesCell, true, string.Format("=IF({0} = 0, 0, {1} / (1 - {2} / {0}))", targetCellAddress, budgetTabModel.FixCostList[item.HeaderIndex], budgetTabModel.VariableCostList[item.HeaderIndex]), false, FORMAT_CURRENCY, true);

                // write projection percent
                ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), false, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write actual sales
                    ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualSalesCell, false, 0, false, FORMAT_CURRENCY, true);
                    actualSalesCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

                    // write actual percent
                    ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(actualPercentCell, false, 0, false, FORMAT_PERCENT, true);
                    actualPercentCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

                    // write variance sales
                    ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(varianceSalesCell, false, 0, false, FORMAT_CURRENCY, true);
                    varianceSalesCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

                    // write variance percent
                    ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(variancePercentCell, false, 0, false, FORMAT_PERCENT, true);
                    variancePercentCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                }
            }

            // write projection sales
            ExcelRange salesBreakevenPointCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(salesBreakevenPointCell, true, bepGrandTotalFormula, true, FORMAT_CURRENCY, true);

            // write projection percent
            ExcelRange percentBreakevenPointCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(percentBreakevenPointCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, salesBreakevenPointCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // write actual sales
                ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(actualSalesCell, false, 0, true, FORMAT_CURRENCY, true);
                actualSalesCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

                // write actual percent
                ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(actualPercentCell, false, 0, true, FORMAT_PERCENT, true);
                actualPercentCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

                // write variance sales
                ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(varianceSalesCell, false, 0, true, FORMAT_CURRENCY, true);
                varianceSalesCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

                // write variance percent
                ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(variancePercentCell, false, 0, true, FORMAT_PERCENT, true);
                variancePercentCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
            }

            #endregion

            // insert row to Budget Report
            if (!isVarianceReport)
            {
                // add new row after row total cogs
                startRow = cogsTotalRowIndex + 1;
                sheet.InsertRow(startRow, 1);

                #region Write Gross Profit
                // reset start column index;
                startColumn = 1;

                // write category children name
                this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "GROSS PROFIT", true, string.Empty, true);

                // Write data row by category is all column
                foreach (var item in headerColumnIndexList)
                {
                    // get target address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // write projection sales
                    string salesFormula = string.Format("={0}-{1}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(salesCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                    // write projection percent
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), true, FORMAT_PERCENT, true);
                }

                // write grand total sales
                grossGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grossGrandTotalSalesCell, true, grossGrandTotalSalesFormula, true, FORMAT_CURRENCY, true);

                // write grand total percent
                grossGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grossGrandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, grossGrandTotalSalesCell), true, FORMAT_PERCENT, true);

                #endregion

                // next row index payroll after insert gross profit
                payrollTotalRowIndex++;
                operationTotalRowIndex++;

                // add new row after row total payroll expenses
                startRow = payrollTotalRowIndex + 1;
                sheet.InsertRow(startRow, 1);

                #region Write Prime Cost
                // reset start column index;
                startColumn = 1;

                // write category children name
                this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "PRIME COST", true, string.Empty, true);

                // Write data row by category is all column
                for (int colIndex = 0; colIndex < headerColumnIndexList.Count; colIndex++)
                {
                    // get target address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // sales formula
                    string salesFormula = string.Format("={0}", sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                    dataRowIndexByIsPrimeCost.ForEach(rowIndexIsPrimeCost => salesFormula += string.Format(" + {0}", sheet.Cells[rowIndexIsPrimeCost + 1, startColumn].Address));

                    // write projection sales
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(salesCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                    // write projection percent
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), true, FORMAT_PERCENT, true);
                }

                salesPrimeCostFormula = string.Format("={0}", sheet.Cells[cogsTotalRowIndex, startColumn].Address);
                dataRowIndexByIsPrimeCost.ForEach(rowIndexIsPrimeCost => salesPrimeCostFormula += string.Format(" + {0}", sheet.Cells[rowIndexIsPrimeCost + 1, startColumn].Address));

                // write grand total sales
                salesPrimeCostCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(salesPrimeCostCell, true, salesPrimeCostFormula, true, FORMAT_CURRENCY, true);

                // write grand total percent
                grandTotalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, salesPrimeCostCell), true, FORMAT_PERCENT, true);
                #endregion

                // next row index payroll after insert prime cost
                operationTotalRowIndex++;

                // add new row after total payroll expenses
                startRow = payrollTotalRowIndex + 2;
                sheet.InsertRow(startRow, 1);

                #region Write Operating Profit
                // reset start column index;
                startColumn = 1;

                // write category children name
                this.AddValueFormatCell(sheet.Cells[startRow, startColumn++], false, "OPERATING PROFIT", true, string.Empty, true);

                // Write data row by category is all column
                foreach (var item in headerColumnIndexList)
                {
                    // get target address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // write projection sales
                    string salesFormula = string.Format("={0}-{1}-{2}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address);
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(salesCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                    // write projection percent
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), true, FORMAT_PERCENT, true);
                }

                // write grand total sales
                operatingGrandTotalSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operatingGrandTotalSalesCell, true, operatingGrandTotalSalesFormula, true, FORMAT_CURRENCY, true);

                // write grand total percent
                operatingGrandTotalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(operatingGrandTotalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, operatingGrandTotalSalesCell), true, FORMAT_PERCENT, true);
                #endregion

                // next row index payroll after insert operating profit
                operationTotalRowIndex++;

                // add new row after row total operation expenses
                startRow = operationTotalRowIndex + 1;
                sheet.InsertRow(startRow, 1);
                totalProfitLossRowIndex = startRow + 1;
                sheet.InsertRow(totalProfitLossRowIndex, 1);

                #region Write Net Profit/Loss
                // reset start column index;
                startColumn = 1;

                // write category children name
                this.AddValueFormatCell(sheet.Cells[startRow, startColumn], false, "NET PROFIT/LOSS", true, string.Empty, true);
                this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], false, "NET PROFIT RUNNING TOTAL", true, string.Empty, true);
                startColumn++;

                string salesRunningTotalFormula = string.Empty;

                // Write data row by category is all column
                for (int colIndex = 0; colIndex < budgetTabModel.HeaderColumnList.Count; colIndex++)
                {
                    // get target address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // write projection sales
                    string salesFormula = string.Format("={0}-{1}-{2}-{3}", sheet.Cells[salesTotalRowIndex, startColumn], sheet.Cells[cogsTotalRowIndex, startColumn], sheet.Cells[payrollTotalRowIndex, startColumn], sheet.Cells[operationTotalRowIndex, startColumn]);
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn];
                    this.AddValueFormatCell(salesCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                    // write projection sales of Total Profit Lost Row
                    if (totalProfitLossRowIndex > 0)
                    {
                        salesRunningTotalFormula += salesRunningTotalFormula.Length == 0 ? "=" : "+";
                        salesRunningTotalFormula += sheet.Cells[startRow, startColumn].Address;
                        this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, salesRunningTotalFormula, true, FORMAT_CURRENCY, true);
                    }

                    // next column index
                    startColumn++;

                    // write projection percent
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn];
                    this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell), true, FORMAT_PERCENT, true);

                    // write projection percent of Total Profit Loss Row
                    if (totalProfitLossRowIndex > 0)
                        this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, sheet.Cells[totalProfitLossRowIndex, startColumn - 1]), true, FORMAT_PERCENT, true);

                    // next column index
                    startColumn++;
                }

                // write grand total sales
                string salesProfitFormula = string.Format("={0}-{1}-{2}-{3}", sheet.Cells[salesTotalRowIndex, startColumn].Address, sheet.Cells[cogsTotalRowIndex, startColumn].Address, sheet.Cells[payrollTotalRowIndex, startColumn].Address, sheet.Cells[operationTotalRowIndex, startColumn].Address);
                var salesProfitCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(salesProfitCell, true, salesProfitFormula, true, FORMAT_CURRENCY, true);

                // write projection sales of Total Profit Lost Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", salesProfitCell), true, FORMAT_CURRENCY, true);

                // next column index
                startColumn++;

                // write grand total percent
                var percentProfitCell = sheet.Cells[startRow, startColumn];
                this.AddValueFormatCell(percentProfitCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesAddressBySales, salesProfitCell), true, FORMAT_PERCENT, true);

                // write projection percent of Total Profit Loss Row
                if (totalProfitLossRowIndex > 0)
                    this.AddValueFormatCell(sheet.Cells[totalProfitLossRowIndex, startColumn], true, string.Format("={0}", percentProfitCell), true, FORMAT_PERCENT, true);

                // next column index
                startColumn++;

                #endregion

            }
        }

        /// <summary>
        /// Method: write data row by operation section
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="dataRowOperation"></param>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        private void WriteDataRowByOperation(bool isVarianceReport, List<HeaderItemModel> headerColumnIndexList,
            List<BudgetItemModel> dataRowOperation,
            ExcelWorksheet sheet, ref int startRow)
        {
            // init start column index is 1
            var startColumn = 1;

            // init column size in budget item
            var columnInBudgetItem = isVarianceReport ? 6 : 2;

            // write sales section header row
            ExcelRange cell = sheet.Cells[startRow, startColumn, startRow, headerColumnIndexList.Count * columnInBudgetItem + (1 + columnInBudgetItem)];
            cell.Merge = true;
            this.AddValueFormatCell(cell, false, BCSCommonData.CATEGORY_DEFAULT_OPERATION_EXPENSES_TEXT, true, string.Empty, false);

            // next row index
            startRow++;

            // target sales total formula
            string targetSalesFormula = string.Empty;
            for (int i = 0; i < headerColumnIndexList.Count; i++)
            {
                targetSalesFormula += targetSalesFormula.Length == 0 ? "" : "+";
                targetSalesFormula += sheet.Cells[4, (i * columnInBudgetItem) + 2].Address;
            }

            // grand total formula
            string grandTotalSalesFormula = string.Empty;
            string grandTotalActualFormula = string.Empty;
            string grandTotalVarianceFormula = string.Empty;

            #region Write sales row
            foreach (var itemDetail in dataRowOperation)
            {
                // reset start column index;
                startColumn = 1;

                // write category children name
                ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cellCategoryList, false, itemDetail.CategoryName, false, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // loop budget item by header column index list
                foreach (var item in headerColumnIndexList)
                {
                    // get budget item detail by index
                    var budgetItem = itemDetail.BudgetItemList[item.HeaderIndex];

                    // Write data row by category is all column
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 2].Address);
                    }

                    // get target cell address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // add  projectionsales and percent
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    if (budgetItem.IsPercentage)
                    {
                        this.AddValueFormatCell(salesCell, true, string.Format("=({0} * {1})", percentCell.Address, targetCellAddress), false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, false, budgetItem.ProjectionPercent / 100, false, FORMAT_PERCENT, true);
                    }
                    else
                    {
                        this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell.Address), false, FORMAT_PERCENT, true);
                    }

                    if (isVarianceReport)
                    {
                        // write actual sales
                        ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualSalesCell, true, string.Format("=IF({1} <= {0}, -{0}, {0})", budgetItem.ActualSales, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write actual percent
                        ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                        // write variance sales
                        ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write variance percent
                        ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                    }
                }

                // write grand total sales
                ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                // write grand total percent
                ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write grand total actual sales
                    ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("=IF(({1}) < ({0}), -({0}), {0})", grandTotalActualFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                    // write grand total actual percent
                    ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell.Address, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // write grand total variance sales
                    ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // write grand total variance percent
                    ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            #region Write total sales row
            // column index
            startColumn = 1;

            // TOTAL SALES
            ExcelRange totalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalSalesCell, false, "TOTAL OPERATING EXPENSES", true, string.Empty, true);

            // grand total formula
            grandTotalSalesFormula = string.Empty;
            grandTotalActualFormula = string.Empty;
            grandTotalVarianceFormula = string.Empty;

            // Write total row is all column
            foreach (var item in headerColumnIndexList)
            {
                grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                if (isVarianceReport)
                {
                    grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                    grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                    grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                    grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 2].Address);
                }

                // get target cell address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // total Budgeted projection sales
                var salesFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowOperation.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                // total Budgeted priojection percent
                ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    var actualFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowOperation.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                    ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                    // total actual percent
                    ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSalesCell, true, string.Format("={0}-{1}", totalActualSalesCell.Address, totalSaledCell.Address), true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }
            }

            // Write grand total is total row
            ExcelRange totalProjectionSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalProjectionSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

            //Total Budget percent
            ExcelRange totalPercentSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // total actual sales
                ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} < ({0}), -({0}), {0})", grandTotalActualFormula, totalProjectionSalesCell.Address), true, FORMAT_CURRENCY, true);

                //total actual percent
                ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalActualCell.Address), true, FORMAT_PERCENT, true);

                // total variance sales
                ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                // total variance percent
                ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion
        }

        /// <summary>
        /// Method: write data row by payroll section
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="dataRowByPayroll"></param>
        /// <param name="dataRowByPayrollIsTax"></param>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        /// <param name="dataRowIndexBySales"></param>
        private void WriteDataRowByPayroll(bool isVarianceReport, List<HeaderItemModel> headerColumnIndexList,
            List<BudgetItemModel> dataRowByPayroll,
            List<BudgetItemModel> dataRowByPayrollIsTax,
            ExcelWorksheet sheet, ref int startRow, List<int> dataRowIndexByIsPrimeCost)
        {
            // init start column index is 1
            var startColumn = 1;

            // init column size in budget item
            var columnInBudgetItem = isVarianceReport ? 6 : 2;

            // write sales section header row
            ExcelRange cell = sheet.Cells[startRow, startColumn, startRow, headerColumnIndexList.Count * columnInBudgetItem + (1 + columnInBudgetItem)];
            cell.Merge = true;
            this.AddValueFormatCell(cell, false, BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT, true, string.Empty, false);

            // next row index
            startRow++;

            // init start row index on payroll section
            int startRowIndexOnPayrollSection = startRow;

            // target sales total formula
            string targetSalesFormula = string.Empty;
            for (int i = 0; i < headerColumnIndexList.Count; i++)
            {
                targetSalesFormula += targetSalesFormula.Length == 0 ? "" : "+";
                targetSalesFormula += sheet.Cells[4, (i * columnInBudgetItem) + 2].Address;
            }

            // grand total formula
            string grandTotalSalesFormula = string.Empty;
            string grandTotalActualFormula = string.Empty;
            string grandTotalVarianceFormula = string.Empty;

            #region Write payroll is not tax row
            foreach (var itemDetail in dataRowByPayroll)
            {
                // add prime cost row index
                if (itemDetail.IsPrimeCost)
                {
                    dataRowIndexByIsPrimeCost.Add(startRow);
                }

                // reset start column index;
                startColumn = 1;

                // write category children name
                ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cellCategoryList, false, itemDetail.CategoryName, false, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // loop budget item by header column index list
                foreach (var item in headerColumnIndexList)
                {
                    // get budget item detail by index
                    var budgetItem = itemDetail.BudgetItemList[item.HeaderIndex];

                    // Write data row by category is all column
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                    }

                    // get target cell address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // add  projectionsales and percent
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    if (budgetItem.IsPercentage)
                    {
                        this.AddValueFormatCell(salesCell, true, string.Format("=({0} * {1})", percentCell.Address, targetCellAddress), false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, false, budgetItem.ProjectionPercent / 100, false, FORMAT_PERCENT, true);
                    }
                    else
                    {
                        this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell.Address), false, FORMAT_PERCENT, true);
                    }

                    if (isVarianceReport)
                    {
                        // write actual sales
                        ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualSalesCell, true, string.Format("=IF({1} <= {0}, -{0}, {0})", budgetItem.ActualSales, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write actual percent
                        ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                        // write variance sales
                        ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write variance percent
                        ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                    }
                }

                // write grand total sales
                ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                // write grand total percent
                ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write grand total actual sales
                    ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("=IF(({1}) < ({0}), -({0}), {0})", grandTotalActualFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                    // write grand total actual percent
                    ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell.Address, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // write grand total variance sales
                    ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // write grand total variance percent
                    ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            // init row index by payroll before tax
            int totalPayrollBeforeTaxRowIndex = 0;

            #region Write total payroll is not tax row
            if (dataRowByPayroll.Count > 0 && dataRowByPayrollIsTax.Count > 0)
            {
                // set current row index to row index by payroll before tax
                totalPayrollBeforeTaxRowIndex = startRow;

                // column index
                startColumn = 1;

                // TOTAL SALES
                ExcelRange totalSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSalesCell, false, "TOTAL BEFORE TAX", true, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // Write total row is all column
                foreach (var item in headerColumnIndexList)
                {
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                    }

                    // get target cell address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // total Budgeted projection sales
                    var salesFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByPayroll.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                    ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                    // total Budgeted priojection percent
                    ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                    if (isVarianceReport)
                    {
                        // total actual sales
                        var actualFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByPayroll.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                        ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                        // total actual percent
                        ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1}/{0})", totalSaledCell.Address, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                        // total variance sales
                        var varianceFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByPayroll.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                        ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalVarianceSalesCell, true, varianceFormula, true, FORMAT_CURRENCY, true);

                        // total variance percent
                        ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                    }
                }

                // Write grand total is total row
                ExcelRange totalProjectionSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalProjectionSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                //Total Budget percent
                ExcelRange totalPercentSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} < ({0}), -({0}), {0})", grandTotalActualFormula, totalProjectionSalesCell.Address), true, FORMAT_CURRENCY, true);

                    //total actual percent
                    ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalActualCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            #region Write payroll is tax row
            foreach (var itemDetail in dataRowByPayrollIsTax)
            {
                // add prime cost row index
                if (itemDetail.IsPrimeCost)
                {
                    dataRowIndexByIsPrimeCost.Add(startRow);
                }

                // reset start column index;
                startColumn = 1;

                // write category children name
                ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cellCategoryList, false, itemDetail.CategoryName, false, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // loop budget item by header column index list
                foreach (var item in headerColumnIndexList)
                {
                    // get budget item detail by index
                    var budgetItem = itemDetail.BudgetItemList[item.HeaderIndex];

                    // Write data row by category is all column
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                    }

                    // get target cell address (total payroll before tax if exists, set to total row index)
                    string targetCellAddress = sheet.Cells[(totalPayrollBeforeTaxRowIndex != 0 ? totalPayrollBeforeTaxRowIndex : 4), startColumn].Address;

                    // add  projectionsales and percent
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    if (budgetItem.IsPercentage)
                    {
                        this.AddValueFormatCell(salesCell, true, string.Format("=({0} * {1})", percentCell.Address, targetCellAddress), false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, false, budgetItem.ProjectionPercent / 100, false, FORMAT_PERCENT, true);
                    }
                    else
                    {
                        this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell.Address), false, FORMAT_PERCENT, true);
                    }

                    if (isVarianceReport)
                    {
                        // write actual sales
                        ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualSalesCell, true, string.Format("=IF({1} <= {0}, -{0}, {0})", budgetItem.ActualSales, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write actual percent
                        ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                        // write variance sales
                        ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write variance percent
                        ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                    }
                }

                // write grand total sales
                ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                // write grand total percent
                ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write grand total actual sales
                    ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("=IF(({1}) < ({0}), -({0}), {0})", grandTotalActualFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                    // write grand total actual percent
                    ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell.Address, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // write grand total variance sales
                    ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // write grand total variance percent
                    ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            // init row index by payroll is tax
            int totalPayrollIsTaxRowIndex = 0;

            #region Write total payroll is tax row
            if (dataRowByPayroll.Count > 0 && dataRowByPayrollIsTax.Count > 0)
            {
                // set current row index to row index by payroll before tax
                totalPayrollIsTaxRowIndex = startRow;

                // column index
                startColumn = 1;

                // TOTAL SALES
                ExcelRange totalSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSalesCell, false, "TOTAL", true, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // Write total row is all column
                foreach (var item in headerColumnIndexList)
                {
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                    }

                    // get target cell address
                    string targetCellAddress = sheet.Cells[totalPayrollBeforeTaxRowIndex, startColumn].Address;

                    // total Budgeted projection sales
                    var salesFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByPayrollIsTax.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                    ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                    // total Budgeted priojection percent
                    ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                    if (isVarianceReport)
                    {
                        // total actual sales
                        var actualFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByPayrollIsTax.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                        ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                        // total actual percent
                        ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1}/{0})", totalSaledCell.Address, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                        // total variance sales
                        var varianceFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByPayrollIsTax.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                        ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalVarianceSalesCell, true, varianceFormula, true, FORMAT_CURRENCY, true);

                        // total variance percent
                        ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                    }
                }

                // Write grand total is total row
                ExcelRange totalProjectionSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalProjectionSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                //Total Budget percent
                ExcelRange totalPercentSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} < ({0}), -({0}), {0})", grandTotalActualFormula, totalProjectionSalesCell.Address), true, FORMAT_CURRENCY, true);

                    //total actual percent
                    ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalActualCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            // init end row index on payroll section
            int endRowIndexOnPayrollSection = startRow - 1;

            #region Write TOTAL PAYROLL EXPENSES row
            // column index
            startColumn = 1;

            // TOTAL SALES
            ExcelRange totalPayrollCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalPayrollCell, false, "TOTAL PAYROLL EXPENSES", true, string.Empty, true);

            // grand total formula
            grandTotalSalesFormula = string.Empty;
            grandTotalActualFormula = string.Empty;
            grandTotalVarianceFormula = string.Empty;

            // Write total row is all column
            foreach (var item in headerColumnIndexList)
            {
                grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                if (isVarianceReport)
                {
                    grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                    grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                    grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                    grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 2].Address);
                }

                // get target cell address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // total Budgeted projection sales
                var salesFormula = string.Empty;
                if (dataRowByPayroll.Count > 0 && dataRowByPayrollIsTax.Count > 0)
                    salesFormula = string.Format("=({0}+{1})", sheet.Cells[totalPayrollBeforeTaxRowIndex, startColumn], sheet.Cells[totalPayrollIsTaxRowIndex, startColumn]);
                else
                    salesFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRowIndexOnPayrollSection, startColumn], sheet.Cells[endRowIndexOnPayrollSection, startColumn]);

                ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                // total Budgeted priojection percent
                ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    var actualFormula = string.Empty;
                    if (dataRowByPayroll.Count > 0 && dataRowByPayrollIsTax.Count > 0)
                        actualFormula = string.Format("=({0}+{1})", sheet.Cells[totalPayrollBeforeTaxRowIndex, startColumn], sheet.Cells[totalPayrollIsTaxRowIndex, startColumn]);
                    else
                        actualFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRowIndexOnPayrollSection, startColumn], sheet.Cells[endRowIndexOnPayrollSection, startColumn]);

                    ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                    // total actual percent
                    ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSalesCell, true, string.Format("={0}-{1}", totalActualSalesCell.Address, totalSaledCell.Address), true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }
            }

            // Write grand total is total row
            ExcelRange grandTotalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grandTotalSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

            //Total Budget percent
            ExcelRange grandTotalPercentCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(grandTotalPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalSalesCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // total actual sales
                ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} < ({0}), -({0}), {0})", grandTotalActualFormula, grandTotalSalesCell.Address), true, FORMAT_CURRENCY, true);

                //total actual percent
                ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesCell.Address, totalActualCell.Address), true, FORMAT_PERCENT, true);

                // total variance sales
                ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                // total variance percent
                ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalSalesCell.Address, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion
        }

        /// <summary>
        /// Method: Write data row by cogs section
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="dataRowBySales"></param>
        /// <param name="dataRowByCogs"></param>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        private void WriteDataRowByCogs(bool isVarianceReport, List<HeaderItemModel> headerColumnIndexList,
            List<BudgetItemModel> dataRowByCogs,
            ExcelWorksheet sheet, ref int startRow, IDictionary<int, int> dataRowIndexBySales)
        {
            // get row index by total
            var totalRowIndex = startRow + dataRowByCogs.Count;

            // init start column index is 1
            var startColumn = 1;

            // init column size in budget item
            var columnInBudgetItem = isVarianceReport ? 6 : 2;

            // write sales section header row
            ExcelRange cell = sheet.Cells[startRow, startColumn, startRow, headerColumnIndexList.Count * columnInBudgetItem + (1 + columnInBudgetItem)];
            cell.Merge = true;
            this.AddValueFormatCell(cell, false, BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT, true, string.Empty, false);

            // next row index
            startRow++;

            // target sales total formula
            string targetSalesFormula = string.Empty;
            for (int i = 0; i < headerColumnIndexList.Count; i++)
            {
                targetSalesFormula += targetSalesFormula.Length == 0 ? "" : "+";
                targetSalesFormula += sheet.Cells[4, (i * columnInBudgetItem) + 4].Address;
            }

            // grand total formula
            string grandTotalSalesFormula = string.Empty;
            string grandTotalActualFormula = string.Empty;
            string grandTotalVarianceFormula = string.Empty;

            #region Write cogs row
            foreach (var itemDetail in dataRowByCogs)
            {
                // reset start column index;
                startColumn = 1;

                // write category children name
                ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cellCategoryList, false, itemDetail.CategoryName, false, string.Empty, false);

                // get sales category mapping by category name
                var rowIndexBySales = 0;
                if (dataRowIndexBySales.ContainsKey(itemDetail.SalesCategoryRefId))
                    rowIndexBySales = dataRowIndexBySales[itemDetail.SalesCategoryRefId];

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // loop budget item by header column index list
                foreach (var item in headerColumnIndexList)
                {
                    // get budget item detail by index
                    var budgetItem = itemDetail.BudgetItemList[item.HeaderIndex];

                    // Write data row by category is all column
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += sheet.Cells[startRow, startColumn + 4].Address;
                    }

                    // get actual sales address by sales section
                    string actualSalesBySalesAddress = null;
                    if (rowIndexBySales > 0)
                    {
                        actualSalesBySalesAddress = sheet.Cells[rowIndexBySales, startColumn + 2].Address;
                    }

                    // add  projectionsales and percent
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    if (budgetItem.IsPercentage)
                    {
                        if (actualSalesBySalesAddress != null)
                            this.AddValueFormatCell(salesCell, true, string.Format("=({0} * {1})", percentCell.Address, actualSalesBySalesAddress), false, FORMAT_CURRENCY, true);
                        else
                            this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);

                        this.AddValueFormatCell(percentCell, false, budgetItem.ProjectionPercent / 100, false, FORMAT_PERCENT, true);
                    }
                    else
                    {
                        this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);
                        if (actualSalesBySalesAddress != null)
                            this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesBySalesAddress, salesCell.Address), false, FORMAT_PERCENT, true);
                        else
                            this.AddValueFormatCell(percentCell, false, budgetItem.ProjectionPercent / 100, false, FORMAT_PERCENT, true);
                    }

                    if (isVarianceReport)
                    {
                        // write actual sales
                        ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualSalesCell, false, budgetItem.ActualSales, false, FORMAT_CURRENCY, true);

                        // write actual percent
                        ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                        if (actualSalesBySalesAddress != null)
                            this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", actualSalesBySalesAddress, actualSalesCell.Address), false, FORMAT_PERCENT, true);
                        else
                            this.AddValueFormatCell(actualPercentCell, false, budgetItem.ActualPercent / 100, false, FORMAT_PERCENT, true);

                        // write variance sales
                        ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", salesCell.Address, actualSalesCell.Address), false, FORMAT_CURRENCY, true);

                        // write variance percent
                        ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                        if (actualSalesBySalesAddress != null)
                            this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", actualSalesBySalesAddress, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                        else
                            this.AddValueFormatCell(variancePercentCell, true, string.Format("={0}-{1}", percentCell.Address, actualPercentCell.Address), false, FORMAT_CURRENCY, true);
                    }
                }

                // get grand total actual sales address by section sales
                string grandTotallActualSalesAddress = null;
                if (rowIndexBySales > 0)
                {
                    grandTotallActualSalesAddress = sheet.Cells[rowIndexBySales, startColumn + 2].Address;
                }

                // write grand total sales
                ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                // write grand total percent
                ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
                if (grandTotallActualSalesAddress != null)
                    this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotallActualSalesAddress, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);
                else
                    this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write grand total actual sales
                    ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("=IF({1} < ({0}), -({0}), {0})", grandTotalActualFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                    // write grand total actual percent
                    ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    if (grandTotallActualSalesAddress != null)
                        this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotallActualSalesAddress, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);
                    else
                        this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // write grand total variance sales
                    ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // write grand total variance percent
                    ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    if (grandTotallActualSalesAddress != null)
                        this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotallActualSalesAddress, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                    else
                        this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("={0} - {1}", grandTotalBudgetedPercentCell.Address, grandTotalActualPercentCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            #region Write total cogs row
            // column index
            startColumn = 1;

            // TOTAL SALES
            ExcelRange totalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalSalesCell, false, "TOTAL COGS", true, string.Empty, true);

            // get sales category mapping by category name
            var rowIndexByTotalSales = dataRowIndexBySales.Count == 0 ? 0 : dataRowIndexBySales[0];

            // grand total formula
            grandTotalSalesFormula = string.Empty;
            grandTotalActualFormula = string.Empty;
            grandTotalVarianceFormula = string.Empty;

            // Write total row is all column
            foreach (var item in headerColumnIndexList)
            {
                grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                if (isVarianceReport)
                {
                    grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                    grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                    grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                    grandTotalVarianceFormula += sheet.Cells[startRow, startColumn + 4].Address;
                }

                // get cell total actual sales address by sales section
                string totalActualSalesBySalesAddress = null;
                if (rowIndexByTotalSales > 0)
                    totalActualSalesBySalesAddress = sheet.Cells[rowIndexByTotalSales, startColumn + 2].Address;
                else
                    totalActualSalesBySalesAddress = sheet.Cells[4, startColumn + 2].Address;

                // total Budgeted projection sales
                var salesFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByCogs.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                // total Budgeted priojection percent
                ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                if (totalActualSalesBySalesAddress != null)
                    this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalActualSalesBySalesAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);
                else
                    this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalActualSalesBySalesAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    var actualFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByCogs.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                    ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                    // total actual percent
                    ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    if (totalActualSalesBySalesAddress != null)
                        this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalActualSalesBySalesAddress, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);
                    else
                        this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalActualSalesBySalesAddress, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    var varianceFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowByCogs.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                    ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSalesCell, true, varianceFormula, true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                    if (totalActualSalesBySalesAddress != null)
                        this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalActualSalesBySalesAddress, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                    else
                        this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("={0} - {1}", totalPercentCell.Address, totalActualPercentCell.Address), true, FORMAT_PERCENT, true);
                }
            }

            // get cell total actual sales address by sales section
            string grandTotalActualSalesBySalesAddress = null;
            if (rowIndexByTotalSales > 0)
            {
                grandTotalActualSalesBySalesAddress = sheet.Cells[rowIndexByTotalSales, startColumn + 2].Address;
            }

            // Write grand total is total row
            ExcelRange totalProjectionSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalProjectionSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

            //Total Budget percent
            ExcelRange totalPercentSalesCell = sheet.Cells[startRow, startColumn++];
            if (grandTotalActualSalesBySalesAddress != null)
                this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalActualSalesBySalesAddress, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);
            else
                this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // total actual sales
                ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} < ({0}), -({0}), {0})", grandTotalActualFormula, totalProjectionSalesCell.Address), true, FORMAT_CURRENCY, true);

                //total actual percent
                ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                if (grandTotalActualSalesBySalesAddress != null)
                    this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalActualSalesBySalesAddress, totalActualCell.Address), true, FORMAT_PERCENT, true);
                else
                    this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalActualCell.Address), true, FORMAT_PERCENT, true);

                // total variance sales
                ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                // total variance percent
                ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                if (grandTotalActualSalesBySalesAddress != null)
                    this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalActualSalesBySalesAddress, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
                else
                    this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion
        }

        /// <summary>
        /// Method : Write data row by sales section
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="dataRowBySales"></param>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        private void WriteDataRowBySales(bool isVarianceReport, List<HeaderItemModel> headerColumnIndexList,
            List<BudgetItemModel> dataRowBySales,
            ExcelWorksheet sheet, ref int startRow, IDictionary<int, int> dataRowIndexBySales)
        {
            // get row index by total
            var totalRowIndex = startRow + dataRowBySales.Count;

            // init start column index is 1
            var startColumn = 1;

            // init column size in budget item
            var columnInBudgetItem = isVarianceReport ? 6 : 2;

            // write sales section header row
            ExcelRange cell = sheet.Cells[startRow, startColumn, startRow, headerColumnIndexList.Count * columnInBudgetItem + (1 + columnInBudgetItem)];
            cell.Merge = true;
            this.AddValueFormatCell(cell, false, BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT, true, string.Empty, false);

            // next row index
            startRow++;

            // target sales total formula
            string targetSalesFormula = string.Empty;
            for (int i = 0; i < headerColumnIndexList.Count; i++)
            {
                targetSalesFormula += targetSalesFormula.Length == 0 ? "" : "+";
                targetSalesFormula += sheet.Cells[4, (i * columnInBudgetItem) + 2].Address;
            }

            // grand total formula
            string grandTotalSalesFormula = string.Empty;
            string grandTotalActualFormula = string.Empty;
            string grandTotalVarianceFormula = string.Empty;

            #region Write sales row
            foreach (var itemDetail in dataRowBySales)
            {
                // add data row index by sales use mapping to COGS section
                dataRowIndexBySales.Add(itemDetail.CategorySettingId, startRow);

                // reset start column index;
                startColumn = 1;

                // write category children name
                ExcelRange cellCategoryList = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(cellCategoryList, false, itemDetail.CategoryName, false, string.Empty, false);

                // grand total formula
                grandTotalSalesFormula = string.Empty;
                grandTotalActualFormula = string.Empty;
                grandTotalVarianceFormula = string.Empty;

                // loop budget item by header column index list
                foreach (var item in headerColumnIndexList)
                {
                    // get budget item detail by index
                    var budgetItem = itemDetail.BudgetItemList[item.HeaderIndex];

                    // Write data row by category is all column
                    grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                    grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                    if (isVarianceReport)
                    {
                        grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                        grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                        grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                        grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                    }

                    // get target cell address
                    string targetCellAddress = sheet.Cells[4, startColumn].Address;

                    // add  projectionsales and percent
                    ExcelRange salesCell = sheet.Cells[startRow, startColumn++];
                    ExcelRange percentCell = sheet.Cells[startRow, startColumn++];
                    if (budgetItem.IsPercentage)
                    {
                        this.AddValueFormatCell(salesCell, true, string.Format("=({0} * {1})", percentCell.Address, targetCellAddress), false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, false, budgetItem.ProjectionPercent / 100, false, FORMAT_PERCENT, true);
                    }
                    else
                    {
                        this.AddValueFormatCell(salesCell, false, budgetItem.ProjectionSales, false, FORMAT_CURRENCY, true);
                        this.AddValueFormatCell(percentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, salesCell.Address), false, FORMAT_PERCENT, true);
                    }

                    if (isVarianceReport)
                    {
                        // total actual sales address
                        var totalActualCellAddress = sheet.Cells[4, startColumn].Address;

                        // write actual sales
                        ExcelRange actualSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualSalesCell, false, budgetItem.ActualSales, false, FORMAT_CURRENCY, true);

                        // write actual percent
                        ExcelRange actualPercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(actualPercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", totalActualCellAddress, actualSalesCell.Address), false, FORMAT_PERCENT, true);

                        // write variance sales
                        ExcelRange varianceSalesCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(varianceSalesCell, true, string.Format("={0}-{1}", actualSalesCell.Address, salesCell.Address), false, FORMAT_CURRENCY, true);

                        // write variance percent
                        ExcelRange variancePercentCell = sheet.Cells[startRow, startColumn++];
                        this.AddValueFormatCell(variancePercentCell, true, string.Format("=IF(ABS({0}) = 0, 0, {1} / ABS({0}))", salesCell.Address, varianceSalesCell.Address), false, FORMAT_PERCENT, true);
                    }
                }

                // write grand total sales
                ExcelRange grandTotalBudgetedSalesCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

                // write grand total percent
                ExcelRange grandTotalBudgetedPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(grandTotalBudgetedPercentCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // write grand total actual sales
                    ExcelRange grandTotalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualSalesCell, true, string.Format("=IF(({1})>({0}), -({0}), {0})", grandTotalActualFormula, grandTotalBudgetedSalesCell.Address), true, FORMAT_CURRENCY, true);

                    // write grand total actual percent
                    ExcelRange grandTotalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell.Address, grandTotalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // write grand total variance sales
                    ExcelRange grandTotalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVarianceSalesCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                    // write grand total variance percent
                    ExcelRange grandTotalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(grandTotalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", grandTotalBudgetedSalesCell, grandTotalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }

                // next row index
                startRow++;
            }
            #endregion

            #region Write total sales row
            // add data row index total sales use mapping all section
            dataRowIndexBySales.Add(0, startRow);

            // column index
            startColumn = 1;

            // TOTAL SALES
            ExcelRange totalSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalSalesCell, false, "TOTAL SALES", true, string.Empty, true);

            // grand total formula
            grandTotalSalesFormula = string.Empty;
            grandTotalActualFormula = string.Empty;
            grandTotalVarianceFormula = string.Empty;

            // Write total row is all column
            foreach (var item in headerColumnIndexList)
            {
                grandTotalSalesFormula += grandTotalSalesFormula.Length == 0 ? "" : "+";
                grandTotalSalesFormula += sheet.Cells[startRow, startColumn].Address;
                if (isVarianceReport)
                {
                    grandTotalActualFormula += grandTotalActualFormula.Length == 0 ? "" : "+";
                    grandTotalActualFormula += string.Format("ABS({0})", sheet.Cells[startRow, startColumn + 2].Address);
                    grandTotalVarianceFormula += grandTotalVarianceFormula.Length == 0 ? "" : "+";
                    grandTotalVarianceFormula += string.Format("{0}", sheet.Cells[startRow, startColumn + 4].Address);
                }

                // get target cell address
                string targetCellAddress = sheet.Cells[4, startColumn].Address;

                // total Budgeted projection sales
                var salesFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowBySales.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                ExcelRange totalSaledCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalSaledCell, true, salesFormula, true, FORMAT_CURRENCY, true);

                // total Budgeted priojection percent
                ExcelRange totalPercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", targetCellAddress, totalSaledCell.Address), true, FORMAT_PERCENT, true);

                if (isVarianceReport)
                {
                    // total actual sales
                    var actualFormula = string.Format("=SUM({0}:{1})", sheet.Cells[startRow - dataRowBySales.Count, startColumn], sheet.Cells[startRow - 1, startColumn]);
                    ExcelRange totalActualSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualSalesCell, true, actualFormula, true, FORMAT_CURRENCY, true);

                    // total actual percent
                    ExcelRange totalActualPercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalActualPercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalActualSalesCell.Address), true, FORMAT_PERCENT, true);

                    // total variance sales
                    ExcelRange totalVarianceSalesCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarianceSalesCell, true, string.Format("={0}-{1}", totalActualSalesCell.Address, totalSaledCell.Address), true, FORMAT_CURRENCY, true);

                    // total variance percent
                    ExcelRange totalVarancePercentCell = sheet.Cells[startRow, startColumn++];
                    this.AddValueFormatCell(totalVarancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalSaledCell.Address, totalVarianceSalesCell.Address), true, FORMAT_PERCENT, true);
                }
            }

            // Write grand total is total row
            ExcelRange totalProjectionSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalProjectionSalesCell, true, string.Format("={0}", grandTotalSalesFormula), true, FORMAT_CURRENCY, true);

            //Total Budget percent
            ExcelRange totalPercentSalesCell = sheet.Cells[startRow, startColumn++];
            this.AddValueFormatCell(totalPercentSalesCell, true, string.Format("=IF(({0}) = 0, 0, {1} / ({0}))", targetSalesFormula, totalProjectionSalesCell.Address), true, FORMAT_PERCENT, true);

            if (isVarianceReport)
            {
                // total actual sales
                ExcelRange totalActualCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualCell, true, string.Format("=IF({1} > ({0}), -({0}), {0})", grandTotalActualFormula, totalProjectionSalesCell.Address), true, FORMAT_CURRENCY, true);

                //total actual percent
                ExcelRange totalActualPCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalActualPCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalActualCell.Address), true, FORMAT_PERCENT, true);

                // total variance sales
                ExcelRange totalVarianceSaleCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVarianceSaleCell, true, string.Format("={0}", grandTotalVarianceFormula), true, FORMAT_CURRENCY, true);

                // total variance percent
                ExcelRange totalVariancePercentCell = sheet.Cells[startRow, startColumn++];
                this.AddValueFormatCell(totalVariancePercentCell, true, string.Format("=IF({0} = 0, 0, {1} / {0})", totalProjectionSalesCell.Address, totalVarianceSaleCell.Address), true, FORMAT_PERCENT, true);
            }

            #endregion
        }

        /// <summary>
        /// Method: add header to excel file
        /// </summary>
        /// <param name="isVarianceReport"></param>
        /// <param name="inputMethod"></param>
        /// <param name="headerColumnIndexList"></param>
        /// <param name="budgetTab"></param>
        /// <param name="sheet"></param>
        private void AddHeaderExcel(bool isVarianceReport, int inputMethod, List<HeaderItemModel> headerColumnIndexList, BudgetTabModel budgetTab, ExcelWorksheet sheet)
        {
            // remove text "Period budget" in budet type is Period get year in tab name
            string year = string.Empty;
            DateTime startDateByMonth = new DateTime();
            if (budgetTab.TabName.IndexOf("Period budget") != -1)
            {
                year = budgetTab.TabName.Split('-')[1].Trim();
            }
            else if (budgetTab.TabName.IndexOf("-") != -1)
            {
                var month = DateTime.ParseExact(budgetTab.HeaderColumnList[0], "MMMM", CultureInfo.CurrentCulture).Month;
                startDateByMonth = new DateTime(int.Parse(budgetTab.TabName.Split('-')[0].Trim()), month, 1);
            }

            #region HeaderName Excel
            // Set Column Width
            sheet.Column(1).Width = 30;
            sheet.Column(2).Width = 18;

            // write annual sales
            this.AddValueFormatCell(sheet.Cells[1, 1], false, "Annual Sales", true, string.Empty, false);
            var cellAnnualSales = sheet.Cells[1, 2];
            this.AddValueFormatCell(cellAnnualSales, false, budgetTab.AnnualSales, false, FORMAT_CURRENCY, false);
            cellAnnualSales.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;

            // write header row
            var columnIndex = 2;
            foreach (var item in headerColumnIndexList)
            {
                ExcelRange cell = sheet.Cells[3, columnIndex, 3, columnIndex + 1 + (isVarianceReport ? 4 : 0)];
                cell.Merge = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                this.AddValueFormatCell(cell, false, item.HeaderName, true, string.Empty, false);
                columnIndex += 2;

                if (isVarianceReport)
                {
                    columnIndex += 4;
                }
            }

            // write target row
            columnIndex = 2;
            foreach (var item in headerColumnIndexList)
            {
                // write target item
                ExcelRange targetProjectSalesItemCell = sheet.Cells[4, columnIndex, 4, columnIndex + 1];
                ExcelRange targetPercentSalesItemCell = sheet.Cells[5, columnIndex, 5, columnIndex + 1];
                if (inputMethod == BCSCommonData.INPUT_METHOD_DOLLAR)
                {
                    // target sales
                    targetProjectSalesItemCell.Merge = true;
                    targetProjectSalesItemCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    this.AddValueFormatCell(targetProjectSalesItemCell, false, budgetTab.TargetColumnList[item.HeaderIndex].TargetSales, false, FORMAT_CURRENCY, false);

                    // target percent
                    targetPercentSalesItemCell.Merge = true;
                    targetPercentSalesItemCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    this.AddValueFormatCell(targetPercentSalesItemCell, true, string.Format("=IF({0} = 0, 0, {1}/{0})", cellAnnualSales.Address, targetProjectSalesItemCell.Start.Address), false, FORMAT_PERCENT, false);

                    if (isVarianceReport)
                    {
                        ExcelRange actual = sheet.Cells[4, columnIndex + 2, 4, columnIndex + 3];
                        ExcelRange actualPercent = sheet.Cells[5, columnIndex + 2, 5, columnIndex + 3];
                        ExcelRange variance = sheet.Cells[4, columnIndex + 4, 4, columnIndex + 5];
                        ExcelRange variancePercent = sheet.Cells[5, columnIndex + 4, 5, columnIndex + 5];

                        // Actual sales
                        actual.Merge = true;
                        actual.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(actual, false, budgetTab.SalesTotal[item.HeaderIndex].ActualSales, false, FORMAT_CURRENCY, false);

                        // Actual percent
                        actualPercent.Merge = true;
                        actualPercent.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(actualPercent, true, string.Format("=IF({0} = 0, 0, {1}/{0})", targetProjectSalesItemCell.Start.Address, actual.Start.Address), false, FORMAT_PERCENT, false);

                        // Variance sales
                        variance.Merge = true;
                        variance.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(variance, false, budgetTab.SalesTotal[item.HeaderIndex].VarianceSales, false, FORMAT_CURRENCY, false);

                        // Variance percent
                        variancePercent.Merge = true;
                        variancePercent.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(variancePercent, true, string.Format("=IF({0} = 0, 0, {1}/{0})", targetProjectSalesItemCell.Start.Address, variance.Start.Address), false, FORMAT_PERCENT, false);
                    }
                }
                else
                {
                    // target sales
                    targetProjectSalesItemCell.Merge = true;
                    targetProjectSalesItemCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    this.AddValueFormatCell(targetProjectSalesItemCell, true, string.Format("=({0} * {1})", cellAnnualSales.Address, targetPercentSalesItemCell.Start.Address), false, FORMAT_CURRENCY, false);

                    // target percent
                    targetPercentSalesItemCell.Merge = true;
                    targetPercentSalesItemCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    this.AddValueFormatCell(targetPercentSalesItemCell, false, (budgetTab.TargetColumnList[item.HeaderIndex].TargetPercent / 100), false, FORMAT_PERCENT, false);

                    if (isVarianceReport)
                    {
                        ExcelRange actual = sheet.Cells[4, columnIndex + 2, 4, columnIndex + 3];
                        ExcelRange actualPercent = sheet.Cells[5, columnIndex + 2, 5, columnIndex + 3];
                        ExcelRange variance = sheet.Cells[4, columnIndex + 4, 4, columnIndex + 5];
                        ExcelRange variancePercent = sheet.Cells[5, columnIndex + 4, 5, columnIndex + 5];

                        // Actual sales
                        actual.Merge = true;
                        actual.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(actual, true, string.Format("=({0} * {1})", targetProjectSalesItemCell.Start.Address, actualPercent.Start.Address), false, FORMAT_CURRENCY, false);

                        // Actual percent
                        actualPercent.Merge = true;
                        actualPercent.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(actualPercent, false, budgetTab.SalesTotal[item.HeaderIndex].ActualPercent / 100, false, FORMAT_PERCENT, false);

                        // Variance sales
                        variance.Merge = true;
                        variance.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(variance, true, string.Format("=({0} * {1})", targetProjectSalesItemCell.Start.Address, variancePercent.Start.Address), false, FORMAT_CURRENCY, false);
                        
                        // Variance percent
                        variancePercent.Merge = true;
                        variancePercent.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        this.AddValueFormatCell(variancePercent, false, budgetTab.SalesTotal[item.HeaderIndex].VariancePercent / 100, false, FORMAT_PERCENT, false);
                    }
                }

                //Budgeted
                ExcelRange budgeted = sheet.Cells[6, columnIndex, 6, columnIndex + 1];
                budgeted.Merge = true;
                budgeted.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                var budgetedName = "Budgeted";
                this.AddValueFormatCell(budgeted, false, budgetedName, true, string.Empty, false);

                if (isVarianceReport)
                {
                    // Actual
                    ExcelRange actualName = sheet.Cells[6, columnIndex + 2, 6, columnIndex + 3];
                    actualName.Merge = true;
                    actualName.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    actualName.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    this.AddValueFormatCell(actualName, false, "Actual", true, string.Empty, false);

                    // Variance
                    ExcelRange varianceName = sheet.Cells[6, columnIndex + 4, 6, columnIndex + 5];
                    varianceName.Merge = true;
                    varianceName.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    varianceName.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    this.AddValueFormatCell(varianceName, false, "Variance", true, string.Empty, false);

                    columnIndex += 4;
                }

                columnIndex += 2;
            }

            // write Grand Total
            ExcelRange grandTotal = sheet.Cells[3, columnIndex, 3, columnIndex + 1 + (isVarianceReport ? 4 : 0)];
            grandTotal.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            grandTotal.Merge = true;
            var grandTotalName = "Grand Total";
            this.AddValueFormatCell(grandTotal, false, grandTotalName, true, string.Empty, false);

            // Budgeted
            ExcelRange grandbudgeted = sheet.Cells[4, columnIndex, 6, columnIndex + 1];
            grandbudgeted.Merge = true;
            grandbudgeted.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            grandbudgeted.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            var grandbudgetedName = "Budgeted";
            this.AddValueFormatCell(grandbudgeted, false, grandbudgetedName, true, string.Empty, false);

            if (isVarianceReport)
            {
                // Actual
                ExcelRange grandactual = sheet.Cells[4, columnIndex + 2, 6, columnIndex + 3];
                grandactual.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                grandactual.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                grandactual.Merge = true;
                var grandactualName = "Actual";
                this.AddValueFormatCell(grandactual, false, grandactualName, true, string.Empty, false);

                // Variance
                ExcelRange grandvariance = sheet.Cells[4, columnIndex + 4, 6, columnIndex + 5];
                grandvariance.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                grandvariance.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                grandvariance.Merge = true;
                var grandvarianceName = "Variance";
                this.AddValueFormatCell(grandvariance, false, grandvarianceName, true, string.Empty, false);
            }

            #endregion
        }

        /// <summary>
        /// Method: write data/formula value to cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="isFormula"></param>
        /// <param name="value"></param>
        /// <param name="isBold"></param>
        /// <param name="format"></param>
        /// <param name="textAlignRight"></param>
        private void AddValueFormatCell(ExcelRange cell, bool isFormula, object value, bool isBold, string format, bool textAlignRight)
        {
            if (isFormula)
                cell.Formula = value.ToString();
            else
            {
                cell.Value = value;
                if (value.GetType().Name != "String")
                {
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
            }

            cell.Style.Numberformat.Format = format;
            cell.Style.Font.Bold = isBold;
            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            if (textAlignRight)
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }

        /// <summary>
        /// Method: Get all data in tab by budget id and tab id
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="budgetTabId"></param>
        /// <returns></returns>
        private BudgetTabModel GetDataByBudgetTabId(int budgetId, int budgetTabId)
        {
            // get all budget tab by budget id
            var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budgetId, CurrentUser.UserId).FirstOrDefault(s => s.BudgetTabId == budgetTabId);

            // convert budget tab to budget tab model
            Mapper.CreateMap<BudgetTab, BudgetTabModel>();
            var budgetTabModel = Mapper.Map<BudgetTab, BudgetTabModel>(budgetTabList);

            // add default value to budget item
            StringBuilder budgetItemRow = new StringBuilder();
            for (int i = 0; i < budgetTabModel.HeaderColumnList.Count; i++)
            {
                budgetItemRow.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, 0, 0, 0, 0, 0);
            }

            // get data item by budget tab
            var dataItemByBudgetTab = SingletonIpl.GetInstance<BudgetItemBll>().GetBudgetItemByBudgetTabId(budgetTabModel.BudgetId, budgetTabModel.BudgetTabId, budgetItemRow.ToString(), CurrentUser.UserId);
            Mapper.CreateMap<BudgetItem, BudgetItemModel>();
            budgetTabModel.BudgetItemModelList = Mapper.Map<List<BudgetItem>, List<BudgetItemModel>>(dataItemByBudgetTab.ToList());
            return budgetTabModel;
        }

        #endregion

        #region SYNC DATA FROM SSP

        /// <summary>
        /// call service for category
        /// </summary>
        /// <returns></returns>
        public ActionResult SyncBudgetCategorySettings(int? id)
        {
            bool status = false;
            string message = string.Empty;

            try
            {
                // check parament input
                if (id == null || id == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                // get budget by id
                var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
                if (budget == null || budget.BudgetId == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                // 1. get rest code default and token id by user id
                var restCodebyUserId = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).FirstOrDefault(s => s.RestCode == budget.RestCode);

                // 2. Call service get category by rest code
                var result = BCS.Web.WebUtils.BCSWebUtils.GetAllCategory(restCodebyUserId.TokenId, budget.RestCode);
                if (result.Content == "204" && result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    message = "Your Default Restaurant is not activated. Please go to SSP System to active Restaurant and try again.";
                    return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
                }
                // check result for service
                var CategoryListSSP = JsonConvert.DeserializeObject<List<SSPCategory>>(result.Content);
                if (CategoryListSSP == null)
                {
                    message = "The user is not authorized to make the request.";
                    return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
                }

                // check result empty
                if (CategoryListSSP.Count == 0)
                {
                    message = "Your Default Restaurant doesn't have any Categories Setting. Please select another Default Restaurant and try again.";
                    return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
                }
                // 3. Get category setting by budget id
                var categorySettingByBudgetId = SingletonIpl.GetInstance<CategorySettingBll>().GetCategorySettingByBudgetId(id.Value, CurrentUser.UserId).ToList();

                // 4. Get parent sales category setting by budget id
                var salesParentCategory = categorySettingByBudgetId.FirstOrDefault(s => s.ParentCategoryId == 0 && s.BudgetId == id.Value && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT);
                var salesChildrenCategory = categorySettingByBudgetId.Where(s => s.ParentCategoryId == salesParentCategory.CategorySettingId).OrderBy(s => s.SortOrder).ToList<CategorySetting>();

                // 5. Get parent sales category setting by budget id
                var cogsParentCategory = categorySettingByBudgetId.FirstOrDefault(s => s.ParentCategoryId == 0 && s.BudgetId == id.Value && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT);
                var cogsChildrenCategory = categorySettingByBudgetId.Where(s => s.ParentCategoryId == cogsParentCategory.CategorySettingId).OrderBy(s => s.SortOrder).ToList<CategorySetting>();
                
                // delelte all category children by sales & cogs section
                SingletonIpl.GetInstance<CategorySettingBll>().DeleteByParentCategoryId(budget.BudgetId, salesParentCategory.CategorySettingId, CurrentUser.UserId);
                SingletonIpl.GetInstance<CategorySettingBll>().DeleteByParentCategoryId(budget.BudgetId, cogsParentCategory.CategorySettingId, CurrentUser.UserId);

                var sortOrder = 1;
                foreach (var item in CategoryListSSP)
                {
                    // update categoryName from SSP.
                    CategorySetting categorySetting = new CategorySetting()
                    {
                        BudgetId = id.Value,
                        ParentCategoryId = salesParentCategory.CategorySettingId,
                        CategoryName = item.CategoryName,
                        SortOrder = sortOrder++,
                        IsSelected = false,
                        IsPrimeCost = false,
                        IsPercentage = true,
                        IsTaxCost = false,
                        DeletedFlg = false,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        CreatedUserId = CurrentUser.UserId,
                        UpdatedUserId = CurrentUser.UserId
                    };

                    // change entity insert sales parent
                    categorySetting.ParentCategoryId = salesParentCategory.CategorySettingId;

                    // insert category to sales section
                    var salesCategoryRefId = SingletonIpl.GetInstance<CategorySettingBll>().Save(categorySetting, CurrentUser.UserId);

                    // change entity insert cogs parent
                    categorySetting.ParentCategoryId = cogsParentCategory.CategorySettingId;
                    categorySetting.SalesCategoryRefId = salesCategoryRefId;

                    // set default is prime cost
                    categorySetting.IsPrimeCost = true;

                    // insert category to cogs section
                    SingletonIpl.GetInstance<CategorySettingBll>().Save(categorySetting, CurrentUser.UserId);
                }

                status = true;
                message = "Sync Budget Categories from SSP successfully.";
            }
            catch
            {
                message = "Sync Budget Categories from SSP unsuccessfully.";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// call service sync data sales and cogs from ssp
        /// </summary>
        /// <returns></returns>
        public ActionResult SyncBudgetActualFromSSP(int? id, List<BudgetTabHeaderModel> headerOjectList, int budgetType)
        {
            bool status = false;
            string message = string.Empty;
            StringBuilder sbmessage = new StringBuilder();
            try
            {
                // 1. Check parament input
                if (id == null || id == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                // 2 .Get budget by id
                var budget = SingletonIpl.GetInstance<BudgetBll>().Get(id.Value);
                if (budget == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // 3. Call Sync actual number from ssp.
                status = this.SyncSalesActualNumberFromSSP(budget, headerOjectList, budgetType);
                message = status ? "Import Actual number from SSP successfully." : "Import Actual number from SSP unsuccessfully.";
            }
            catch
            {
                message = "Error: Import Actual number, please contact to admin system.";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method call service sync actual data
        /// </summary>
        /// <param name="budget"></param>
        /// <param name="headerOjectList"></param>
        /// <param name="budgetType"></param>
        /// <returns></returns>
        private bool SyncSalesActualNumberFromSSP(Budget budget, List<BudgetTabHeaderModel> headerOjectList, int budgetType)
        {
            try
            {
                // 1. Get all budget tab by budget id
                var budgetTabList = SingletonIpl.GetInstance<BudgetTabBll>().GetBudgetTabByBudgetId(budget.BudgetId, CurrentUser.UserId);
                Mapper.CreateMap<BudgetTab, BudgetTabModel>();
                var budgetTabModelList = Mapper.Map<List<BudgetTab>, List<BudgetTabModel>>(budgetTabList.ToList());

                // 1.1. get token id by user id and rest code in budget
                var tokenId = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).FirstOrDefault(s => s.RestCode == budget.RestCode).TokenId;

                // 1.2. start year by budget
                DateTime budgetStartDate = budget.BudgetLengthStart.Value;

                // 1.3. budget type
                bool periodicType = (budgetType == BCSCommonData.BUDGET_TYPE_PRERIODIC) ? true : false;

                // 1.4. week range
                int weekRange = budget.BudgetLength == BCSCommonData.BUDGET_PRERIODIC_LENGTH_FIFTYTOW_PERIODIC ? 1 : 4;

                // 2. get budget item row by budget tab
                foreach (var budgetTabModel in budgetTabModelList)
                {
                    // header count on tab
                    int headerCount = budgetTabModel.HeaderColumnList.Count;

                    // 2.1. check budget tab exists header checked list
                    if (headerOjectList.Any(s => s.TabId == budgetTabModel.BudgetTabId))
                    {
                        // 2.2. add default value to budget item
                        StringBuilder budgetItemRow = new StringBuilder();
                        for (int i = 0; i < headerCount; i++)
                        {
                            budgetItemRow.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, 0, 0, 0, 0, 0);
                        }

                        // 2.5. Get budget item (data row by category) by budget tab id
                        var dataItemByBudgetTab = SingletonIpl.GetInstance<BudgetItemBll>().GetBudgetItemByBudgetTabId(budgetTabModel.BudgetId, budgetTabModel.BudgetTabId, budgetItemRow.ToString(), CurrentUser.UserId);

                        // 2.6. Convert budget item list to budget item model list
                        Mapper.CreateMap<BudgetItem, BudgetItemModel>();
                        budgetTabModel.BudgetItemModelList = Mapper.Map<List<BudgetItem>, List<BudgetItemModel>>(dataItemByBudgetTab.ToList());

                        // 2.7. Loop all header index choise from screen by tab
                        var currentTabOnScreen = headerOjectList.FirstOrDefault(s => s.TabId == budgetTabModel.BudgetTabId);
                        foreach (var headerIndex in currentTabOnScreen.HeaderIndex)
                        {
                            // 2.7.1. Call web service get actual sales & cogs from ssp
                            var resultDataActualFromSSP = BCS.Web.WebUtils.BCSWebUtils.GetDataActualFromSSP(tokenId, budget.RestCode, budgetStartDate, headerIndex, weekRange, periodicType);
                            var actualSyncList = JsonConvert.DeserializeObject<List<BudgetActual>>(resultDataActualFromSSP.Content);

                            // 2.7.2. Check exists result
                            if (actualSyncList != null && actualSyncList.Count > 0)
                            {
                                // call update data actual
                                this.UpdateDataActualByHeaderIndex(headerIndex, BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT, budgetTabModel, actualSyncList);

                                // call update data actual
                                this.UpdateDataActualByHeaderIndex(headerIndex, BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT, budgetTabModel, actualSyncList);
                            }
                        }
                    }

                    // 2.8. set budget start date on next tab
                    if (periodicType)
                    {
                        // 2.8.1. next year to budget start date
                        budgetStartDate = budgetStartDate.AddYears(1);
                    }
                    else
                    {
                        // 2.8.2. next month by header count to budget start date of budget type is month
                        budgetStartDate = budgetStartDate.AddMonths(headerCount);
                    }
                }

                // return flag after sync
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Method loop actual sync data call save data actual by header index
        /// </summary>
        /// <param name="headerIndex"></param>
        /// <param name="parentCategoryName"></param>
        /// <param name="budgetTabModel"></param>
        /// <param name="actualSyncList"></param>
        private void UpdateDataActualByHeaderIndex(int headerIndex, string parentCategoryName, BudgetTabModel budgetTabModel, List<BudgetActual> actualSyncList)
        {
            // 5. Loop all data result
            foreach (var item in actualSyncList)
            {
                // sync data sales from SSP
                var budgetItemModel = budgetTabModel.BudgetItemModelList.FirstOrDefault(p => p.CategoryName == item.CategoryName && p.ParentCategoryName == parentCategoryName);
                if (budgetItemModel != null)
                {
                    // set mearge data to item Actual Sales by index
                    var budgetItemList = budgetItemModel.BudgetItemList;
                    budgetItemList[headerIndex].ActualSales = (parentCategoryName == BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT ? item.ActualSales : item.ActualCogs);

                    // reset data list to model
                    budgetItemModel.BudgetItemList = budgetItemList;

                    // call method get budget item by id
                    var budgetItemUpdate = SingletonIpl.GetInstance<BudgetItemBll>().Get(budgetItemModel.BudgetItemId);

                    // set data update
                    budgetItemUpdate.BudgetItemRow = budgetItemModel.BudgetItemRow;
                    budgetItemUpdate.UpdatedDate = DateTime.Now;
                    budgetItemUpdate.UpdatedUserId = CurrentUser.UserId;

                    // call method save update budget item
                    SingletonIpl.GetInstance<BudgetItemBll>().Save(budgetItemUpdate, CurrentUser.UserId);
                }
            }
        }

        #endregion
    }
}
