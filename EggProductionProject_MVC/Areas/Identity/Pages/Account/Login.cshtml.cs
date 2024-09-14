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
using Microsoft.AspNetCore.Http;

namespace EggProductionProject_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly EggPlatformContext _context;
        private readonly TokenService _tokenService;
        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
             EggPlatformContext context,
             TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        //第三方登入
        //public List<AuthenticationScheme> Schemes { get; set; }

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

                        if (member.IsBlocked == 1)
                        {
                            HttpContext.Session.Clear();
                            return RedirectToPage("./Lockout");
                        }                  
                    
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
                    
                    ModelState.AddModelError(string.Empty, "登入失敗，請重新嘗試");
                    return Page();

                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        // 用於處理 QR Code 掃描後的請求
        [HttpGet]
        public async Task<IActionResult> AuthenticateQRCode(string token)
        {
            // 驗證 token 並獲取對應的 userId
            var userId = _tokenService.ValidateToken(token);
            if (userId == null)
            {
                return Unauthorized(); // token 無效或過期
            }

            // 根據 userId 查找對應的用戶
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(); // 無效的用戶
            }

            // 確認用戶的身份，並進行登入
            await _signInManager.SignInAsync(user, isPersistent: false);

            // 登入成功後重定向到首頁
            return RedirectToAction("Index", "Home");
        }

        // 生成 QR Code 登入頁面，並顯示 QR Code
        [HttpGet]
        public async Task<IActionResult> QRCodeLogin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // 如果用戶未登入，請求登入
            }

            // 生成 QR Code 的 token
            var token = _tokenService.GenerateToken(user.Id);

            // 生成 QR Code URL，該 URL 包含 token
            var qrCodeUrl = Url.Action("AuthenticateQRCode", "Account", new { token }, Request.Scheme);
            if (qrCodeUrl == null)
            {
                // 記錄錯誤或日誌
                throw new Exception("QR Code URL 生成失敗");
            }
            // 將 QR Code URL 傳給視圖
            ViewData["QRCodeUrl"] = qrCodeUrl;
            return Page();
        }
    }



    public class TokenService
    {
        private readonly Dictionary<string, (string UserId, DateTime Expiry)> _tokens = new Dictionary<string, (string, DateTime)>();

        // 生成唯一的 QR Code token 並存儲用戶對應
        public string GenerateToken(string userId)
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5); // Token 5 分鐘過期
            _tokens[token] = (userId, expiry);
            return token;
        }

        // 驗證 token 是否有效
        public string ValidateToken(string token)
        {
            if (_tokens.TryGetValue(token, out var tokenInfo) && tokenInfo.Expiry > DateTime.UtcNow)
            {
                return tokenInfo.UserId; // 返回用戶ID
            }

            return null; // token 無效
        }
    }

}

