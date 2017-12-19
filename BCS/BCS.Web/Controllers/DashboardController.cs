using AutoMapper;
using BCS.BusinessLogic;
using BCS.Commons;
using BCS.Entity;
using BCS.Framework.Singleton;
using BCS.Framework.Web;
using BCS.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCS.Web.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            // check permission
            if (CurrentUser.RoleId == BCSCommonData.ROLE_NAME_SUPPER_USER)
            {
                // call back to user manager page
                return RedirectToAction("Index", "User");
            }

            // get budget item by user id
            var result = SingletonIpl.GetInstance<BudgetBll>().GetBudgetByUserId(CurrentUser.UserId).ToList();
            Mapper.CreateMap<Budget, BudgetModel>();
            var budgetList = Mapper.Map<List<Budget>, List<BudgetModel>>(result);

            DashBoardModel model = new DashBoardModel()
            {
                BudgetList = budgetList,
            };

            // get permission
            ViewBag.SystemId = CurrentUser.SystemId;

            return View(model);
        }

        // GET: Dashboard/Delete/5
        public ActionResult Delete(int id)
        {
            // get budget by id
            var result = SingletonIpl.GetInstance<BudgetBll>().Get(id);

            // update deleted flag is true
            result.DeletedFlg = true;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedUserId = CurrentUser.UserId;

            // call action update budget
            var budgetId = SingletonIpl.GetInstance<BudgetBll>().Save(result, CurrentUser.UserId);

            // return result after update budget
            return Json(new { Status = (budgetId == id) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get all rest reference by current user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRestCodeByUser()
        {
            // get rest code by user id
            var restCodeList = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).ToList();

            // add not link rest
            RestActiveCode item = new RestActiveCode();
            item.RestCode = "Not Link";
            item.RestName = "Not link to a restaurant";
            restCodeList.Add(item);

            return Json(restCodeList.Select(s => new { RestCode = s.RestCode, RestName = string.Format("{0} - {1}", s.RestCode, s.RestName) }) , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get user by rest code
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserReferenceByUser(string restCode)
        {
            if (string.IsNullOrWhiteSpace(restCode))
            {
                // get rest code by user id
                var restCodeList = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).ToList();

                // get user list by rest code list
                restCodeList.ForEach(s => restCode += s.RestCode + ",");
                restCode = restCode.Substring(0, restCode.Length - 1);
            }

            // get user by rest code
            var userList = SingletonIpl.GetInstance<UserBll>().GetUserByRestCode(CurrentUser.UserId, restCode).ToList();
            return Json(userList.Select(s => new { UserId = s.UserId, UserName = s.FullName }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get budget type
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBudgetType()
        {
            // get common_Data by datacode
            var BudgetType = SingletonIpl.GetInstance<CommonBll>().GetCommonDataByCode(BCSCommonData.BUDGET_TYPE_CODE);
            return Json(BudgetType.Select(s => new { DataValue = s.DataValue, DataText = s.DataText }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: filter budget
        /// </summary>
        /// <returns></returns>
        public ActionResult FilterBudget()
        {
            DashBoardModel model = new DashBoardModel();
            if (Request.Cookies["restCode"] != null)
            {
                model.RestCode = JsonConvert.DeserializeObject<string>(HttpUtility.UrlDecode(Request.Cookies["restCode"].Value));
            }
            if (Request.Cookies["createdUser"] != null)
            {
                model.CreatedUserId = JsonConvert.DeserializeObject<string>(HttpUtility.UrlDecode(Request.Cookies["createdUser"].Value));
            }
            if (Request.Cookies["startDate"] != null)
            {
                model.StartDate = JsonConvert.DeserializeObject<string>(HttpUtility.UrlDecode(Request.Cookies["startDate"].Value));
            }
            if (Request.Cookies["endDate"] != null)
            {
                model.EndDate = JsonConvert.DeserializeObject<string>(HttpUtility.UrlDecode(Request.Cookies["endDate"].Value));
            }
            if (Request.Cookies["budgetType"] != null)
            {
                model.BudgetType = JsonConvert.DeserializeObject<string>(HttpUtility.UrlDecode(Request.Cookies["budgetType"].Value));
            }

            // get budget item by user id
            var result = SingletonIpl.GetInstance<BudgetBll>().GetBudgetByUserId(CurrentUser.UserId).ToList();

            if (!string.IsNullOrWhiteSpace(model.RestCode))
            {
                if (model.RestCode.Equals("Not Link"))
                {
                    result = result.Where(s => s.RestCode == null).ToList();
                }
                else
                {
                    result = result.Where(s => s.RestCode == model.RestCode).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(model.CreatedUserId))
            {
                var userId = Convert.ToInt32(model.CreatedUserId);
                result = result.Where(s => s.CreatedUserId == userId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.BudgetType))
            {
                var type = Convert.ToInt32(model.BudgetType);
                result = result.Where(s => s.BudgetLengthType == type).ToList();
            }
            if (!string.IsNullOrEmpty(model.StartDate))
            {
                var date = Convert.ToDateTime(model.StartDate);
                result = result.Where(s => s.BudgetLengthStart.Value.Year > date.Year || (s.BudgetLengthStart.Value.Year == date.Year && s.BudgetLengthStart.Value.Month >= date.Month)).ToList();
            }
            if (!string.IsNullOrEmpty(model.EndDate))
            {
                var date = Convert.ToDateTime(model.EndDate);
                result = result.Where(s => s.BudgetLengthStart.Value.Year < date.Year || (s.BudgetLengthStart.Value.Year == date.Year && s.BudgetLengthStart.Value.Month <= date.Month)).ToList();
            }

            Mapper.CreateMap<Budget, BudgetModel>();
            var budgetList = Mapper.Map<List<Budget>, List<BudgetModel>>(result);
            model.BudgetList = budgetList;
            return View("Index", model);
        }

        /// <summary>
        /// View recycle bin
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewRecycleBin()
        {
            // check permission
            if (CurrentUser.RoleId == BCSCommonData.ROLE_NAME_SUPPER_USER)
            {
                // call back to user manager page
                return RedirectToAction("Index", "User");
            }

            // get budget deleted item by user id
            var result = SingletonIpl.GetInstance<BudgetBll>().GetBudgetDeletedByUserId(CurrentUser.UserId).ToList();
            Mapper.CreateMap<Budget, BudgetModel>();
            var budgetList = Mapper.Map<List<Budget>, List<BudgetModel>>(result);

            DashBoardModel model = new DashBoardModel()
            {
                BudgetList = budgetList,
            };

            // get permission
            ViewBag.SystemId = CurrentUser.SystemId;

            return View(model);
        }

        /// <summary>
        /// Method restore budget deleted
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public ActionResult RestoreBudgetDeleted(int budgetId)
        {
            // get budget by id
            var result = SingletonIpl.GetInstance<BudgetBll>().GetBudgetDeletedByUserId(CurrentUser.UserId).FirstOrDefault(s => s.BudgetId == budgetId);

            // update deleted flag is true
            result.DeletedFlg = false;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedUserId = CurrentUser.UserId;

            // call action update budget
            var retoreBudgetId = SingletonIpl.GetInstance<BudgetBll>().Save(result, CurrentUser.UserId);

            // return result after update budget
            return Json(new { Status = (budgetId == retoreBudgetId) }, JsonRequestBehavior.AllowGet);
        }
    }
}
