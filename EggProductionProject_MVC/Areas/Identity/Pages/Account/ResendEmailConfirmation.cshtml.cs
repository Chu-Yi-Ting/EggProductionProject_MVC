using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace EggProductionProject_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "信箱為必填欄位！")]
            [EmailAddress(ErrorMessage ="不正確的信箱格式！")]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }


            // 生成驗證碼並發送驗證信
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);  // 確保token已被編碼
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code ,token = encodedToken },
                protocol: Request.Scheme);
            //修改寄件內容
            await _emailSender.SendEmailAsync(Input.Email, "建立GoodEgg帳戶",
                "<h2>驗證您的電子郵件位址</h2>" +
                $"此步驟是要驗證您用來登入GoodEgg好蛋雞農整合平台的電子郵件位址。若要完成建立帳戶，請按右方的[驗證連結]以進入網站。" +
                $"<a  href='{HtmlEncoder.Default.Encode(callbackUrl)}'>驗證帳戶</a>");

            ModelState.AddModelError(string.Empty, "驗證信已寄出，請於30分鐘內檢查信箱");
            return new JsonResult(new { success = true, message = "驗證信已寄出，請於30分鐘內檢查信箱" });
        }
    }
}
