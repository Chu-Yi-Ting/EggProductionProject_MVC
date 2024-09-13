using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class DeleteVideoDTO
    {
        [Display(Name ="狀態")]
        public int PublicStatusNo { get; set; }

        public int VideoSid {  get; set; }
    }
}
