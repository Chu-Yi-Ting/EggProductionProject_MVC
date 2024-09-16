namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
    public class ProductViewModel
    {
        public int productSid { get; set; }
        public int subcategoryNo { get; set; }
		public string? subcategoryName { get; set; }
		public int? itemNo { get; set; }
		public string? itemName { get; set; }
		public string? productImagePath { get; set; }
        public string? productName { get; set; }
        public string? description { get; set; }
		public decimal? price { get; set; }
        public int? stock { get; set; }
		public int? quantity { get; set; }
		public int? weight { get; set; }
		public string? component { get; set; }
		public decimal? discountPercent { get; set; }
		public DateTime? launchTime { get; set; }
		public int? publicStatusNo { get; set; }

        // 接受上傳圖片的參數
        public IFormFile? productImage { get; set; }

        public IFormFile? croppedImage { get; set; }

        // 新增這個屬性來存放所有產品
        public List<ProductViewModel>? products { get; set; }
	}
}