﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Coupon
{
    public int CouponSid { get; set; }

    public int? CouponTypeNo { get; set; }

    public int? CouponStatusNo { get; set; }

    public DateTime? CollectionTime { get; set; }

    public int? MemberSid { get; set; }

    public virtual CouponStatus? CouponStatusNoNavigation { get; set; }

    public virtual CouponType? CouponTypeNoNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
