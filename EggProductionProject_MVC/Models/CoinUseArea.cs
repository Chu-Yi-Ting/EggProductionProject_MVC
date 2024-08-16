using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class CoinUseAreas
{
    public int CoinUseAreaNo { get; set; }

    public string? CoinUseArea { get; set; }

    public virtual ICollection<StoreCoin> StoreCoins { get; set; } = new List<StoreCoin>();
}
