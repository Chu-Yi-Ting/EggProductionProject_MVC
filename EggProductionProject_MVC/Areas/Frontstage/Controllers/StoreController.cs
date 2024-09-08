using EggProductionProject_MVC.Areas.Frontstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class StoreController : Controller
    {
        private readonly EggPlatformContext _context;
		private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;  // 使用Identity User Manager

		public StoreController(EggPlatformContext context, IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
			_hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        //查詢賣家資訊及賣家的商品
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
			// 獲取當前登入用戶的AspUserId
			var aspUserId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(aspUserId))
			{
				return RedirectToAction("Login", "Account", new { area = "Identity" });
			}

			// 使用 aspUserId 查詢 Member 表中的 MemberSid
			var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
			if (member == null)
			{
				// 如果查詢不到會員，跳轉到登入頁面
				return RedirectToAction("Login", "Account", new { area = "Identity" });
			}
			
            // 查詢賣場資訊，如果該會員已有賣場，則載入賣場資訊
			var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

			// 傳遞會員資料和賣家資訊到前端頁面
			var seller = new StoreViewModel
			{
                storeSid = store?.StoreSid ?? 0,  // 若無賣場則為 0
                memberSid = member.MemberSid,
				storeName = store?.Company,  // 若無賣場資料則為 null
				storeIntroduction = store?.StoreIntroduction,
				storeImagePath = store?.StoreImagePath,  // 若無則為 null
				establishDate = store?.EstablishDate  // 若無則為 null
			};

			return View(seller);
            
        }

		[HttpPost]
		public IActionResult SellerInformation(StoreViewModel model)
		{
			// 1. 取得當前已登入的用戶，並根據 AspUserId 取得對應的 Member
			var aspUserId = _userManager.GetUserId(User);
			var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
			if (member == null)
			{
				return RedirectToAction("Login", "Account");
			}

            // 2. 檢查是否已經存在該會員的賣場，若無則創建新的 Store 資料
            var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

            // 3. 處理圖片上傳
            string storeImagePath = store?.StoreImagePath; // 保留舊圖片路徑
            if (model.storeImage != null && model.storeImage.Length > 0)
            {
                // 3.1 刪除舊的圖片
                if (!string.IsNullOrEmpty(storeImagePath))
                {
                    var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, storeImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // 3.2 上傳新的圖片
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", "Stores");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.storeImage.FileName);
                var newImagePath = Path.Combine(uploadPath, uniqueFileName);
                storeImagePath = $"/images/stores/{uniqueFileName}";  // 將相對路徑存入資料庫

                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    model.storeImage.CopyTo(stream);
                }
            }
           
			if (store == null)
			{
				// 創建新賣場資料
				store = new Store
				{
					MemberSid = member.MemberSid,
					Company = model.storeName,  // 賣場名稱
					StoreIntroduction = model.storeIntroduction,  // 賣場簡介
					StoreImagePath = storeImagePath,  // 圖片路徑
					EstablishDate = DateOnly.FromDateTime(DateTime.Now),   // 設定創建時間
                    PublicStatusNo = 1  // 設定賣場公開狀態為 1（公開）
                };

				_context.Stores.Add(store);  // 將新賣場資料加入資料庫
			}
			else
			{
				// 更新現有賣場資料
				store.Company = model.storeName;  // 更新賣場名稱
				store.StoreIntroduction = model.storeIntroduction;  // 更新賣場簡介
				store.StoreImagePath = storeImagePath;  // 更新圖片路徑
			}

			_context.SaveChanges();  // 儲存變更到資料庫

			return RedirectToAction("SellerInformation");
		}

		//ProductLaunch動作函式生賣家中心商品上架畫面
		public IActionResult ProductLaunch()
        {
			// 查詢當前登入用戶的 StoreSid
			var aspUserId = _userManager.GetUserId(User);
			var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
			var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

			var model = new ProductLaunchViewModel();

			if (store != null)
			{
				// 查詢該賣家的當前上架產品 (PublicStatusNo = 1)
				model.CurrentProducts = _context.Products
					.Where(p => p.PublicStatusNo == 1 && p.StoreSid == store.StoreSid)
					.Select(p => new ProductViewModel
					{
						productSid = p.ProductSid,
						productName = p.ProductName,
						price = p.Price,
						stock = p.Stock,
						launchTime = p.LaunchTime,
						publicStatusNo = p.PublicStatusNo
					})
					.ToList();

				// 查詢該賣家的審核中產品 (PublicStatusNo = 2)
				model.ReviewingProducts = _context.Products
					.Where(p => p.PublicStatusNo == 2 && p.StoreSid == store.StoreSid)
					.Select(p => new ProductViewModel
					{
						productSid = p.ProductSid,
						productName = p.ProductName,
						price = p.Price,
						stock = p.Stock,
						launchTime = p.LaunchTime,
						publicStatusNo = p.PublicStatusNo
					})
					.ToList();
			}

			// 查詢所有子分類並傳送到前端
			ViewBag.Subcategories = _context.ProductSubcategories.Select(s => new
			{
				s.SubcategoryNo,
				s.SubcategoryName
			}).ToList();

			ViewBag.Items = _context.ProductItems.Select(i => new
			{
				i.ItemNo,
				i.ItemName
			}).ToList();

			return View(model);
        }

		[HttpPost]
		public IActionResult ProductLaunch(ProductViewModel model)
		{
			// 驗證輸入的資料
			if (ModelState.IsValid)
			{
				// 處理資料邏輯
				TempData["success"] = "商品已成功送出審核!";

				// 查詢登錄的用戶對應的賣場
				var aspUserId = _userManager.GetUserId(User);
				var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
				var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

				if (store != null)
				{
					// 創建新商品資料
					var product = new Product
					{
						ProductName = model.productName,
						Price = model.price,
						Stock = model.stock,
						SubcategoryNo = model.subcategoryNo,
						ItemNo = model.itemNo,
						Description = model.description,
						Origin = "台灣",  // 預設為台灣
						Quanitity = model.quantity,
						Weight = model.weight,
						Component = model.component,
						StoreSid = store.StoreSid,
						DiscountPercent = model.discountPercent,
						LaunchTime = DateOnly.FromDateTime(DateTime.Now),
						PublicStatusNo = 2  // 預設為非公開
					};

					// 保存商品到資料庫
					_context.Products.Add(product);
					_context.SaveChanges();					
				}
			}
			// 重定向到審核中Tab頁面
			return RedirectToAction("ProductLaunch");
			//return View(model);
		}
	}
}
