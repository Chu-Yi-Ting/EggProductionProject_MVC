using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ProductSubcategory
{
    public int SubcategoryNo { get; set; }

    public int? ProductCategoryNo { get; set; }

    public string? SubcategoryName { get; set; }

    public virtual ICollection<ChickHouse> ChickHouses { get; set; } = new List<ChickHouse>();

    public virtual ProductCategory? ProductCategoryNoNavigation { get; set; }

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
