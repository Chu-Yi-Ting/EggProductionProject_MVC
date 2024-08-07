using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class DailyEggRe
{
    public int EggSid { get; set; }

    public int HouseSid { get; set; }

    public int MemberSid { get; set; }

    public int? EggAmount { get; set; }

    public int? UnQamount { get; set; }

    public int SubcategoryNo { get; set; }

    public DateOnly Date { get; set; }

    public virtual ChickHouse HouseS { get; set; } = null!;
}
