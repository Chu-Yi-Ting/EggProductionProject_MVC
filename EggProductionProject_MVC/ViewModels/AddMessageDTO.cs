using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public partial class AddMessageDTO
    {

        [Display(Name ="影片id")]
        public int VideoSid { get; set; }

        [Display(Name ="會員id")]
        public int MemberSid { get; set; }

        [Display(Name ="留言內容")]
        public string MessageContent { get; set; }

        [Display(Name ="再留言")]
        public int? MessageNumber { get; set; }
        [Display(Name ="留言讚數")]
        public int MessageLikes { get; set; }

        [Display(Name ="留言時間")]
       public DateTime MessageDate { get; set; }

        [Display(Name ="留言刪除")]
        public bool MessageDelete { get; set; }
    }
}
