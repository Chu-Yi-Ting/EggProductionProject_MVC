﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class TrackStatus
{
    public int TrackStatusNo { get; set; }

    public string TrackStatus1 { get; set; }

    public int? IsHomeGet { get; set; }

    public virtual ICollection<TrackTime> TrackTimes { get; set; } = new List<TrackTime>();
}