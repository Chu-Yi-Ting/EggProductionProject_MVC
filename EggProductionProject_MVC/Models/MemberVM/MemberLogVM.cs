using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.Models.MemberVM
{
    public class MemberLogVM
    {
        public int MemberSid { get; set; }


        [Display(Name = "會員信箱")]
        public string Email { get; set; }
        [Display(Name = "會員密碼")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        
    }
}
