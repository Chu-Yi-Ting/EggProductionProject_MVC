﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class WebSiteType
{
    public int WebSiteTypeSid { get; set; }

    public string WebSiteType1 { get; set; }

    public virtual ICollection<Collect> Collects { get; set; } = new List<Collect>();

    public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();
}