using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CarrierOpen
{
    public int CarrierOpenNo { get; set; }

    public int? StoreSid { get; set; }

    public int? StoreOpen { get; set; }

    public int? HouseOpen { get; set; }

    public virtual Store? StoreS { get; set; }
}
