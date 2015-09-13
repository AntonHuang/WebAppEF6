using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Common;
using WebApp.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using WebApp.DomainModels.Core;
using System.Security.Claims;
using WebApp.DomainModels.Customer;
using System.IO;
using Newtonsoft.Json;
using WebAppEF6;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using WebAppEF6.Models;
using WebApp.DomainModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{


    [Authorize(Roles = "Administrator,ShopManager")]
    public class SettingController : AsyncController
    {


        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext _applicationDbContext;

        public SettingController()
        {
        }

        public SettingController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager,
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
        public async Task<ActionResult> PointRule()
        {
            if (ModelState.IsValid)
            {
                SettingViewModel pointRuleSetting = await GetSetting(SettingName.PointRule.ToString());

                if (pointRuleSetting == null)
                {
                    var defaultRule = MemberPointRule.fromJson(null);
                    await PointRule(defaultRule);
                    return Json(defaultRule, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(MemberPointRule.fromJson(pointRuleSetting.Value), JsonRequestBehavior.AllowGet);
                }
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

    
        private async Task<ActionResult> PointRule(MemberPointRule pointRule)
        {
            if (ModelState.IsValid)
            {
                //MemberPointRule pointRule = null;
               if (pointRule.Level0 == null && Request.Form.Count > 0) {
                    string v = Request.Form["pointRule"];
                    pointRule = MemberPointRule.fromJson(v);
                }

                if (pointRule == null) {
                    return JsonMessage.BadRequestJsonResult("Cannot parse request context.");
                }

                //MemberPointRule r = JsonConvert.<MemberPointRule>()
                // string bodyStr = GetFromBodyString(Request);
                //MemberPointRule pointRule = MemberPointRule.fromJson(pointRuleValue);
                await AddOrUpdateSetting(SettingName.PointRule.ToString(), "json", pointRule.toJson());
                return Json("OK");
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.ToJson());
        }


        [HttpPost]
        public async Task<ActionResult> PointRule(PointRuleViewModel pointRule)
        {
            if (ModelState.IsValid)
            {
                var rule = new MemberPointRule {

                    Level0 = new LevelRule {
                        SelfRate = new Amount("%", pointRule.Level0SelfRate),
                        SonRate = new Amount("%", pointRule.Level0SonRate),
                        GrandsonRate = new Amount("%", pointRule.Level0GrandsonRate),
                    },
                    Level1 = new LevelRule {
                        SelfRate = new Amount("%", pointRule.Level1SelfRate),
                    },
                    AvailableAfter = TimeSpan.FromDays(pointRule.AvailableAfter)
                };
                
                return await PointRule(rule);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.ToJson());
        }


        private async Task AddOrUpdateSetting(string sName, string sValueT, string sValue)
        {
            Setting pointRuleSetting = await GetSettingEntity(sName);

            if (pointRuleSetting == null)
            {
                this.AppDbContext.Settings.Add(new Setting
                {
                    ID = sName,
                    Name = sName,
                    SettingValue = sValue,
                    SettingValueType = sValueT,
                    CreateBy = await this.GetCurrentUserName()
                });
                await this.AppDbContext.SaveChangesAsync();
            }
            else
            {
                SettingHistory history = new SettingHistory(pointRuleSetting);
                history.CreateBy = await this.GetCurrentUserName();

                pointRuleSetting.SettingValue = sValue;

                this.AppDbContext.SettingHistorys.Add(history);

                await this.AppDbContext.SaveChangesAsync();
            }
        }

        private async Task<SettingViewModel> GetSetting(string id)
        {
            return await (from s in this.AppDbContext.Settings
                          where s.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                          select new SettingViewModel
                          {
                              ID = s.ID,
                              Name = s.Name,
                              Value = s.SettingValue,
                              ValueType = s.SettingValueType,
                              Desc = s.Desc
                          }).FirstOrDefaultAsync();
        }

        private async Task<Setting> GetSettingEntity(string id)
        {
            return await (from s in this.AppDbContext.Settings
                          where s.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                          select s
                          ).FirstOrDefaultAsync();
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await this.UserManager.FindByIdAsync(GetCurrentUserID());
        }

        private string GetCurrentUserID()
        {

            var userID = SignInManager.GetVerifiedUserId();
            if (userID == null)
            {
                userID = User.Identity.GetUserId();
            }
            return userID;
        }

        private async Task<string> GetCurrentUserName()
        {
            var t = await GetCurrentUserAsync();
            return t.UserName;
        }

}
}
