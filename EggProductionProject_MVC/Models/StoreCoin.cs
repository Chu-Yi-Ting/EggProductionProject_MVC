﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class StoreCoin
{
    public int StoreCoinSid { get; set; }

    public int? MemberSid { get; set; }

    public int? CoinUseAreaNo { get; set; }

    public int? AreaSid { get; set; }

    public int? IsPositive { get; set; }

    public decimal? Money { get; set; }

    public DateTime? ChangeTime { get; set; }

    public virtual Order? AreaS { get; set; }

    public virtual CoinUseAreas? CoinUseAreaNoNavigation { get; set; }

    public virtual Member? MemberS { get; set; }
}
