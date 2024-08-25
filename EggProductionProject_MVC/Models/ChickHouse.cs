﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ChickHouse
{
    public int HouseSid { get; set; }

    public int? AreaSid { get; set; }

    public int? MemberSid { get; set; }

    public int? SubcategoryNo { get; set; }

    public string? HouseName { get; set; }

    public int? ChickAmount { get; set; }

    public DateOnly? InsertDate { get; set; }

    public int? WeekNum { get; set; }

    public virtual MemberArea? AreaS { get; set; }

    public virtual ICollection<ChickDeath> ChickDeaths { get; set; } = new List<ChickDeath>();

    public virtual ICollection<ChickLotNo> ChickLotNos { get; set; } = new List<ChickLotNo>();

    public virtual ICollection<DailyEggRe> DailyEggRes { get; set; } = new List<DailyEggRe>();

    public virtual ICollection<Manure> Manures { get; set; } = new List<Manure>();

    public virtual ProductSubcategory? SubcategoryNoNavigation { get; set; }
}
