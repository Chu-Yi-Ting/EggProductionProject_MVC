using System.Security.Claims;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EggPlatformContext _context;

        public UserInfoController(UserManager<IdentityUser> userManager,EggPlatformContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            // 從 token 中獲取使用者 ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // 查找與使用者相關的資料
                    var member = _context.Members.FirstOrDefault(m => m.AspUserId == user.Id);

                    // 返回使用者名稱和頭像等資料
                    return Ok(new
                    {
                        success = true,
                        userName = member?.Name ?? user.UserName,
                        profilePic = member?.ProfilePic // 可選：設定預設的圖片
                    });
                }
            }

            return Unauthorized(new { success = false, message = "Invalid token" });
        }
    }
}
