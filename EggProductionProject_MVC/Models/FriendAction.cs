using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class FriendAction
{
    public int FriendActionSid { get; set; }

    public int ActionUserid { get; set; }

    public int ActionRecieverSid { get; set; }

    public int? ActionTypeSid { get; set; }

    public DateOnly? ActionDate { get; set; }

    public int? FriendSid { get; set; }

    public virtual FriendActionType? ActionTypeS { get; set; }

    public virtual Friend? FriendS { get; set; }
}
