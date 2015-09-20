using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppEF6.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "代码")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "记住此浏览器?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        
        public string AccountID { get; set; }

        public string ReferenceID { get; set; }

        public string Name { get; set; }
        public string CardID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Level { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class FindMemberViewModel
    {
        public string MemberID { get; set; }
        public string ReferenceID { get; set; }
        public string Name { get; set; }
        public string IDCard { get; set; }
        public string Phone { get; set; }

        public int page { get; set; }
        public int PageSize { get; set; }
    }

    public class MemberInfoModel
    {
        public string MemberID { get; set; }
        public string ReferenceID { get; set; }
        public string Name { get; set; }
        public string IDCard { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }
        public string Level { get; set; }
    }

    public class UserInfoViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Role { get;  set; }
        public string RegisterDate { get;  set; }
        public bool NeedToChangePassword { get;  set; }
        public decimal SelfPointRate { get;  set; }
        public decimal Down1PointRate { get;  set; }
        public decimal Down2PointRate { get;  set; }
    }
}
