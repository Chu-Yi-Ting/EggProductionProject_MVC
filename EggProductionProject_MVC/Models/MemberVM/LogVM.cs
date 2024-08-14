using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.Models.MemberVM
{
    public class LogVM
    {
        public int LogSid { get; set; }
        [Display(Name = "會員名稱")]
        public string MemberName { get; set; }
        [Display(Name = "登入嘗試")]
        public byte? IsLogSuccess { get; set; }
        [Display(Name = "登入時間")]
        public DateTime? LogTime { get; set; }

        public virtual Member MemberS { get; set; }
    }
}
