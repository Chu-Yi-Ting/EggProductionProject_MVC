using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class NotifyType
{
    public int NotifyTypeSid { get; set; }

    public string? NotifyType1 { get; set; }

    public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();
}
