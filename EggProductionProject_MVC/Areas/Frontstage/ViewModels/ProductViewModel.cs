namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
    internal class ProductViewModel
    {
        public int productSid { get; set; }
        public string? productImagePath { get; set; }
        public string? productName { get; set; }
        public decimal? price { get; set; }
    }
}