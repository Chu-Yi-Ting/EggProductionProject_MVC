using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CarrierAddress
{
    public int CarrierAddressSid { get; set; }

    public int? MemberSid { get; set; }

    public int? StoreUsed { get; set; }

    public string? RecordName { get; set; }

    public string? RecordPhone { get; set; }

    public int? CarrierWayNo { get; set; }

    public string? Adress { get; set; }

    public string? StoreId { get; set; }

    public string? StoreName { get; set; }

    public int? PublicStatusNo { get; set; }

    public virtual CarrierWays? CarrierWayNoNavigation { get; set; }

    public virtual Member? MemberS { get; set; }

    public virtual ICollection<Track> TrackReceiveSources { get; set; } = new List<Track>();

    public virtual ICollection<Track> TrackSendSouces { get; set; } = new List<Track>();
}
