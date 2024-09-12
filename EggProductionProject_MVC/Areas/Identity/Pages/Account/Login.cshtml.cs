using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EggProductionProject_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly EggPlatformContext _context;
        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
             EggPlatformContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {

            [EmailAddress(ErrorMessage = "請輸入正確的電子郵件地址")]
            [Required(ErrorMessage = "電子信箱為必填欄位")]
			//[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "請輸入有效的電子郵件地址")]
			public string Email { get; set; }

            [Required(ErrorMessage = "密碼為必填欄位")]
            [StringLength(100, ErrorMessage = "密碼必須至少為6個英文數字混和並且包含一個特殊符號的組合", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {

                // 使用 Email 查找使用者
                var user = await _userManager.FindByEmailAsync(Input.Email);
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    _logger.LogInformation("User logged in.");

                    // 這裡你可以存取到 user.Id 或其他使用者資料
                    
                    
                    var userEmail = user.Email;
                    //foreach(Member m in _context.Members)
                    //{
                    //    Console.WriteLine(m.AspUserId+">>"+m.Email);
                    //}
                    //登入的時候如果有找到該名會員(aspid去找)
                    var member = _context.Members.Include(m => m.AspUser).FirstOrDefault(x => x.AspUser.Id.ToLower() == user.Id.ToLower());

                    //登入時儲存使用者名稱、AspID、大頭貼路徑
                    if (member != null)
                    {
                        if (member.Name != null) { HttpContext.Session.SetString("userName", member.Name); }
                        if (member.ProfilePic != null) { HttpContext.Session.SetString("userProfilePic", member.ProfilePic); }
                        HttpContext.Session.SetInt32("userMemberSid", member.MemberSid);
                        HttpContext.Session.SetString("userId", user.Id);

                    }

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, "帳號密碼錯誤");
                    return Page();

                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
