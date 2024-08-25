﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class FriendActionType
{
    public int ActionTypeSid { get; set; }

    public string? ActionType { get; set; }

    public virtual ICollection<FriendAction> FriendActions { get; set; } = new List<FriendAction>();
}
