using EggProductionProject_MVC.Models;
using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class ShowViedoSummary
    {
        [Key]
        public int VideoSid { get; set; }

        [Display(Name ="創作者編號")]
        public int? CreatorSid { get; set; }

        [Display(Name ="影片時長")]
        public int? VideoDuration { get; set; }

        [Display(Name ="影片標題")]
        public string? VideoTitle { get; set; }

        [Display(Name = "會員名稱")]
        public string? MemberName { get; set; }

        [Display(Name = "觀看次數")]
        public int? TimesWatched { get; set; }

        [Display(Name = "影片內容")]
        public string? MoviePath { get; set; }

        [Display(Name = "影片資訊")]
        public string? InformationColumn { get; set; }

        [Display(Name = "影片封面圖")]
        public string? VideoCoverImage { get; set; }

        [Display(Name = "影片上傳時間")]
        public DateTime UploadDate { get; set; }

        [Display(Name = "影片分類")]
        public string? ViedoNature { get; set; }

        [Display(Name = "已被投放廣告")]
        public bool? Advertised { get; set; }

        [Display(Name = "廣告來源")]
        public bool? AdSource { get; set; }

        [Display(Name ="字幕語言")]
        public string? ScreenTextCategory { get; set; }

        [Display(Name ="公開狀態")]
        public string? StatusDescription { get; set; }


        public virtual ICollection<Advertisment> Advertisments { get; set; } = new List<Advertisment>();

        public virtual Creator? CreatorS { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual Nature? NatureS { get; set; }

        public virtual PublicStatus? PublicStatusNoNavigation { get; set; }

        public virtual ScreenSummary? ScreenTextS { get; set; }
    }
}
