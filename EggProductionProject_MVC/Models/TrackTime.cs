using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class TrackTime
{
    public int TrackTimeSid { get; set; }

    public int? TrackSid { get; set; }

    public int? TrackStatusNo { get; set; }

    public DateTime? CreatedTime { get; set; }

    public virtual Track? TrackS { get; set; }

    public virtual TrackStatus? TrackStatusNoNavigation { get; set; }
}
