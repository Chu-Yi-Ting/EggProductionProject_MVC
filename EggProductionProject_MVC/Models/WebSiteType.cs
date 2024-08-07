using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class WebSiteType
{
    public int WebSiteTypeSid { get; set; }

    public string? WebSiteType1 { get; set; }

    public virtual ICollection<Collect> Collects { get; set; } = new List<Collect>();

    public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();
}
