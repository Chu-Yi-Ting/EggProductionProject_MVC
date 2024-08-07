﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class EcImage
{
    public int EcImgSid { get; set; }

    public byte[]? EcImg { get; set; }

    public int? ProductSid { get; set; }

    public int? StoreSid { get; set; }

    public virtual Product? ProductS { get; set; }

    public virtual Store? StoreS { get; set; }
}
