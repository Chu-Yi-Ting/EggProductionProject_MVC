﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Nature
{
    public int NatureSid { get; set; }

    public string? ViedoNature { get; set; }

    public virtual ICollection<VideoSummary> VideoSummaries { get; set; } = new List<VideoSummary>();
}
