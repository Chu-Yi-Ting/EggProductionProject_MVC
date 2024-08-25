﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Creator
{
    public int CreatorSid { get; set; }

    public int? MemberSid { get; set; }

    public string? MemberName { get; set; }

    public string? PersonalProfile { get; set; }

    public int? SubscriptionSid { get; set; }

    public byte[]? Image { get; set; }

    public virtual Member? MemberS { get; set; }

    public virtual SubscriptionMasterList? SubscriptionS { get; set; }

    public virtual ICollection<VideoSummary> VideoSummaries { get; set; } = new List<VideoSummary>();
}
