﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class SalesStatus
{
    public int RunningStatusNo { get; set; }

    public string RunningStatusDescription { get; set; }

    public virtual ICollection<SalesBatch> SalesBatches { get; set; } = new List<SalesBatch>();
}