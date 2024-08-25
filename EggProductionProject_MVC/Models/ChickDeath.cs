﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ChickDeath
{
    public int ChickDeathSid { get; set; }

    public int? HouseSid { get; set; }

    public int? DeathAmount { get; set; }

    public DateOnly? Date { get; set; }

    public virtual ChickHouse? HouseS { get; set; }
}
