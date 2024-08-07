using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Edit
{
    public int EditSid { get; set; }

    public int? ArticleSid { get; set; }

    public int? ReplySid { get; set; }

    public string? EditBefore { get; set; }

    public string? EditAfter { get; set; }

    public DateOnly? EditTime { get; set; }

    public virtual Article? ArticleS { get; set; }

    public virtual Reply? ReplyS { get; set; }
}
