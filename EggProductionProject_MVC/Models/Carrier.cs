using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Carrier
{
    public int CarrierNo { get; set; }

    public string? CarrierCode { get; set; }

    public string? CarrierName { get; set; }

    public virtual ICollection<CarrierWays> CarrierWays { get; set; } = new List<CarrierWays>();
}
