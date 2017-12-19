using AutoMapper;
using BCS.BusinessLogic;
using BCS.Commons;
using BCS.Entity;
using BCS.Framework.Singleton;
using BCS.Framework.Web;
using BCS.Web.Models;
using BCS.Web.WebUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BCS.Web.Controllers
{
    public class LinkRestaurantController : BaseController
    {
        // GET: ListRestaurant
        public ActionResult Index()
        {
            return View();
        }

        // GET: ListRestaurant
        public ActionResult ConfigRestaurant(string isNotShow)
        {
            // get rest code by current user
            var result = SingletonIpl.GetInstance<RestActiveCodeBll>().GetRestCodeByUserId(CurrentUser.UserId).ToList();
            Mapper.CreateMap<RestActiveCode, RestActiveCodeModel>();
            var restList = Mapper.Map<List<RestActiveCode>, List<RestActiveCodeModel>>(result);
            ViewBag.RestaurantList = restList;
            ViewBag.IsNotShow = isNotShow;

            return View();
        }

        /// <summary>
        /// Method check token id exists system after add reference rest code
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public JsonResult AddReferenceRestCode(string tokenId)
        {
            bool status = false;
            string message = string.Empty;

            // get rest active code by token id
            var restActiveCodelBll = SingletonIpl.GetInstance<RestActiveCodeBll>();
            var restActiveCode = restActiveCodelBll.GetByToken(tokenId);
            if (restActiveCode != null && restActiveCode.UserId > 0)
            {
                message = "This SSP user already was linked by other BCS user. Please select other SSP user to link your Restaurant.";
            }
            else
            {
                // this token is not exists: add reference rest code to this user
                var sspUser = BCSWebUtils.GetUserByTokenId(tokenId);
                if (sspUser != null && sspUser.Content != null && sspUser.Content.Length > 0)
                {
                    var objSSPUser = JsonConvert.DeserializeObject<SSPUser>(sspUser.Content);

                    // check flag is default case user standlone.
                    var isDefaultFlag = (CurrentUser.RoleId == BCSCommonData.ROLE_NAME_BCS_USER && CurrentUser.SystemId == BCSCommonData.SYSTEM_ID_BUDGET_SYSTEM) ? true : false;

                    // check rest code and user id exists bcs system
                    var checkResult = restActiveCodelBll.GetByUserIdAndRestCode(CurrentUser.UserId, objSSPUser.Restaurant.RestCode);
                    if (checkResult != null)
                    {
                        message = "This restaurant is already linked.";
                    }
                    else
                    {
                        restActiveCode = new RestActiveCode()
                        {
                            UserId = CurrentUser.UserId,
                            RestCode = objSSPUser.Restaurant.RestCode,
                            RestName = objSSPUser.Restaurant.RestName,
                            TokenId = objSSPUser.TokenId,
                            IsDefault = isDefaultFlag,
                            DeletedFlg = false,
                            CreatedUserId = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedUserId = CurrentUser.UserId,
                            UpdatedDate = DateTime.Now,
                        };

                        var restActiveCodeId = restActiveCodelBll.Save(restActiveCode, CurrentUser.UserId);
                        if (restActiveCodeId > 0)
                        {
                            status = true;
                            message = "Add Reference Successfully.";

                            // get current user by id
                            var userBll = SingletonIpl.GetInstance<UserBll>();
                            var currentUser = userBll.Get(CurrentUser.UserId);

                            // check if system of current user is budget system
                            if (currentUser.SystemId == BCSCommonData.SYSTEM_ID_BUDGET_SYSTEM)
                            {
                                // change system of current user to ssp system after link rest successfully.
                                currentUser.SystemId = BCSCommonData.SYSTEM_ID_SSP_SYSTEM;
                                CurrentUser.SystemId = BCSCommonData.SYSTEM_ID_SSP_SYSTEM;

                                // call save user info
                                userBll.Save(currentUser, CurrentUser.UserId);
                            }
                        }
                        else
                        {
                            message = "Add reference failed.";
                        }
                    }
                }
            }
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }
 
        /// <summary>
        /// Method update default rest code by user id
        /// </summary>
        /// <param name="restCode"></param>
        /// <returns></returns>
        public JsonResult UpdateDefaultRestCode(string restCode)
        {
            bool status = false;
            string message = string.Empty;

            // reset not default rest active code
            if (!string.IsNullOrEmpty(restCode) && restCode.Equals("NotDefault"))
            {
                // call action reset not default rest active code
                SingletonIpl.GetInstance<RestActiveCodeBll>().ResetNotDefaultRestCodeByUserId(CurrentUser.UserId);

                // return after change
                return Json(new { Status = true, Message = "Successful." }, JsonRequestBehavior.AllowGet);
            }


            // check rest code is not exists by user
            var restActiveCode = SingletonIpl.GetInstance<RestActiveCodeBll>().GetByUserIdAndRestCode(CurrentUser.UserId, restCode);
            if (restActiveCode == null)
            {
                message = "RestCode is not exists.";
            }
            else
            {
                // update this rest code is default: true
                var result = SingletonIpl.GetInstance<RestActiveCodeBll>().UpdateDefaultRestCodeByUser(CurrentUser.UserId, restCode);

                if (result)
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

    }
}
