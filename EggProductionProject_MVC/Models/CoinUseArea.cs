﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CoinUseArea
{
    public int CoinUseAreaNo { get; set; }

    public string CoinUseArea1 { get; set; }

    public virtual ICollection<StoreCoin> StoreCoins { get; set; } = new List<StoreCoin>();
}