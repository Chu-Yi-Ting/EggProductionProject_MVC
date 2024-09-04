using EggProductionProject_MVC.Areas.Frontstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class StoreController : Controller
    {
        private readonly EggPlatformContext _context;

        public StoreController(EggPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Store(int storeSid)
        {            
            // 查詢商品以及賣家資訊
            var productsAndStore = _context.Products
                .Where(p => p.StoreSid == storeSid)
                .Select(p => new ProductAndStoreViewModel
                {
                    productSid = p.ProductSid,
                    productImagePath = _context.ProductImages
                        .Where(img => img.ProductSid == p.ProductSid)
                        .Select(img => img.ProductImagePath)
                        .FirstOrDefault(),
                    productName = p.ProductName,                   
                    price = p.Price,                                                    
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

            if (productsAndStore == null)
            {
                return NotFound();
            }
            return View(productsAndStore);
        }
    }
}
