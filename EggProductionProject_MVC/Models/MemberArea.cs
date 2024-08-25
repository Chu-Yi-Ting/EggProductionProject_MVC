using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class MemberArea
{
    public int AreaSid { get; set; }

    public int MemberSid { get; set; }

    public string MemberName { get; set; } = null!;

    public string MemberAddress { get; set; } = null!;

    public virtual ICollection<AreaFeed> AreaFeeds { get; set; } = new List<AreaFeed>();

    public virtual ICollection<ChickHouse> ChickHouses { get; set; } = new List<ChickHouse>();

    public virtual Member MemberS { get; set; } = null!;
}
