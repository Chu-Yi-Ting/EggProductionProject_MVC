using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Article
{
    public int ArticleSid { get; set; }

    public int? ArticleCreaterSid { get; set; }

    public string? ArticleTitle { get; set; }

    public string ArticleInfo { get; set; } = null!;

    public int? ArticleCategoriesSid { get; set; }

    public DateTime? ArticleDate { get; set; }

    public DateTime? ArticleUpdate { get; set; }

    public int? EditCountTimes { get; set; }

    public int? TagMemberNo { get; set; }

    public int? PublicStatusNo { get; set; }

    public bool? DeleteOrNot { get; set; }

    public virtual ArticleCategory? ArticleCategoriesS { get; set; }

    public virtual Member? ArticleCreaterS { get; set; }

    public virtual ICollection<Edit> Edits { get; set; } = new List<Edit>();

    public virtual ICollection<GoodorBad> GoodorBads { get; set; } = new List<GoodorBad>();

    public virtual PublicStatus? PublicStatusNoNavigation { get; set; }

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
}
