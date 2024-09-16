using EggProductionProject_MVC.Areas.Frontstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class ProductController : Controller
    {
        private readonly EggPlatformContext _context;

        public ProductController(EggPlatformContext context)
        {
            _context = context;
        }

        public IActionResult OnlineShop()
        {
            ViewData["Title"] = "Good EGG商城購物";
            return View();
        }

		//商品詳細資料包含賣場資訊的分頁
		[HttpGet]
		public IActionResult ProductDetail(int productSid)
		{
			ViewData["Title"] = "商品分頁";
            // 查詢商品資料
            var product = _context.Products
				.Where(p => p.ProductSid == productSid)
				.Select(p => new ProductDetailViewModel
				{
					productSid = p.ProductSid,
					productImagePath = _context.ProductImages
						.Where(img => img.ProductSid == p.ProductSid)
						.Select(img => img.ProductImagePath)
						.FirstOrDefault(),
					productName = p.ProductName,
					stock = p.Stock,
					price = p.Price,
					description = p.Description,
					origin = p.Origin,
					quanitity = p.Quanitity,
					weight = p.Weight,
					component = p.Component,
					launchTime = p.LaunchTime,
                    discountPercent = p.DiscountPercent,

                    storeInfo = _context.Stores
						.Where(s => s.StoreSid == p.StoreSid)
						.Select(s => new StoreViewModel
						{
							storeSid = s.StoreSid,
							memberSid = s.MemberSid,
                            storeImagePath = s.StoreImagePath,
							storeName = s.Company,
							establishDate = s.EstablishDate,
							storeIntroduction = s.StoreIntroduction,
							productCount = _context.Products.Where(p => p.StoreSid == s.StoreSid).Count()
						}).FirstOrDefault()
				}).FirstOrDefault();

			if (product == null)
			{
				return NotFound();
			}

			return View(product); // 傳遞模型到視圖
		}

	}
}
