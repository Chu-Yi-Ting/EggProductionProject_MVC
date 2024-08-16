using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.Models.MemberVM
{
    public class MemberLogVM
    {
        public int MemberSid { get; set; }

        [Display(Name = "會員名稱")]
        public string Name { get; set; }
        [Display(Name = "會員信箱")]

        public string Email { get; set; }
        [Display(Name = "聯絡電話")]

        public bool RememberMe { get; set; }
        
    }
}
