using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Notify
{
    public int NotifySid { get; set; }

    public int? NotifyRecieverid { get; set; }

    public int? NotifySenderid { get; set; }

    public int? WebSiteTypeSid { get; set; }

    public int? NotifyTypeSid { get; set; }

    public TimeOnly? NotifyTime { get; set; }

    public virtual Member? NotifyReciever { get; set; }

    public virtual NotifyType? NotifyTypeS { get; set; }

    public virtual WebSiteType? WebSiteTypeS { get; set; }
}
