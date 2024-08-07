using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class LogicalDeletion
{
    public int LogicalDeletionNo { get; set; }

    public string? DeletionStatus { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
