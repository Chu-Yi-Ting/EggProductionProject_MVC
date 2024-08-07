using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Talk
{
    public int TalkSid { get; set; }

    public int? SenderSid { get; set; }

    public int? RecieverSid { get; set; }

    public string? MessageContent { get; set; }

    public TimeOnly? TimeStamp { get; set; }

    public byte? IsRead { get; set; }

    public byte[]? Attachment { get; set; }

    public int? FriendSid { get; set; }

    public virtual Friend? FriendS { get; set; }
}
