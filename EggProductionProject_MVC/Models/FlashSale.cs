using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class FlashSale
{
    public int DiscountSid { get; set; }

    public int? PeriodSid { get; set; }

    public int? ProductSid { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual SalesBatch? PeriodS { get; set; }

    public virtual Product? ProductS { get; set; }
}
