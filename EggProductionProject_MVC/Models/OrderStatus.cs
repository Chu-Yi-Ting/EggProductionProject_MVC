using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class OrderStatuses
{
    public int OrderStatusNo { get; set; }

    public string? OrderStatus { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
