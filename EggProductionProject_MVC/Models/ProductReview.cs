﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ProductReview
{
    public int ReviewSid { get; set; }

    public int? ProductSid { get; set; }

    public int? MemberSid { get; set; }

    public int? Score { get; set; }

    public string ReviewContent { get; set; }

    public virtual Product ProductS { get; set; }
}