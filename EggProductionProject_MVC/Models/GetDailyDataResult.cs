﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EggProductionProject_MVC.Models
{
    public partial class GetDailyDataResult
    {
        public string 生產總類 { get; set; }
        public string Date { get; set; }
        public double? ActualEggRate { get; set; }
        public double? NormalizedEggRate { get; set; }
    }
}
