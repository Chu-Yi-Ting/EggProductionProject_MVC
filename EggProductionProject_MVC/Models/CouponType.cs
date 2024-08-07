using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CouponType
{
    public int CouponTypeNo { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public decimal? Minimum { get; set; }

    public int? PublicStatusNo { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? UseAlone { get; set; }

    public int? EmployeeSid { get; set; }

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual PublicStatus? PublicStatusNoNavigation { get; set; }
}
