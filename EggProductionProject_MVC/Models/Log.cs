﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Log
{
    public int LogSid { get; set; }

    public int? MemberSid { get; set; }

    public byte? IsLogSuccess { get; set; }

    public DateTime? LogTime { get; set; }

    public virtual Member? MemberS { get; set; }
}
