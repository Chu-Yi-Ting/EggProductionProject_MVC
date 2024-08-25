using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Collect
{
    public int CollectSid { get; set; }

    public int? WebSiteTypeSid { get; set; }

    public int? CollectObjectTypeSid { get; set; }

    public int? CollectorSid { get; set; }

    public virtual WebSiteType? WebSiteTypeS { get; set; }
}
