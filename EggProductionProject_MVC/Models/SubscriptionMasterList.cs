﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class SubscriptionMasterList
{
    public int SubscriptionSid { get; set; }

    public int? CreatorMemberNo { get; set; }

    public int? SubscriberMemberNo { get; set; }

    public virtual ICollection<Creator> Creators { get; set; } = new List<Creator>();
}
