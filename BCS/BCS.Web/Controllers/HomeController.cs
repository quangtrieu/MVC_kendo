using BCS.BusinessLogic;
using BCS.Framework.Singleton;
using BCS.Framework.Web;
using BCS.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using BCS.Entity;
using AutoMapper;
using BCS.Web.WebUtils;
using Newtonsoft.Json;
using System;
using System.IO;
using BCS.Commons;

namespace BCS.Web.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public async Task<ActionResult> Index()
        {
            if (CurrentUser != null)
            {
                // Check role is technical support
                if (CurrentUser.SystemId == BCSCommonData.SYSTEM_ID_BUDGET_SYSTEM && CurrentUser.RoleId == BCSCommonData.ROLE_NAME_SITE_TECH_SUPPORT)
                {
                    // set technical support data information to session
                    CurrentUser.SupportUser = CurrentUser;

                    return View();
                }
                else if (CurrentUser.SystemId == BCSCommonData.SYSTEM_ID_BUDGET_SYSTEM && CurrentUser.RoleId == BCSCommonData.ROLE_NAME_SUPPER_USER)
                {
                    // call back to user manager page
                    return RedirectToAction("Index", "User");
                }

                //var model = new DashBoardModel();
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        [HttpGet]
        public ActionResult Result()
        {
            return View();
        }

        public ActionResult ViewAccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Process Show or hidde help
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="helpSettingDataId"></param>
        /// <returns></returns>
        public ActionResult ShowOrHiddenHelp(bool flag, int helpSettingDataId)
        {
            bool status = false;
            string message = string.Empty;

            // get help setting by user id and help setting data id
            var currentHelpSetting = SingletonIpl.GetInstance<HelpSettingBll>().GetByUserIdAndHelpSettingDataId(CurrentUser.UserId, helpSettingDataId);

            // current help setting is null
            if (currentHelpSetting == null)
            {
                currentHelpSetting = new HelpSetting()
                {
                    UserId = CurrentUser.UserId,
                    HelpSettingDataId = helpSettingDataId,
                    DeletedFlg = false,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = CurrentUser.UserId
                };
            }

            currentHelpSetting.IsHidden = flag;
            currentHelpSetting.UpdatedDate = DateTime.Now;
            currentHelpSetting.UpdatedUserId = CurrentUser.UserId;

            // call method update
            var result = SingletonIpl.GetInstance<HelpSettingBll>().Save(currentHelpSetting, CurrentUser.UserId);
            if (result > 0)
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
        /// Method: delete file after download
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteFile(string folder, string fileName)
        {
            try
            {
                string path = Path.Combine(Server.MapPath(folder), fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}