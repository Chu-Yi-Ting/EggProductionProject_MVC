using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class OrderDetail
{
    public int OrderDetailSid { get; set; }

    public int? OrderSid { get; set; }

    public int? ProductSid { get; set; }

    public decimal? BuyPrice { get; set; }

    public int? Qty { get; set; }

    public virtual Order? OrderS { get; set; }

    public virtual Product? ProductS { get; set; }
}
