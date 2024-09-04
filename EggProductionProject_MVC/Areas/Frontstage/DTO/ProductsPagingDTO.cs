using EggProductionProject_MVC.Areas.Frontstage.Models;

namespace EggProductionProject_MVC.Areas.Frontstage.DTO
{
    public class ProductsPagingDTO
    {
        public int TotalPages { get; set; }

        //定義了一個公開的屬性 ProductsResult，它是一個 List<SpotImagesSpot> 類型的列表。
        //SpotImagesSpot 是從 AjaxDemo.Models 命名空間中引入的模型類別，表示每一筆景點資料。
        //這個屬性用來存儲當前頁的景點資料列表。
        public List<ProductWithImagesDTO>? ProductsResult { get; set; }
    }
}
