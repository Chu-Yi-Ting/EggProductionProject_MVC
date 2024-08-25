using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class PublicStatus
{
    public int PublicStatusNo { get; set; }

    public string? StatusDescription { get; set; }

    public virtual ICollection<Advertisment> Advertisments { get; set; } = new List<Advertisment>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<CouponType> CouponTypes { get; set; } = new List<CouponType>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<VideoSummary> VideoSummaries { get; set; } = new List<VideoSummary>();
}
