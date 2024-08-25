﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ArticleCategory
{
    public int ArticleCategoriesSid { get; set; }

    public string? ArticleCategories { get; set; }

    public byte[]? ArticleCategoriesImg { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
