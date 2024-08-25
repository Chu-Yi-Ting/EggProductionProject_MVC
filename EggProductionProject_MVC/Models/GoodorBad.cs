using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class GoodorBad
{
    public int GorBsid { get; set; }

    public int? MemberNo { get; set; }

    public int? ArticleSid { get; set; }

    public int? ReplySid { get; set; }

    public DateOnly? GorBdate { get; set; }

    public int GorBtype { get; set; }

    public virtual Article? ArticleS { get; set; }

    public virtual Member? MemberNoNavigation { get; set; }

    public virtual Reply? ReplyS { get; set; }
}
