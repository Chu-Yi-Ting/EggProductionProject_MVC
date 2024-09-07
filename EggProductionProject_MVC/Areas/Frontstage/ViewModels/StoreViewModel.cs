using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Frontstage.ViewModels
{
	public class StoreViewModel
	{
		public int storeSid { get; set; }
		public int? memberSid { get; set; }
		public string? storeImagePath { get; set; }
		public IFormFile? storeImage { get; set; }
		public string? storeName { get; set; }
		public DateOnly? establishDate { get; set; }
		public string? storeIntroduction { get; set; }
		public int? productCount { get; set; }
		
	}
}