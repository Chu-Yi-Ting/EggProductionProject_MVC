using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
    public class CombinedRecord
    {
        public int HouseSid { get; set; }
        public int MemberSid { get; set; } // 可選的，只有 DailyEggRate 需要
        public int? EggAmount { get; set; } // 只有 DailyEggRate 需要
        public int? UnQAmount { get; set; } // 只有 DailyEggRate 需要
        public int? DeathAmount { get; set; } // 只有 ChickDeath 需要
        public DateTime Date { get; set; }
    }
    public partial class CombinedRecord2
    {

        public int AreaSid { get; set; }

        public int MemberSid { get; set; }

        public string LotNo { get; set; }

        public decimal Weight { get; set; }

        public DateTime Date { get; set; }

        public decimal Cost { get; set; }

    }
}
