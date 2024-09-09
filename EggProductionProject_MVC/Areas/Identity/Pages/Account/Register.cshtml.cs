using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace EggProductionProject_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly EggPlatformContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            EggPlatformContext context)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            
            [EmailAddress(ErrorMessage = "請輸入正確的電子郵件地址")]
            [Display(Name = "Email")]
			[Required(ErrorMessage = "電子信箱為必填欄位")]
			//[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "請輸入有效的電子郵件地址")]
			public string Email { get; set; }

            [Required(ErrorMessage = "密碼為必填欄位")]
            [StringLength(100, ErrorMessage = "密碼必須至少為6個英文數字混和並且包含一個特殊符號的組合", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]        
            [Compare("Password", ErrorMessage = "兩次輸入的密碼不一致")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    //新增會員帳號的同時新增一個member
                    Member member = new Member { AspUserId = user.Id, Email = Input.Email, IsChickFarm = 0, IsBlocked = 0 };
                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //修改寄件內容
                    await _emailSender.SendEmailAsync(Input.Email, "建立GoodEgg帳戶",
                        "<h2>驗證您的電子郵件位址</h2>" +
                        $"此步驟是要驗證您用來登入GoodEgg好蛋雞農整合平台的電子郵件位址。若要完成建立帳戶，請按右方的[驗證連結]以進入網站。" +
                        $"<a  href='{HtmlEncoder.Default.Encode(callbackUrl)}'>驗證帳戶</a>");


                    // 設定 ViewData 變數來觸發 Modal
                    ViewData["ShowModal"] = true;

                    //如果需要驗證信箱的話(此專案是要)
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //跳轉到這個頁面，但我目前不想讓他跳轉
                        //return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });

                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);

                    }
                }
               //信箱重複或其他的錯誤
                foreach (var error in result.Errors)
                {
                    //ModelState.AddModelError(string.Empty, error.Description);

                    if (error.Code == "DuplicateEmail")
                    {
                        // 這裡加入信箱重複的錯誤訊息，傳回前端
                        TempData["DuplicateEmailError"] = "信箱已經存在，請使用其他信箱";
                        // 回傳信箱重複的 JSON
                        return new JsonResult(new { success = false, message = "信箱已經存在，請使用其他信箱。" });
                    }
                    
                }
            }

            // If we got this far, something failed, redisplay form
            //return Page();

            // 如果有其他錯誤，回傳一般錯誤訊息
            return new JsonResult(new { success = false });
        }
    }
}
