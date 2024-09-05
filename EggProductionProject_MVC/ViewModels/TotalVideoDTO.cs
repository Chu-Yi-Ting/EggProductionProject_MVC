using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class TotalVideoDTO
    {
        [Display(Name = "影片標題")]
        public string? VideoTitle { get; set; }

        [Display(Name = "觀看次數")]
        public int? TimesWatched { get; set; }

        [Display(Name = "影片內容")]
        public string? MoviePath { get; set; }

        [Display(Name = "影片上傳時間")]
        public DateTime UploadDate { get; set; }

        [Display(Name ="會員名稱")]
        public string NumberName { get; set; }
    }
}
