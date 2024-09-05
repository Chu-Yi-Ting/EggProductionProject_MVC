﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Reply
{
    public int ReplySid { get; set; }

    public int? ArticleSid { get; set; }

    public int? ArticleCreaterSid { get; set; }

    public string ReplyInfo { get; set; } = null!;

    public DateTime? ReplyDate { get; set; }

    public DateTime? ReplyUpdate { get; set; }

    public int? EditTimes { get; set; }

    public int? TagMemberNo { get; set; }

    public int? PublicStatusNo { get; set; }

    public bool? DeleteOrNot { get; set; }

    public virtual Member? ArticleCreatorS { get; set; }

    public virtual Article? ArticleS { get; set; }

    public virtual ICollection<Edit> Edits { get; set; } = new List<Edit>();

    public virtual ICollection<GoodorBad> GoodorBads { get; set; } = new List<GoodorBad>();

    public virtual PublicStatus? PublicStatusNoNavigation { get; set; }
}
