using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ProductCategory
{
    public int ProductCategoryNo { get; set; }

    public string? ProductCategoryName { get; set; }

    public virtual ICollection<ProductSubcategory> ProductSubcategories { get; set; } = new List<ProductSubcategory>();
}
