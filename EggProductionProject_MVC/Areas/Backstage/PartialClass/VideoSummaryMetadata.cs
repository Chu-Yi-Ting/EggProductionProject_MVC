using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.Models
{
    public class VideoSummaryMetadata
    {
        [Key]
        public int VideoSid { get; set; }

        [Display(Name = "影片時長")]
        public TimeOnly? VideoDuration { get; set; }

        [Display(Name = "影片標題")]
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

        [Display(Name = "是否已經被當作廣告使用")]
        public bool? Advertise { get; set; }




    }
}