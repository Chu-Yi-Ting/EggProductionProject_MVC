using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class VideoMessageDTO
    {
        public int MessageSid { get; set; }
        public int? VideoSid { get; set; }

        [Display(Name = "會員sid")]
        public int MemberSid { get; set; }

        [Display(Name = "留言")]
        public string MessageContent { get; set; }
        [Display(Name = "留言案讚")]
        public int? MessageLikes { get; set; }

        [Display(Name = "留言時間")]
        public DateTime? MessageDate { get; set; }

        [Display(Name = "再留言")]
        public int? MessageNumber { get; set; }

        [Display(Name ="留言總數")]
        public int? MessageLength { get; set; }

        [Display(Name = "會員圖片")]
        public string MemberImage { get; set; }

        public string NumberName { get; set; }

    }
}
