namespace EggProductionProject_MVC.Areas.Frontstage.DTO
{
    public class StoreQueryDTO
    {
        public int storeSid { get; set; }
        public int page { get; set; } = 1; // 預設為第1頁
        public int pageSize { get; set; } = 2; // 預設每頁9個商品
    }
}