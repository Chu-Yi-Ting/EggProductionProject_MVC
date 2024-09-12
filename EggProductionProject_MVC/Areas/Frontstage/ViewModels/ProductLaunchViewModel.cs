namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
	public class ProductLaunchViewModel
	{
		public List<ProductViewModel>? CurrentProducts { get; set; } // 當前上架的商品
		public List<ProductViewModel>? ReviewingProducts { get; set; } // 審核中的商品
	}
}