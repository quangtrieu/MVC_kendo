using BCS.BusinessLogic;
using BCS.Entity;
using BCS.Framework.SecurityServices;
using BCS.Framework.Singleton;
using BCS.Web.Models;
using BCS.Web.WebUtils;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace BCS.Web.Controllers
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        /// <summary>
        /// Register User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string id, int systemId)
        {
            var model = new RegisterModel();
            model.TokenId = id;

            var result = BCSWebUtils.GetUserByTokenId(id);
            if (result != null && result.Content != null && result.Content.Length > 0)
            {
                var obj = JsonConvert.DeserializeObject<SSPUser>(result.Content);

                model.UserName = obj.UserName;
                model.FullName = obj.FirstName + " " + obj.LastName;
                model.Email = obj.Email;
                model.Phone = string.IsNullOrEmpty(obj.CellPhone) || obj.CellPhone.Equals("0") ? string.Empty : obj.CellPhone;
                model.RestCode = obj.Restaurant.RestCode;
                model.RestName = obj.Restaurant.RestName;
                model.SystemId = systemId;
            }

            return View(model);
        }

        /// <summary>
        /// Method save new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.TokenId))
                {
                    // Get user by token id.
                    var result = BCSWebUtils.GetUserByTokenId(model.TokenId);
                    if (result == null || result.Content == null || result.Content.Length == 0)
                    {
                        ModelState.AddModelError(string.Empty, string.Format("Invalid Token error on Register."));
                        return View(model);
                    }

                    // change information register user from ssp
                    var registerUser = model.GetUser();
                    registerUser.RoleId = 3; // set role id is SSP user
                    registerUser.Active = true; // set active register user

                    // save new user
                    var id = SingletonIpl.GetInstance<UserBll>().Save(registerUser, 0);

                    if (id > 0)
                    {
                        var entity = new RestActiveCode()
                        {
                            UserId = id,
                            RestCode = model.RestCode,
                            RestName = model.RestName,
                            TokenId = model.TokenId,
                            IsDefault = true,
                            DeletedFlg = false,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = id,
                            UpdatedDate = DateTime.Now,
                            UpdatedUserId = id
                        };

                        // save new rest active code by user
                        SingletonIpl.GetInstance<RestActiveCodeBll>().Save(entity, id);

                        // LogIn
                        SecurityService.Login(model.UserName.Trim(), model.Password.Trim(), true);
                        return RedirectToAction("Result", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, string.Format("Invalid Token error on Register."));
                    return View(model);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Method check token id exists system
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public JsonResult CheckTokenIdExist(string tokenId)
        {
            string message = string.Empty;
            var currentUser = SingletonIpl.GetInstance<UserBll>().GetUserByTokenId(tokenId);
            if (currentUser != null && currentUser.UserId > 0)
            {
                message = "This SSP user restaurant is already created or is linked under BCS user account in BCS system. Please Sign In with BCS user.";
            }
            return Json(new { Message = message}, JsonRequestBehavior.AllowGet);
        }
    }
}