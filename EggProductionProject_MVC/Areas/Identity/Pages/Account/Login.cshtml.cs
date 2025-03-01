﻿using System;
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
using Newtonsoft.Json;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.IdentityModel.Tokens;

namespace EggProductionProject_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly EggPlatformContext _context;
        private readonly GoogleCaptchaService _captchaService;
		private readonly IConfiguration _configuration;

		//recapctha密鑰匙
		private readonly string secretKey = "6LdH2UQqAAAAAP-UAga-dBgpEtj5SrxfRdMb880_"; // 替換為你的 Secret Key
        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
             EggPlatformContext context,
             GoogleCaptchaService captchaService,
             IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _captchaService = captchaService;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
       
        public string Token { get; set; }

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
            public string Token { get; set; }
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
            Console.WriteLine(Input.Email+Input.Password+Input.Token);
			//驗證capchta token with google
			var captchaResult = await _captchaService.VerifyToken(Input.Token);
			// 打印或記錄分數
			
			if (!captchaResult)return Page();


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
					// 這裡生成 JWT token
					//var token = GenerateJwtToken(user);

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

                    // 重定向到指定的頁面並附帶 JWT token
                    //string redirectUrlWithToken = $"{returnUrl}?token={token}";
                    //return LocalRedirect(redirectUrlWithToken);
                   

					//舊版的
					return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
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


		private string GenerateJwtToken(IdentityUser user)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
    public class GoogleCaptchaConfig
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
    }
    public class GoogleCaptchaService
    {
        public async Task<bool> VerifyToken(string token)
        {
            try
            {
                var url = $"https://www.google.com/recaptcha/api/siteverify?secret=6LdH2UQqAAAAAP-UAga-dBgpEtj5SrxfRdMb880_&response={token}";
                using (var client = new HttpClient())
                {
                    var httpResult = await client.GetAsync(url);
                    if (httpResult.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }
                    var responseString = await httpResult.Content.ReadAsStringAsync();
                    var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);

                    return googleResult.Success && googleResult.score >= 0.5;
                }
            }
            catch (Exception ex) { return false; }
        }
    }

    public class GoogleCaptchaResponse
    {
        public bool Success { get; set; }
        public double score { get; set; }
    }



}
