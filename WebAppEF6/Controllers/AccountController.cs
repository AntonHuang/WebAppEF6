using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebAppEF6.Models;
using WebApp.Models;
using WebApp.Common;
using WebApp.DomainModels.Customer;

using System.Data.Entity;
using LinqKit;
using WebApp.DomainModels.Core;
using WebApp.DomainModels;

namespace WebAppEF6.Controllers
{
    [Authorize]
    public class AccountController : AsyncController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext _applicationDbContext;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager, 
            ApplicationDbContext dbContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            AppDbContext = dbContext;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        public ApplicationDbContext AppDbContext
        {
            get
            {
                return _applicationDbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _applicationDbContext = value;
            }
        }




        [HttpGet]
        public async Task<ActionResult> UserInfo()
        {
            //EnsureDatabaseCreated(_applicationDbContext);
            ApplicationUser user = await GetCurrentUserAsync();
            return await GetUserInfo(user);
        }

        private async Task<ActionResult> GetUserInfo(ApplicationUser user)
        {
            try
            {
                if (user == null)
                {
                    return HttpNotFound();
                }



                var role = (await this.UserManager.GetRolesAsync(user.UserName)).FirstOrDefault();
                UserInfoViewModel menber = null;
                if (user.MemberInfo == null)
                {
                    menber = await (from member in this.AppDbContext.Members
                                  where member.MemberID.Equals(user.UserName)
                                  select new UserInfoViewModel
                                  {
                                      ID = member.MemberID,
                                      Name = member.Name ?? "",
                                      Level = member.Level ?? "",
                                      Role = role ?? "",
                                      RegisterDate = member.RegisterDate.ToString("yyyy'-'MM'-'dd"),
                                      NeedToChangePassword = user.ChangedPassword == false
                                  }).FirstOrDefaultAsync();
                  
                } else
                {
                    menber =  new UserInfoViewModel
                    {
                        ID = user.MemberInfo.MemberID,
                        Name = user.MemberInfo.Name ?? "",
                        Level = user.MemberInfo.Level ?? "",
                        Role = role ?? "",
                        RegisterDate = user.MemberInfo.RegisterDate.ToString("yyyy'-'MM'-'dd"),
                        NeedToChangePassword = user.ChangedPassword == false
                    };
                }

                if (menber == null)
                {
                    return HttpNotFound();
                }

                MemberPointRule pointRule = await this.GetPointRule();
                menber.SelfPointRate = pointRule.GetPointRate(menber.Level, LevelRelation.Self).ValueOfNumber;
                menber.Down1PointRate = pointRule.GetPointRate(menber.Level, LevelRelation.Son).ValueOfNumber;
                menber.Down2PointRate = pointRule.GetPointRate(menber.Level, LevelRelation.Grandson).ValueOfNumber;

                menber.Level = GetLevelDisplayName(menber.Level);

                return JsonMessage.JsonResult(menber);
            }
            catch (Exception e)
            {
                return JsonMessage.BadRequestJsonResult(new
                {
                    Message = e.Message
                });
            }
        }

        private string GetLevelDisplayName(string level)
        {
            if ("LevelA".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "普通会员";
            }
            else if ("levelB".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员";
            }
            else if ("levelB1".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 1级";
            }
            else if ("levelB2".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 2级";
            }
            else if ("levelB3".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 3级";
            }
            else if ("levelB4".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 4级";
            }
            else if ("levelB5".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 5级";
            }
            else if ("levelB6".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 6级";
            }
            else if ("levelB7".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 7级";
            }
            else if ("levelB8".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return "高级会员 8级";
            }
            return string.Empty;
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // 这不会计入到为执行帐户锁定而统计的登录失败次数中
                // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
                var result = await this.SignInManager.PasswordSignInAsync(model.UserID, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        //return RedirectToLocal(returnUrl);
                        ApplicationUser user = await this.UserManager.FindByNameAsync(model.UserID);
                        return await GetUserInfo(user);

                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "无效的登录尝试。");
                        return View(model);
                }
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 要求用户已通过使用用户名/密码或外部登录名登录
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 以下代码可以防范双重身份验证代码遭到暴力破解攻击。
            // 如果用户输入错误代码的次数达到指定的次数，则会将
            // 该用户帐户锁定指定的时间。
            // 可以在 IdentityConfig 中配置帐户锁定设置
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代码无效。");
                    return View(model);
            }
        }


        [HttpGet]
        public async Task<ActionResult> NextAccountID()
        {
            var newID = await Task.Run<string>(() => {
                return IDGenerator.GetMemberIDGenerator(this.AppDbContext)
                                    .GetNext();
            });
            return JsonMessage.JsonResult(new { NextAccountID = newID ?? "" });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member menberReference = null;
                string refID = model.ReferenceID;
                if (String.IsNullOrWhiteSpace(refID))
                {
                    var role = this.RoleManager.FindByName("Administrator").Users.FirstOrDefault();

                    var adminUser = ( await this.UserManager.FindByIdAsync(role.UserId));
                    if (adminUser != null)
                    {
                        refID = adminUser.UserName;
                    }
                }

                if (String.IsNullOrWhiteSpace(refID) == false)
                {
                    menberReference = await (from m in this.AppDbContext.Members
                                       where m.MemberID.Equals(refID)
                                       select m
                                       ).FirstOrDefaultAsync();
                }

                if (menberReference == null)
                {
                    return JsonMessage.BadRequestJsonResult("ReferenceID is not exist.");
                }

                var member = new Member
                {
                    MemberID = model.AccountID,
                    Reference = menberReference,
                    Name = model.Name,
                    IDCard = model.CardID,
                    Address = model.Address,
                    Level = model.Level
                };

                var user = new ApplicationUser
                {
                    Id = model.AccountID,
                    UserName = model.AccountID,
                    MemberInfo = member,
                    PhoneNumber = model.Phone
                };

                this.AppDbContext.Members.Add(member);

                var result = await this.UserManager.CreateAsync(user, ApplicationUser.IINT_PASSWORD);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Context.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction(nameof(HomeController.Index), "Home");
                    return JsonMessage.JsonResult("OK");
                }
                AddErrors(result);
            }

            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userID = GetCurrentUserID();

                var result = await this.UserManager.ChangePasswordAsync(userID, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await GetCurrentUserAsync();
                    user.ChangedPassword = true;
                    var updateUserResult = await this.UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Json("OK", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return JsonMessage.BadRequestJsonResult(updateUserResult.Errors);
                    }
                }
                else
                {
                    return JsonMessage.BadRequestJsonResult(result.Errors);
                }
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }



        [HttpGet]
        public  ActionResult FindMember(FindMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = from u in this.AppDbContext.Users
                             join m in this.AppDbContext.Members
                             on u.UserName equals m.MemberID
                             orderby m.RegisterDate descending
                             select new MemberInfoModel
                             {
                                 MemberID = m.MemberID,
                                 ReferenceID = m.ReferenceMemberID,
                                 Name = m.Name,
                                 IDCard = m.IDCard,
                                 Address = m.Address,
                                 Phone = u.PhoneNumber,
                                 Level = m.Level
                             };

                if (string.IsNullOrWhiteSpace(model.MemberID) == false)
                {
                    //OR --> use False
                    var predicate = PredicateBuilder.False<MemberInfoModel>();

                    predicate = predicate.Or(m => m.MemberID != null
                                  && m.MemberID.StartsWith(model.MemberID));

                    predicate = predicate.Or(m => m.ReferenceID != null && m.ReferenceID.StartsWith(model.MemberID));

                    predicate = predicate.Or(m => m.Name != null && m.Name.IndexOf(model.MemberID) > -1);

                    predicate = predicate.Or(m => m.IDCard != null && m.IDCard.StartsWith(model.MemberID));

                    predicate = predicate.Or(m => m.Phone != null && m.Phone.StartsWith(model.MemberID));

                    result = result.Where(predicate.Compile()).AsQueryable();
                }

                if (string.IsNullOrWhiteSpace(model.ReferenceID) == false)
                {
                    result = result.Where(m => m.ReferenceID != null && m.ReferenceID.StartsWith(model.ReferenceID,
                                                                        StringComparison.InvariantCultureIgnoreCase));
                }
                if (string.IsNullOrWhiteSpace(model.Name) == false)
                {
                    result = result.Where(m => m.Name != null && m.Name.IndexOf(model.Name) > -1);
                }

                if (string.IsNullOrWhiteSpace(model.IDCard) == false)
                {
                    result = result.Where(
                        m => m.IDCard != null && m.IDCard.StartsWith(model.IDCard,
                                                     StringComparison.InvariantCultureIgnoreCase));
                }

                if (string.IsNullOrWhiteSpace(model.Phone) == false)
                {
                    result = result.Where(
                        m => m.Phone != null && m.Phone.StartsWith(model.Phone,
                                                                    StringComparison.InvariantCultureIgnoreCase));
                }

                int size = result.Count();

                // paging
                result = result.Skip(model.page * model.PageSize).Take(model.PageSize);

                var items = result.ToList();

                return Json(new
                {
                    TotalSize = size,
                    Members = items
                }, JsonRequestBehavior.AllowGet);

            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));

        }

        [HttpPost]
        public async Task<ActionResult> ModifyMember(MemberInfoModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await  this.AppDbContext.Users.
                               Where(u => u.UserName.Equals(model.MemberID, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefaultAsync();
                var member = await this.AppDbContext.Members
                               .Where(m => m.MemberID.Equals(model.MemberID, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefaultAsync();

                if (user != null || member != null)
                {
                    if (user != null)
                    {
                        user.PhoneNumber = model.Phone;
                    }

                    if (member != null)
                    {
                        member.Address = model.Address;
                        member.Level = model.Level;
                    }

                    try
                    {
                        await this.AppDbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        return JsonMessage.BadRequestJsonResult(e.Message);
                    }
                }
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        /*

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 请不要显示该用户不存在或者未经确认
                    return View("ForgotPasswordConfirmation");
                }

                // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                // 发送包含此链接的电子邮件
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "重置密码", "请通过单击 <a href=\"" + callbackUrl + "\">此处</a>来重置你的密码");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // 请不要显示该用户不存在
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 生成令牌并发送该令牌
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 如果用户没有帐户，则提示该用户创建帐户
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }*/

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


        /*
        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }*/

        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion



        private async Task<MemberPointRule> GetPointRule()
        {
            var ruleSetting = await (from s in this.AppDbContext.Settings
                                     where s.ID.Equals(SettingName.PointRule.ToString(), StringComparison.InvariantCultureIgnoreCase)
                                     select s
                          ).FirstOrDefaultAsync();
            if (ruleSetting == null)
            {
                return MemberPointRule.Default;
            }
            else
            {
                return MemberPointRule.fromJson(ruleSetting.SettingValue);
            }
        }


        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await this.UserManager.FindByIdAsync(GetCurrentUserID());
        }

        private string GetCurrentUserID() {

            var userID = SignInManager.GetVerifiedUserId();
            if (userID == null)
            {
                userID = User.Identity.GetUserId();
            }
            return userID;
        }


    }
}