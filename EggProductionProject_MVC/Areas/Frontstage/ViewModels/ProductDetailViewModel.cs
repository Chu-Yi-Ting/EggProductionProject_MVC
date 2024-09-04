﻿namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
    internal class ProductDetailViewModel
    {
        public int? productSid { get; set; }
        public string? productImagePath { get; set; }
        public string? productName { get; set; }
        public int? stock { get; set; }
        public decimal? price { get; set; }        
        public string? description { get; set; }
        public string? origin { get; set; }
        public int? quanitity { get; set; }
        public int? weight { get; set; }
        public string? component { get; set; }
        public DateOnly? launchTime { get; set; }
        public StoreViewModel? storeInfo { get; set; }

    }

}