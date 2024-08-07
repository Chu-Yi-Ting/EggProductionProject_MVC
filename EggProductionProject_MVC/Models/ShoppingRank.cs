using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ShoppingRank
{
    public int ShoppingRankNo { get; set; }

    public string? ShoppingRank1 { get; set; }

    public byte[]? Eggimage { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
