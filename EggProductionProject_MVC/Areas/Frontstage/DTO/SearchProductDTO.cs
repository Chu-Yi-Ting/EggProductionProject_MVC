namespace EggProductionProject_MVC.Areas.Frontstage.DTO
{
    public class SearchProductDTO
    {
        public int? productSid { get; set; }
        public int? subcategoryNo { get; set; } = 0;
        public string? keyword { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 9;
        public string? sortType { get; set; } = "desc";
        public string? sortBy { get; set; }
        public int? minPrice { get; set; } = 0;
        public int? maxPrice { get; set; } = 1000;

    }

}
