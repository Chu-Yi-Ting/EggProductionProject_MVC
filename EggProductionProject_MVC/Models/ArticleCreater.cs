using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ArticleCreater
{
    public int ArticleCreaterSid { get; set; }

    public int? MemberNo { get; set; }

    public string? PersonalInfo { get; set; }

    public int? ArticleCount { get; set; }
}
