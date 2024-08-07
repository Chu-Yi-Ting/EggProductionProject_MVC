using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class OrderStatus
{
    public int OrderStatusNo { get; set; }

    public string? OrderStatus1 { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
