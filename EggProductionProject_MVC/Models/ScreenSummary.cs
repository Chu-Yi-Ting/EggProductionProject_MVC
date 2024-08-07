using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ScreenSummary
{
    public int ScreenTextSid { get; set; }

    public string? ScreenTextCategory { get; set; }

    public virtual ICollection<VideoSummary> VideoSummaries { get; set; } = new List<VideoSummary>();
}
