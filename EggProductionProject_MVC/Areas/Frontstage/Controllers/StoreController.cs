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
            var storeInfo = _context.Stores
                .Where(s => s.StoreSid == storeSid)
                .Select(s => new StoreViewModel
                {
                    storeSid = s.StoreSid,
                    memberSid = s.MemberSid,
                    storeImagePath = s.StoreImagePath,
                    storeName = s.Company,
                    establishDate = s.EstablishDate,
                    storeIntroduction = s.StoreIntroduction,
                    productCount = _context.Products.Where(p => p.StoreSid == s.StoreSid).Count()
                })
                .FirstOrDefault();

            if (storeInfo == null)
            {
                return NotFound();
            }

            // 查詢該賣家的所有商品
            var products = _context.Products
                .Where(p => p.StoreSid == storeSid)
                .Select(p => new ProductViewModel
                {
                    productSid = p.ProductSid,
                    productImagePath = _context.ProductImages
                        .Where(img => img.ProductSid == p.ProductSid)
                        .Select(img => img.ProductImagePath)
                        .FirstOrDefault(),
                    productName = p.ProductName,
                    price = p.Price
                })
                .ToList();

            // 組合商品及賣家資訊
            var ProductInStore = new ProductAndStoreViewModel
            {
                storeInfo = storeInfo,
                products = products
            };

            return View(ProductInStore);
        }

        //SellerInformation動作函式生賣家中心基本資料畫面
        public IActionResult SellerInformation()
        {
            return View();
        }

        public IActionResult ProductLaunch()
        {
            return View();
        }


    }
}
