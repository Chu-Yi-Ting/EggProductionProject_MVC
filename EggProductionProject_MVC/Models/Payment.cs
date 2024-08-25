using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Payment
{
    public int PaymentNo { get; set; }

    public string? Pay { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
