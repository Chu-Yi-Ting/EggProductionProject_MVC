﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Product
{
    public int ProductSid { get; set; }

    public string ProductNo { get; set; }

    public string ProductName { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public int? SubcategoryNo { get; set; }

    public int? ItemNo { get; set; }

    public int? StoreSid { get; set; }

    public string Description { get; set; }

    public string Origin { get; set; }

    public int? Quanitity { get; set; }

    public int? Weight { get; set; }

    public string Component { get; set; }

    public DateOnly? LaunchTime { get; set; }

    public int? PublicStatusNo { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<EcImage> EcImages { get; set; } = new List<EcImage>();

    public virtual ICollection<FlashSale> FlashSales { get; set; } = new List<FlashSale>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual PublicStatus PublicStatusNoNavigation { get; set; }

    public virtual Store StoreS { get; set; }

    public virtual ProductSubcategory SubcategoryNoNavigation { get; set; }
}