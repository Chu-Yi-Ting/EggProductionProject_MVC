namespace EggProductionProject_MVC.Areas.Frontstage.DTO
{
    public class ProductWithImagesDTO
    {
        public int productSid { get; set; }
        public string? productNo { get; set; }
        public string? productName { get; set; }
        public decimal? price { get; set; }
        public int? stock { get; set; }
        public int? subcategoryNo { get; set; }
        public int? itemNo { get; set; }
        public int? storeSid { get; set; }
        public string? description { get; set; }
        public string? origin { get; set; }
        public int? quantity { get; set; }
        public DateTime? launchTime { get; set; }
        public string? productImagePath { get; set; }
        public string? imageDescription { get; set; }
        public DateTime? uploadTime { get; set; }
    }
}