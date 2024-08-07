using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Track
{
    public int TrackSid { get; set; }

    public int? OrderSid { get; set; }

    public string? TrackingNum { get; set; }

    public int? SendSouceSid { get; set; }

    public int? ReceiveSourceSid { get; set; }

    public virtual Order? OrderS { get; set; }

    public virtual CarrierAddress? ReceiveSourceS { get; set; }

    public virtual CarrierAddress? SendSouceS { get; set; }

    public virtual ICollection<TrackTime> TrackTimes { get; set; } = new List<TrackTime>();
}
