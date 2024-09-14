using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class CreatVideoDTO
    {
       public int  CreatorSid { get; set; }

        [Display(Name = "影片標題")]
        public string VideoTitle { get; set; }

        [Display(Name = "影片資訊")]
       public string? InformationColumn { get; set; }

        [Display(Name = "影片時長")]
        public int? VideoDuration { get; set; }

        [Display(Name = "影片分類")]
        public int? NatureSid { get; set; }

        [Display(Name = "影片封面圖")]
        public string? VideoCoverImage { get; set; }

        [Display(Name = "影片內容")]
        public string? MoviePath { get; set; }


        [Display(Name = "影片上傳時間")]
        public DateTime UploadDate { get; set; }
    }
}
