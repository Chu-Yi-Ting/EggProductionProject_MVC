using EggProductionProject_MVC.Data;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.ArticlesDTO;

namespace EggProductionProject_MVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly EggPlatformContext _context;
        public ChatHub(EggPlatformContext context)
        {
            _context = context;
        }
        //接收客戶端的消息，並將其廣播給所有連接的客戶端
        public async Task SendMessage(int MemberSid, string ChatContent)
        {
            if (string.IsNullOrWhiteSpace(ChatContent))
            {
                // 如果訊息是空的，則不任何處理
                return;
            }
            var member = _context.Members.Find(MemberSid);  // 假設這裡有 Member 資料表
            if (member == null)
            {
                return;
            }
            var ChatTime = DateTime.UtcNow;

            // 廣播訊息給所有已連接的客戶端
            await Clients.All.SendAsync("ReceiveMessage", MemberSid, ChatContent, ChatTime);

            // 保存訊息到資料庫
            var chatMessageDto = new ChatMessageDto
            {
                MemberSid = member.MemberSid,
                Name = member.Name,  // 使用會員的名字
                ChatContent = ChatContent,
                ChatTime = ChatTime
            };
            await Clients.All.SendAsync("ReceiveMessage", chatMessageDto);

            // 保存訊息到資料庫
            var chatMessage = new ChatRoom
            {
                MemberSid = MemberSid,
                ChatContent = ChatContent,
                ChatTime = ChatTime
            };
            try
            {
                _context.ChatRooms.Add(chatMessage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 這裡可以進行日誌記錄，或者返回錯誤給客戶端
                Console.WriteLine($"Error saving message to the database: {ex.Message}");
                // 你也可以選擇將此錯誤信息返回給客戶端
                await Clients.Caller.SendAsync("ErrorMessage", "Failed to save message.");
            }
        }
    }
}

