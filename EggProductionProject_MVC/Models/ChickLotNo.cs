using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ChickLotNo
{
    public int ChickSid { get; set; }

    public int HouseSid { get; set; }

    public string? ChickLotNo1 { get; set; }

    public int? ChickAmount { get; set; }

    public decimal? Cost { get; set; }

    public DateOnly? Date { get; set; }

    public int? WeekNum { get; set; }

    public virtual ChickHouse HouseS { get; set; } = null!;

    public virtual ICollection<Vaccinate> Vaccinates { get; set; } = new List<Vaccinate>();
}
