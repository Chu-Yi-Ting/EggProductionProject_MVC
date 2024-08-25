using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Manure
{
    public int ManureSid { get; set; }

    public int HouseSid { get; set; }

    public decimal Weight { get; set; }

    public decimal Cost { get; set; }

    public DateOnly Date { get; set; }

    public virtual ChickHouse HouseS { get; set; } = null!;
}
