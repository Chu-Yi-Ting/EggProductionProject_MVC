﻿namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
	public class ProductLaunchViewModel
	{
		public List<ProductViewModel>? CurrentProducts { get; set; } // 當前上架的商品
		public List<ProductViewModel>? ReviewingProducts { get; set; } // 審核中的商品
        public bool IsStoreSetup { get; set; } // 是否已填寫賣場資料
        public int productSid { get; set; } // 新增的 productSid 屬性

    }
}