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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace BCS.Web.Controllers
{
    public class CategoriesSettingController : BaseController
    {
        // GET: CategoriesSetting
        public ActionResult Index()
        {
            // get category by user
            var result = SingletonIpl.GetInstance<CategoryBll>().GetCategoryByUserId(CurrentUser.UserId).ToList();
            Mapper.CreateMap<Category, CategoryModel>();
            ViewBag.CategoriesList = Mapper.Map<List<Category>, List<CategoryModel>>(result);
            // get rest name rest code and token id by user id
            var restCodebyUserId = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).FirstOrDefault(s => s.IsDefault == true);
            if (restCodebyUserId != null)
            {
                ViewBag.RestCode = restCodebyUserId.RestCode;
                ViewBag.RestName = restCodebyUserId.RestName;
            }

            return View();
        }

        public ActionResult ViewEditCategories(int id)
        {
            // Get categories by id
            var result = SingletonIpl.GetInstance<CategoryBll>().Get(id);
            Mapper.CreateMap<Category, CategoryModel>();
            CategoryModel model = Mapper.Map<Category, CategoryModel>(result);

            return PartialView("_ViewEditCategories", model);
        }

        public ActionResult SaveCategories(List<CategoryModel> listObj, string parentCategoryName)
        {
            Mapper.CreateMap<CategoryModel, Category>();
            var categoryList = Mapper.Map<List<CategoryModel>, List<Category>>(listObj);

            // check parent category name is "Sales/COGS", call update two section
            int salesParentCategoryId = 0, cogsParentCategoryId = 0;
            IList<Category> salesCategoryList = null, cogsCategoryList = null;
            if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName || BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT == parentCategoryName)
            {
                // get all category on parent category is "Sales"
                salesCategoryList = SingletonIpl.GetInstance<CategoryBll>().GetCategoryByParentName(BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT, CurrentUser.UserId);
                salesParentCategoryId = salesCategoryList.FirstOrDefault(p => p.ParentCategoryId == 0).CategoryId;

                // get all category on parent category is "COGS"
                cogsCategoryList = SingletonIpl.GetInstance<CategoryBll>().GetCategoryByParentName(BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT, CurrentUser.UserId);
                cogsParentCategoryId = cogsCategoryList.FirstOrDefault(p => p.ParentCategoryId == 0).CategoryId;
            }

            List<int> resultList = new List<int>();
            foreach (var obj in categoryList)
            {
                // call update reference category Sales and COGS
                if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName || BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT == parentCategoryName)
                {
                    if (obj.CategoryId == 0)
                    {
                        // add new category
                        Category categoryReference = new Category();
                        Mapper.CreateMap<Category, Category>();
                        Mapper.Map<Category, Category>(obj, categoryReference);
                        categoryReference.IsPercentage = true;

                        // case insert new reference COGS
                        if (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName)
                        {
                            categoryReference.ParentCategoryId = cogsParentCategoryId;
                            categoryReference.IsPrimeCost = true;
                        }
                        else
                        {
                            categoryReference.ParentCategoryId = salesParentCategoryId;
                        }

                        // call method save category reference
                        SingletonIpl.GetInstance<CategoryBll>().Save(categoryReference, CurrentUser.UserId);
                    }
                    else
                    {
                        // get category name old
                        var categoryNameOld = SingletonIpl.GetInstance<CategoryBll>().Get(obj.CategoryId).CategoryName;

                        // update category
                        var categoryReference = (BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT == parentCategoryName ? cogsCategoryList : salesCategoryList).FirstOrDefault(p => p.CategoryName == categoryNameOld);
                        if (categoryReference != null)
                        {
                            categoryReference.CategoryName = obj.CategoryName;
                            categoryReference.SortOrder = obj.SortOrder;
                            categoryReference.DeletedFlg = obj.DeletedFlg;
                            categoryReference.UpdatedDate = DateTime.Now;
                            categoryReference.UpdatedUserId = CurrentUser.UserId;

                            // call method save category reference
                            SingletonIpl.GetInstance<CategoryBll>().Save(categoryReference, CurrentUser.UserId);
                        }
                    }
                }

                // call method save category
                obj.UpdatedDate = DateTime.Now;
                obj.UpdatedUserId = CurrentUser.UserId;
                var result = SingletonIpl.GetInstance<CategoryBll>().Save(obj, CurrentUser.UserId);

                if (result > 0)
                    resultList.Add(result);
            }

            return Json(new { Status = (resultList.Count == categoryList.Count) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategoriesByParentId(int id, bool isTaxFlag)
        {
            // Get childen categories by parent category id
            var result = SingletonIpl.GetInstance<CategoryBll>().GetCategoryByParentId(CurrentUser.UserId, id).Where(s => s.IsTaxCost == isTaxFlag).ToList();
            Mapper.CreateMap<Category, CategoryModel>();
            var data = Mapper.Map<List<Category>, List<CategoryModel>>(result);

            return Json(new DataSourceResult { Data = data, Total = data.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RestoreCategories()
        {
            // call method delete all categories default by user
            SingletonIpl.GetInstance<CategoryBll>().DeleteAllCategoryByUser(CurrentUser.UserId);

            return Json(new { Status = true }, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>
        /// call service sync data for category setting from ssp
        /// </summary>
        /// <returns></returns>
        public ActionResult syncCategorySettings()
        {
            bool status = false;
            string message = string.Empty;

            try
            {
                // 1. get rest code default and token id by user id
                var restCodebyUserId = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).FirstOrDefault(s => s.IsDefault == true);

                // 2. Call service get category by rest code
                var result = BCS.Web.WebUtils.BCSWebUtils.GetAllCategory(restCodebyUserId.TokenId, restCodebyUserId.RestCode);
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

                // get all category from DB
                var categoryList = SingletonIpl.GetInstance<CategoryBll>().GetCategoryByUserId(CurrentUser.UserId).ToList();

                // get parent sales category
                var salesParentCategory = categoryList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT);
                var salesChildrenCategory = categoryList.Where(s => s.ParentCategoryId == salesParentCategory.CategoryId).OrderBy(s => s.SortOrder).ToList<Category>();

                // get parent cogs category : use parent id
                var cogsParentCategory = categoryList.FirstOrDefault(s => s.ParentCategoryId == 0 && s.CategoryName == BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT);
                var cogsChildrenCategory = categoryList.Where(s => s.ParentCategoryId == cogsParentCategory.CategoryId).OrderBy(s => s.SortOrder).ToList<Category>();
                
                // delelte all category children by sales & cogs section
                SingletonIpl.GetInstance<CategoryBll>().DeleteAllCategoryChildrenByParentCategoryId(salesParentCategory.CategoryId, CurrentUser.UserId);
                SingletonIpl.GetInstance<CategoryBll>().DeleteAllCategoryChildrenByParentCategoryId(cogsParentCategory.CategoryId, CurrentUser.UserId);

                var sortOrder = 1;
                foreach (var item in CategoryListSSP)
                {
                    // update categoryName from SSP.
                    Category category = new Category()
                    {
                        ParentCategoryId = salesParentCategory.CategoryId,
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
                    category.ParentCategoryId = salesParentCategory.CategoryId;

                    // insert category to sales section
                    SingletonIpl.GetInstance<CategoryBll>().Save(category, CurrentUser.UserId);

                    // change entity insert cogs parent
                    category.ParentCategoryId = cogsParentCategory.CategoryId;
                    category.IsPrimeCost = true;

                    // insert category to cogs section
                    SingletonIpl.GetInstance<CategoryBll>().Save(category, CurrentUser.UserId);
                }

                // set flag after sync category
                status = true;
                message = "Sync Categories from SSP successfully.";
            }
            catch
            {
                message = "Sync Categories from SSP unsuccessfully.";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
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
        /// Method: save all category by template
        /// </summary>
        /// <param name="listObj"></param>
        /// <returns></returns>
        public ActionResult SaveCategorySettingByTemplate(List<SectionModel> sectionList)
        {
            bool status = false;
            string message = string.Empty;

            try
            {
                // create instance category bll
                var categoryBll = SingletonIpl.GetInstance<CategoryBll>();

                // 1. Loop all section
                foreach (var section in sectionList)
                {
                    bool isTaxCost = section.SectionName.Contains("(Tax)");

                    // 1.1. Get parent category id by section name and user id
                    string parentCategoryName = isTaxCost ? section.SectionName.Replace(" (Tax)", "") : section.SectionName;
                    var parentCategory = categoryBll.GetCategoryByParentName(parentCategoryName, CurrentUser.UserId).FirstOrDefault(s => s.ParentCategoryId == 0);

                    // 1.2. Delete all children category by parent category id
                    if (!isTaxCost)
                    {
                        categoryBll.DeleteAllCategoryChildrenByParentCategoryId(parentCategory.CategoryId, CurrentUser.UserId);
                    }

                    // 1.3. Loop all category from template
                    if (section.CategoryBySection != null)
                    {
                        foreach (var category in section.CategoryBySection)
                        {
                            Category obj = new Category()
                            {
                                CategoryName = category.CategoryName,
                                SortOrder = category.SortOrder,
                                ParentCategoryId = parentCategory.CategoryId,
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
                            categoryBll.Save(obj, CurrentUser.UserId);
                        }
                    }
                }

                // set flag and message update successfully.
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
            try
            {
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
            catch
            {
                return new DataTable();
            }
        }
    }
}
