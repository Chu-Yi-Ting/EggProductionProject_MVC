using EggProductionProject_MVC.Models;
using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Areas.Backstage.ViewModels;

public class CouponTypeViewModels
{


    public int CouponTypeNo { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public decimal? Minimum { get; set; }

    public int? PublicStatusNo { get; set; }
    public string? StatusDescription { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? UseAlone { get; set; }

    public string? EmpName { get; set; }
}
