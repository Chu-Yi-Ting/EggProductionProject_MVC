﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class ProductImage
{
    public int ProductImageSid { get; set; }

    public int? ProductSid { get; set; }

    public string ProductImagePath { get; set; }

    public string ImageDescription { get; set; }

    public DateTime? UploadTime { get; set; }

    public virtual Product ProductS { get; set; }
}