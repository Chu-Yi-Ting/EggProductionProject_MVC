namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
    public class ProductAndStoreViewModel
    {
        public StoreViewModel? storeInfo { get; set; }
        public List<ProductViewModel>? products { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalPages { get; set; }
	}
}