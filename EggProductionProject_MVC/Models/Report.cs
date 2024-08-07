using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Report
{
    public int ReportSid { get; set; }

    public int? WebSiteTypeSid { get; set; }

    public int? ReportObjectSid { get; set; }

    public DateOnly? ReportDate { get; set; }

    public int? ReporterSid { get; set; }

    public int? ReportReasonSid { get; set; }

    public virtual ReportReason? ReportReasonS { get; set; }

    public virtual WebSiteType? WebSiteTypeS { get; set; }
}
