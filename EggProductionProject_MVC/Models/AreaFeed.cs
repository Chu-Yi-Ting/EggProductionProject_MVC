﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class AreaFeed
{
    public int FeedSid { get; set; }

    public int AreaSid { get; set; }

    public int MemberSid { get; set; }

    public string LotNo { get; set; }

    public decimal Weight { get; set; }

    public DateOnly Date { get; set; }

    public decimal Cost { get; set; }

    public virtual MemberArea AreaS { get; set; }
}