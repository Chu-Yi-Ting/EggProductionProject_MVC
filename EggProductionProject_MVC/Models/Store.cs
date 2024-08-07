﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Store
{
    public int StoreSid { get; set; }

    public int? MemberSid { get; set; }

    public string? Company { get; set; }

    public DateOnly? EstablishDate { get; set; }

    public byte[]? StoreImg { get; set; }

    public string? StoreIntroduction { get; set; }

    public int? LogicalDeletionNo { get; set; }

    public virtual ICollection<Advertisment> Advertisments { get; set; } = new List<Advertisment>();

    public virtual ICollection<CarrierOpen> CarrierOpens { get; set; } = new List<CarrierOpen>();

    public virtual ICollection<EcImage> EcImages { get; set; } = new List<EcImage>();

    public virtual LogicalDeletion? LogicalDeletionNoNavigation { get; set; }

    public virtual Member? MemberS { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
