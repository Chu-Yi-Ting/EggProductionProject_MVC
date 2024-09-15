using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.ArticlesDTO;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly EggPlatformContext _context;

        public ChatController(EggPlatformContext context)
        {
            _context = context;
        }
        // 獲取所有聊天記錄
        [HttpGet]
        public IActionResult GetMessages()
        {
            // 從資料庫中獲取所有聊天記錄，並包括會員名稱
            var messages = _context.ChatRooms
                .Select(c => new ChatMessageDto
                {
                    MemberSid = c.MemberSid,
                    Name = _context.Members.FirstOrDefault(m => m.MemberSid == c.MemberSid).Name, // 獲取會員名稱
                    ChatContent = c.ChatContent,
                    ChatTime = c.ChatTime ?? DateTime.UtcNow
                })
                .ToList();

            return Ok(messages); // 返回 HTTP 200 狀態碼和訊息數據
        }

        // 保存聊天記錄
        [HttpPost]
        public IActionResult SaveMessage([FromBody] ChatMessageDto chatMessageDto)
        {
            // 只保存必要的訊息到資料庫
            var chatMessage = new ChatRoom
            {
                MemberSid = chatMessageDto.MemberSid,
                ChatContent = chatMessageDto.ChatContent,
                ChatTime = chatMessageDto.ChatTime
            };

            _context.ChatRooms.Add(chatMessage);
            _context.SaveChanges();

            return Ok();
        }
    }
}
