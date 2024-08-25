using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Friend
{
    public int FriendSid { get; set; }

    public int? MemberSid { get; set; }

    public int? MemberSid2 { get; set; }

    public DateOnly? DateAdded { get; set; }

    public virtual ICollection<FriendAction> FriendActions { get; set; } = new List<FriendAction>();

    public virtual Member? MemberS { get; set; }

    public virtual ICollection<Talk> Talks { get; set; } = new List<Talk>();
}
