﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CouponStatus
{
    public int CouponStatusNo { get; set; }

    public string Status { get; set; }

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}