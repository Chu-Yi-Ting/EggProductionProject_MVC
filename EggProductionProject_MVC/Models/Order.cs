﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Order
{
    public int OrderSid { get; set; }

    public string? OrderNo { get; set; }

    public DateTime? OrderCreatedTime { get; set; }

    public int? MemberSid { get; set; }

    public int? PaymentNo { get; set; }

    public int? AlreadyPaid { get; set; }

    public int? OrderStatusNo { get; set; }

    public int? CouponSid { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Coupon? CouponS { get; set; }

    public virtual Member? MemberS { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatuses? OrderStatusNoNavigation { get; set; }

    public virtual Payments? PaymentNoNavigation { get; set; }

    public virtual ICollection<StoreCoin> StoreCoins { get; set; } = new List<StoreCoin>();

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
