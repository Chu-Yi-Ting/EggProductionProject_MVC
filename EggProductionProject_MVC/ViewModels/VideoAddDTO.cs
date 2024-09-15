using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class VideoAddDTO
    {
        [Display(Name = "影片封面圖")]
        public string? VideoCoverImage { get; set; }

        [Display(Name = "影片內容")]
        public string? MoviePath { get; set; }

        public int CreatorSid { get; set; }

        public int VideoSid { get; set; }
    }
}
