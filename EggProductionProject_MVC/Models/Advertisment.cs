using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Advertisment
{
    public int AdvertismentSid { get; set; }

    public int? StoreSid { get; set; }

    public int? VideoSid { get; set; }

    public string? AdContent { get; set; }

    public DateTime? UploadTime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? AdBehavior { get; set; }

    public int? PublicStatusNo { get; set; }

    public virtual PublicStatus? PublicStatusNoNavigation { get; set; }

    public virtual Store? StoreS { get; set; }

    public virtual VideoSummary? VideoS { get; set; }
}
