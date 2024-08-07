﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ProductItem
{
    public int ItemNo { get; set; }

    public int? SubcategoryNo { get; set; }

    public string? ItemName { get; set; }

    public virtual ProductSubcategory? SubcategoryNoNavigation { get; set; }
}
