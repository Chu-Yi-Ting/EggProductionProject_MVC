using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Cart
{
    public int CartSid { get; set; }

    public int? MemberSid { get; set; }

    public int? ProductSid { get; set; }

    public int? Qty { get; set; }

    public virtual Member? MemberS { get; set; }

    public virtual Product? ProductS { get; set; }
}
