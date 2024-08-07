﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class SalesBatch
{
    public int PeriodSid { get; set; }

    public DateOnly? PeriodDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? RunningStatus { get; set; }

    public virtual ICollection<FlashSale> FlashSales { get; set; } = new List<FlashSale>();
}
