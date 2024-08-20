using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.Models.MemberVM
{
    public class EmpLog
    {
        public int EmployeeSid { get; set; }


        [Display(Name = "員工帳號")]
        public string Account { get; set; }
        [Display(Name = "員工密碼")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
