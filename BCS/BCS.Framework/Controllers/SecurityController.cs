//******************************************************************************
//Description: Contains Actions and functions for SecurityController
//Remarks: SecurityController
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BCS.Framework.Constants;
using BCS.Framework.EmailServices;
using BCS.Framework.Models;
using BCS.Framework.SecurityServices;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.SecurityServices.Utils;
using BCS.Framework.Singleton;
using BCS.Framework.Utilities;
using BCS.Framework.Web;
using Utils = BCS.Framework.Commons.Utils;
using BCS.Framework.Commons;

namespace BCS.Framework.Controllers
{
    public class SecurityController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Session.CurrentUser() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                 var model = new LoginModel();

                 HttpCookie cookie = Request.Cookies[Constant.BCS_LOGIN_INFO];
                 if (cookie != null)
                 {
                     model.UserName = string.IsNullOrEmpty(cookie.Values[Constant.BCS_LOGIN_USERNAME])
                                     ? string.Empty
                                     : SecurityMethod.Base64Decode(cookie.Values[Constant.BCS_LOGIN_USERNAME]);

                     model.Password = string.IsNullOrEmpty(cookie.Values[Constant.BCS_LOGIN_PASSWORD])
                                     ? string.Empty
                                     : SecurityMethod.Base64Decode(cookie.Values[Constant.BCS_LOGIN_PASSWORD]);
                 }
                return View("Login", model);
            }
        }

        [AllowAnonymous]
        public ActionResult LogIn()
        {

            if (Session.CurrentUser() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var model = new LoginModel();

                 HttpCookie cookie = HttpContext.Request.Cookies[Constant.BCS_LOGIN_INFO];
                if (cookie != null && cookie.Expires >= DateTime.Now)
                {
                    model.UserName = string.IsNullOrEmpty(cookie.Values[Constant.BCS_LOGIN_USERNAME])
                                    ? string.Empty
                                    : SecurityMethod.Base64Decode(cookie.Values[Constant.BCS_LOGIN_USERNAME]);

                    model.Password = string.IsNullOrEmpty(cookie.Values[Constant.BCS_LOGIN_PASSWORD])
                                    ? string.Empty
                                    : SecurityMethod.Base64Decode(cookie.Values[Constant.BCS_LOGIN_PASSWORD]);

                    // LogIn
                    SecurityService.Login(model.UserName.Trim(), model.Password.Trim(), model.RememberMe);
                }
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult LogIn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (SecurityService.Login(model.UserName.Trim(), model.Password.Trim(), model.RememberMe))
                {
                   var ctx = SingletonIpl.GetInstance<DataProvider>();
                   var userActive = ctx.GetUser(model.UserName.Trim(), model.Password.Trim());
                   if (userActive.Active)
                   {
                       return RedirectToAction("Index", "Home");
                   }
                   else
                   {
                       // Logout for
                       SecurityService.Logout();
                       ModelState.AddModelError(string.Empty, string.Format("User Name or Email is inactive."));
                       return View(model);
                   }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, string.Format("The Username, Password is incorrect."));
                    return View(model);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            // Logout for
            SecurityService.Logout();

            return RedirectToAction("LogIn", "Security");
        }

        /// <summary>
        /// Action Forgot password
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Forgot()
        {
            if (Session.CurrentUser() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var model = new ForgotModel();
                return View(model);    
            }
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Forgot(ForgotModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["Captcha"] !=null && model.Sum.HasValue&& model.Sum == (int)Session["Captcha"])
                    {
                        var ctx = SingletonIpl.GetInstance<DataProvider>();
                        var user = ctx.GetUserByEmail(model.Email);
                        if (!user.Active)
                        {
                            ModelState.AddModelError(string.Empty, string.Format("User Name or Email is inactive."));
                            return View();
                        }
                        if (user != null)
                        {
                            var sender = Utils.GetSetting("mail-sender", "");
                            var guid = Guid.NewGuid();
                            var url = UrlUtil.GetCurrentHost() + string.Format("Security/ResetPassword/{0}", guid);

                            string body = Utilities.Utils.Stringtmp("ForgotPassword.txt",
                                new { User = user.FullName, Url = url });

                            EmailService.Send(sender, model.Email, "Forgot Password", body);

                            var forgotExpired = DateTime.UtcNow.AddHours(Utils.GetSetting<double>("expiredHour", 4));

                            ctx.ResetExpireDate(user.UserId, forgotExpired, guid);

                            return View("ForgotResult");
                        }
                    }
                    else
                    {
                        if (!model.Sum.HasValue)
                        {
                            ModelState.AddModelError("Sum", "The Captcha is required.");    
                        }
                        else if (Session["Captcha"] != null && model.Sum != (int)Session["Captcha"])
                        {
                            ModelState.AddModelError("Sum", "The Captcha is invalid. Please try again.");    
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }
        
        [AllowAnonymous]
        public ActionResult ResetPassword(Guid id)
        {
            var model = new ChangePasswordModel {ForgotCode = id};
            var ctx = SingletonIpl.GetInstance<DataProvider>();
            var user = ctx.GetUser(model.ForgotCode);

            if (user != null && user.ForgotExpired > DateTime.UtcNow)
            {
                ViewBag.Time_mail = user.ForgotExpired - DateTime.UtcNow;
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ChangePasswordModel model)
        {
            // Get user by forgot code
            var ctx = SingletonIpl.GetInstance<DataProvider>();
            var userExist = ctx.GetUser(model.ForgotCode);

            // check if not exists user or forgot expired date
            if (userExist != null && userExist.ForgotExpired > DateTime.UtcNow)
            {
                ViewBag.Time_mail = userExist.ForgotExpired - DateTime.UtcNow;

                // check validation model
                if (ModelState.IsValid)
                {
                    // call reset password
                    ctx.ResetPassword(userExist.UserId, model.PassWord);

                    // call reset expired date
                    ctx.ResetExpireDate(userExist.UserId, null, null);

                    // return view reset result
                    return View("ResetResult");
                }
            }
            else
            {
                ModelState.AddModelError("ConfirmPassword", "Password change has expired.");
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SessionTimeout()
        {

            Session.Timeout = Session.Timeout;

            return Json("", JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            //generate new question 
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("? = {0} + {1} ", a, b);

            //store answer 
            Session["Captcha" + prefix] = a + b;

            //image stream 
            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise 
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x - r, y - r, r, r);
                    }
                }

                //add question 
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg 
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }
    }
}
