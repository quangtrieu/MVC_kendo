using AutoMapper;
using BCS.BusinessLogic;
using BCS.Commons;
using BCS.Entity;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Entity;
using BCS.Framework.SecurityServices.Utils;
using BCS.Framework.Singleton;
using BCS.Framework.Web;
using BCS.Web.Models;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BCS.Web.Controllers
{
    public class UserController : BaseController
    {
        private static int _SystemAdmin_UserId = BCS.Framework.Commons.Utils.GetSetting("SystemAdminUserId", 1);

        #region User manamgement

        public ActionResult Index()
        {
            // check permission
            if (CurrentUser.RoleId != BCSCommonData.ROLE_NAME_SUPPER_USER)
            {
                // call back to user manager page
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        public ActionResult ViewEditUser(int userId)
        {
            // 1. Init user model
            UserModel model = null;

            // 2. Get user case edit
            if (userId > 0)
            {
                var result = SingletonIpl.GetInstance<UserBll>().Get(userId);
                //Mapper.CreateMap<User, UserInfo>();
                //UserInfo user = Mapper.Map<User, UserInfo>(result);
                model = new UserModel(result);
            }
            else
            {
                // 3. Init user model case add
                model = new UserModel();
            }

            // return view edit user
            return PartialView("_ViewEditUser", model);
        }

        /// <summary>
        /// Method: save change data information by user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditUser(UserModel user)
        {
            // init validation message and flag
            string message = string.Empty;
            bool status = false;

            // 1. Check duplicate email
            var userBll = SingletonIpl.GetInstance<UserBll>();
            var duplicateEmail = userBll.CheckDuplicateEmail(user.UserId, user.Email);

            // 2. Check duplicate user name
            var duplicateUserName = userBll.CheckDuplicateUserName(user.UserId, user.UserName);

            // 3. Check is duplicate email or user name, return message warning to form
            if (duplicateEmail || duplicateUserName)
            {
                return Json(new { DuplicateEmail = duplicateEmail, DuplicateUserName = duplicateUserName }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // check change password, after encode password to new user
                var newUser = user.GetUser();
                if (user.UserId != 0 && user.OldPassword != user.Password)
                {
                    newUser.Password = (user.Password == null) ? user.OldPassword : SecurityMethod.MD5Encrypt(user.Password);
                }

                // 4. Call save edit user
                int newUserId = userBll.Save(newUser, CurrentUser.UserId);

                // 5. Set status after edit
                if (newUserId > 0)
                {
                    status = true;
                }
                else
                {
                    message = "Process error!";
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: set change active status by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult ChangeActiveStatusByUserId(int userId, bool activeFlg)
        {
            // init validation message and flag
            string message = string.Empty;
            bool status = false;

            try
            {
                // 1. Get user by id
                var userBll = SingletonIpl.GetInstance<UserBll>();
                var currentUser = userBll.Get(userId);

                // 2. Set status flag
                currentUser.Active = activeFlg;

                // 3. Call save edit user
                int newUserId = userBll.Save(currentUser, CurrentUser.UserId);

                // 4. Set status after delete
                if (newUserId > 0)
                {
                    status = true;
                }
                else
                {
                    message = "Process error!";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: set delete flag by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult DeleteByUserId(int userId)
        {
            // init validation message and flag
            string message = string.Empty;
            bool status = false;

            try
            {
                // 1. Get user by id
                var userBll = SingletonIpl.GetInstance<UserBll>();
                var currentUser = userBll.Get(userId);

                // 2. Set deleted flag
                currentUser.DeletedFlg = true;

                // 3. Call save edit user
                int newUserId = userBll.Save(currentUser, CurrentUser.UserId);

                // 4. Set status after delete
                if (newUserId > 0)
                {
                    status = true;
                }
                else
                {
                    message = "Process error!";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: show view swith user
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewSwitchUser()
        {
            // return view swith user
            return PartialView("_ViewSwitchUser");
        }

        /// <summary>
        /// Method: swith other user by support user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult SwithUserById(int userId)
        {
            // init validation message and flag
            string message = string.Empty;
            bool status = false;

            try
            {
                // 1. get user support by id
                var userBll = SingletonIpl.GetInstance<UserBll>();
                var swithUser = userBll.Get(userId);

                // 2. get support user
                var supportUser = CurrentUser.SupportUser;

                // 3. Set swith user to session
                Mapper.CreateMap<User, UserInfo>();
                CurrentUser = Mapper.Map<User, UserInfo>(swithUser);

                // 4. set support user to switch user
                CurrentUser.SupportUser = supportUser;

                // set return flag
                status = true;
                message = string.Format("Swith to user: {0} - {1}", swithUser.UserName, swithUser.FullName);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: back to switch user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult BackToSwithUser()
        {
            // init validation message and flag
            string message = string.Empty;
            bool status = false;

            try
            {
                // 1. get support user
                var supportUser = CurrentUser.SupportUser;

                // 2. get user support by id
                var userBll = SingletonIpl.GetInstance<UserBll>();
                var swithUser = userBll.Get(supportUser.UserId);

                // 3. Set swith user to session
                Mapper.CreateMap<User, UserInfo>();
                CurrentUser = Mapper.Map<User, UserInfo>(swithUser);

                // 4. set support user to current session
                //CurrentUser.SupportUser = supportUser;

                // set return flag
                status = true;
                message = "Back To Swith User Successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get users use swith function
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetUsers([DataSourceRequest] DataSourceRequest request)
        {
            // Get user by role
            var userBll = SingletonIpl.GetInstance<UserBll>();
            var bcsSystemUser = userBll.GetUserByRole(CurrentUser.UserId, BCSCommonData.SYSTEM_ID_BUDGET_SYSTEM, BCSCommonData.ROLE_NAME_BCS_USER, _SystemAdmin_UserId).Where(s => s.DeletedFlg == false && s.Active == true);
            var sspSystemUser = userBll.GetUserByRole(CurrentUser.UserId, BCSCommonData.SYSTEM_ID_SSP_SYSTEM, BCSCommonData.ROLE_NAME_BCS_USER, _SystemAdmin_UserId).Where(s => s.DeletedFlg == false && s.Active == true);
            var result = bcsSystemUser.Union(sspSystemUser).OrderBy(s => s.UserName).Select(s => new { UserId = s.UserId, UserName = string.Format("{0} - {1}", s.UserName, s.FullName) });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get user by role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="systemId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ActionResult GetUserByRole([DataSourceRequest] DataSourceRequest request, int systemId, int roleId)
        {
            // Get user by role
            var result = SingletonIpl.GetInstance<UserBll>().GetUserByRole(CurrentUser.UserId, systemId, roleId, _SystemAdmin_UserId).Where(s => s.UserId != CurrentUser.UserId);

            // filter
            if (request.Filters != null && request.Filters.Count > 0)
            {
                // get all filter from request
                var filterList = new List<IFilterDescriptor>();
                this.GetFilterDescriptionByRequest(filterList, request.Filters.First());

                //filterList = request.Filters[0] as CompositeFilterDescriptor;
                foreach (FilterDescriptor filter in filterList)
                {
                    // check column filter by UserName
                    if (filter.Member == "UserName")
                    {
                        result = result.Where(s =>
                            // filter start width
                            (filter.Operator == FilterOperator.StartsWith && s.UserName.StartsWith(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                            // filter contains
                            (filter.Operator == FilterOperator.Contains && s.UserName.ToLower().Contains(filter.Value.ToString().ToLower()) == true) ||
                            // filter is equal
                            (filter.Operator == FilterOperator.IsEqualTo && s.UserName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                            // filter is not equal
                            (filter.Operator == FilterOperator.IsNotEqualTo && s.UserName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == false)
                        );
                    }

                    // check column filter by FullName
                    if (filter.Member == "FullName")
                    {
                        result = result.Where(s =>
                            // filter start width
                            (filter.Operator == FilterOperator.StartsWith && s.FullName.StartsWith(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                            // filter contains
                            (filter.Operator == FilterOperator.Contains && s.FullName.ToLower().Contains(filter.Value.ToString().ToLower()) == true) ||
                            // filter is equal
                            (filter.Operator == FilterOperator.IsEqualTo && s.FullName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                            // filter is not equal
                            (filter.Operator == FilterOperator.IsNotEqualTo && s.FullName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == false)
                        );
                    }

                    // check column filter by Email
                    if (filter.Member == "Email")
                    {
                        result = result.Where(s =>
                            // filter start width
                            (filter.Operator == FilterOperator.StartsWith && s.Email.StartsWith(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                            // filter contains
                            (filter.Operator == FilterOperator.Contains && s.Email.ToLower().Contains(filter.Value.ToString().ToLower()) == true) ||
                            // filter is equal
                            (filter.Operator == FilterOperator.IsEqualTo && s.Email.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                            // filter is not equal
                            (filter.Operator == FilterOperator.IsNotEqualTo && s.Email.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == false)
                        );
                    }
                }
            }

            Mapper.CreateMap<User, UserModel>();
            var data = Mapper.Map<IList<User>, IList<UserModel>>(result.ToList());

            return Json(new DataSourceResult { Data = data, Total = data.Count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: get user by role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="systemId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ActionResult GetSSPMember([DataSourceRequest] DataSourceRequest request, int systemId, int roleId)
        {
            // Get user by role
            var result = SingletonIpl.GetInstance<UserBll>().GetSSPMember(CurrentUser.UserId, systemId, roleId);

            // filter
            if (request.Filters != null && request.Filters.Count > 0)
            {
                // get all filter from request
                var filterList = new List<IFilterDescriptor>();
                this.GetFilterDescriptionByRequest(filterList, request.Filters.First());

                //filterList = request.Filters[0] as CompositeFilterDescriptor;
                foreach (FilterDescriptor filter in filterList)
                {
                    // check column filter by UserName
                    if (filter.Member == "UserName")
                    {
                        result = result.Where(s =>
                            // filter start width
                            (filter.Operator == FilterOperator.StartsWith && s.UserName.StartsWith(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                                // filter contains
                            (filter.Operator == FilterOperator.Contains && s.UserName.ToLower().Contains(filter.Value.ToString().ToLower()) == true) ||
                                // filter is equal
                            (filter.Operator == FilterOperator.IsEqualTo && s.UserName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                                // filter is not equal
                            (filter.Operator == FilterOperator.IsNotEqualTo && s.UserName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == false)
                        ).ToList();
                    }

                    // check column filter by FullName
                    if (filter.Member == "FullName")
                    {
                        result = result.Where(s =>
                            // filter start width
                            (filter.Operator == FilterOperator.StartsWith && s.FullName.StartsWith(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                                // filter contains
                            (filter.Operator == FilterOperator.Contains && s.FullName.ToLower().Contains(filter.Value.ToString().ToLower()) == true) ||
                                // filter is equal
                            (filter.Operator == FilterOperator.IsEqualTo && s.FullName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                                // filter is not equal
                            (filter.Operator == FilterOperator.IsNotEqualTo && s.FullName.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == false)
                        ).ToList();
                    }

                    // check column filter by Email
                    if (filter.Member == "Email")
                    {
                        result = result.Where(s =>
                            // filter start width
                            (filter.Operator == FilterOperator.StartsWith && s.Email.StartsWith(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                                // filter contains
                            (filter.Operator == FilterOperator.Contains && s.Email.ToLower().Contains(filter.Value.ToString().ToLower()) == true) ||
                                // filter is equal
                            (filter.Operator == FilterOperator.IsEqualTo && s.Email.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == true) ||
                                // filter is not equal
                            (filter.Operator == FilterOperator.IsNotEqualTo && s.Email.Equals(filter.Value.ToString(), StringComparison.OrdinalIgnoreCase) == false)
                        ).ToList();
                    }
                }
            }

            Mapper.CreateMap<User, UserModel>();
            var data = Mapper.Map<IList<User>, IList<UserModel>>(result.ToList());

            return Json(new DataSourceResult { Data = data, Total = data.Count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method: loop all filter in request add to filter list
        /// </summary>
        /// <param name="filterList"></param>
        /// <param name="obj"></param>
        private void GetFilterDescriptionByRequest(List<IFilterDescriptor> filterList, object obj)
        {
            // check type is Filter Description add to filter list
            if (obj.GetType() == typeof(FilterDescriptor))
            {
                filterList.Add(obj as FilterDescriptor);
            }
            // check type is FilterDescriptorCollection add to filter list
            else if (obj.GetType() == typeof(Kendo.Mvc.Infrastructure.Implementation.FilterDescriptorCollection))
            {
                var filterDescriptorCollection = (obj as Kendo.Mvc.Infrastructure.Implementation.FilterDescriptorCollection);
                foreach (var item in filterDescriptorCollection)
                {
                    filterList.Add(item as FilterDescriptor);
                }
            }
            // check type is CompositeFilterDescriptor get FilterDescriptors after reCall this method
            else if (obj.GetType() == typeof(CompositeFilterDescriptor))
            {
                // get all filter descriptors
                var filterDescriptors = (obj as CompositeFilterDescriptor).FilterDescriptors;
                foreach (var item in filterDescriptors)
                {
                    // call this method
                    this.GetFilterDescriptionByRequest(filterList, item);
                }
            }
        }

        #endregion

        #region My Profile by current user

        [HttpGet]
        public ActionResult MyProfile()
        {
            // get user infor by user id
            var result = SingletonIpl.GetInstance<UserBll>().Get(CurrentUser.UserId);
            Mapper.CreateMap<User, UserInfo>();
            UserInfo user = Mapper.Map<User, UserInfo>(result);
            var model = new UserModel(user);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var model = new ResetPasswordModel(CurrentUser);
            model.CheckOldPassword = "true";
            model.CheckPassword = "true";
            return PartialView(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveChangePassword(ResetPasswordModel model)
        {
            try
            {
                var newPassword = BCS.Framework.SecurityServices.Utils.SecurityMethod.MD5Encrypt(model.Password);
                if (newPassword != CurrentUser.Password)
                {
                    // 1. change pass
                    SingletonIpl.GetInstance<DataProvider>().ResetPassword(model.UserId, model.Password);

                    // 2. call reset forgot password
                    SingletonIpl.GetInstance<DataProvider>().ResetExpireDate(CurrentUser.UserId, null, null);
                }
                return Json(new { Status = true, Message = "Your password has been changed successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Status = false, Message = "Your password has been changed unsuccessfully." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ChangeResult()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveMyProfile(UserModel model)
        {
            try
            {
                // check duplicate email
                var existMail = SingletonIpl.GetInstance<UserBll>().CheckDuplicateEmail(model.UserId, model.Email);
                if (existMail)
                {
                    return Json(new { StatusCheckDuplicateEmail = existMail }, JsonRequestBehavior.AllowGet);
                }

                // call action update user
                var id = SingletonIpl.GetInstance<UserBll>().Save(model.GetUser(), CurrentUser.UserId);
                if (id > 0)
                {
                    // get support user if exists
                    var supportUser = CurrentUser.SupportUser;

                    // set new information to current session
                    var result = SingletonIpl.GetInstance<UserBll>().Get(CurrentUser.UserId);
                    Mapper.CreateMap<User, UserInfo>();
                    CurrentUser = Mapper.Map<User, UserInfo>(result);

                    // set support user to session after is not null
                    if (supportUser != null)
                    {
                        CurrentUser.SupportUser = supportUser;
                    }

                    // return message sucessful
                    return Json(new { Status = true, Message = "Update User profile Successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = false, Message = "Update User profile Unsuccessfully." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Update User profile Unsuccessfully." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}