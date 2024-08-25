using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CarrierWay
{
    public int CarrierWayNo { get; set; }

    public int? CarrierNo { get; set; }

    public string? Way { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<CarrierAddress> CarrierAddresses { get; set; } = new List<CarrierAddress>();

    public virtual Carrier? CarrierNoNavigation { get; set; }
}
