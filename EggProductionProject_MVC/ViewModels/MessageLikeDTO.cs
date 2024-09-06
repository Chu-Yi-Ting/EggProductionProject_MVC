using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class MessageLikeDTO
    {
        public int MessageSid { get; set; }

        [Display(Name = "留言案讚")]
        public int? MessageLikes { get; set; }


    }
}
