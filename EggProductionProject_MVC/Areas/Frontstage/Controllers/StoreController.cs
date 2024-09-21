using EggProductionProject_MVC.Areas.Frontstage.DTO;
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

		// 查詢賣家資訊及賣家的商品
		public IActionResult Store(StoreQueryDTO _storeQueryDTO)
		{			
            var storeInfo = _context.Stores
				.Where(s => s.StoreSid == _storeQueryDTO.storeSid)
				.Select(s => new StoreViewModel
				{
					storeSid = s.StoreSid,
					memberSid = s.MemberSid,
					storeImagePath = s.StoreImagePath,
					storeName = s.Company,
					establishDate = s.EstablishDate,
					storeIntroduction = s.StoreIntroduction,
					productCount = _context.Products.Where(p => p.StoreSid == s.StoreSid && p.PublicStatusNo == 1).Count() // 只計算公開狀態的商品數量
				})
				.FirstOrDefault();

			if (storeInfo == null)
			{
				return NotFound();
			}

			// 分頁查詢該賣家的公開商品
			var products = _context.Products
				.Where(p => p.StoreSid == _storeQueryDTO.storeSid && p.PublicStatusNo == 1)
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
				.OrderBy(p => p.productName) // 你可以根據需要更改排序依據
				.Skip((_storeQueryDTO.page - 1) * _storeQueryDTO.pageSize)
				.Take(_storeQueryDTO.pageSize)
				.ToList();

			// 組合商品及賣家資訊
			var ProductInStore = new ProductAndStoreViewModel
			{
				storeInfo = storeInfo,
				products = products,
				PageNumber = _storeQueryDTO.page,
				PageSize = _storeQueryDTO.pageSize,
				TotalPages = (int)Math.Ceiling((double)storeInfo.productCount / _storeQueryDTO.pageSize) // 計算總頁數
			};

            // 如果是 AJAX 請求，返回部分視圖
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Areas/Frontstage/PartialViews/_ProductListPartial.cshtml", ProductInStore);
            }

            // 如果是普通請求，返回完整視圖
            return View(ProductInStore);
		}



		//SellerInformation動作函式生賣家中心基本資料畫面
		public IActionResult SellerInformation()
		{
            ViewData["Title"] = "GOOD EGG 賣家中心-基本資料";
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
        public JsonResult SellerInformation(StoreViewModel model)
        {
            // 1. 取得當前已登入的用戶，並根據 AspUserId 取得對應的 Member
            var aspUserId = _userManager.GetUserId(User);
            var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
            if (member == null)
            {
                // 用戶未登入，返回 JSON 結果並提供重定向 URL
                return Json(new
                {
                    success = false,
                    message = "請先登入再進行操作。",
                    redirectUrl = Url.Action("Login", "Account")
                });
            }

            // 2.後端驗證賣場名稱必填
            if (string.IsNullOrWhiteSpace(model.storeName))
            {
                return Json(new { success = false});
            }

            // 2.後端驗證賣場簡介必填
            if (string.IsNullOrWhiteSpace(model.storeIntroduction))
            {
                return Json(new { success = false});
            }

            // 2.檢查賣場名稱是否已存在（排除當前賣場）
            var existingStore = _context.Stores
                .FirstOrDefault(s => s.Company == model.storeName && s.StoreSid != model.storeSid);

            if (existingStore != null)
            {
                return Json(new { success = false});
            }

            // 3. 檢查是否已經存在該會員的賣場，若無則創建新的 Store 資料
            var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

            // 4. 處理圖片上傳
            string storeImagePath = store?.StoreImagePath; // 保留舊圖片路徑
            if (model.storeImage != null && model.storeImage.Length > 0)
            {
                // 4.1 刪除舊的圖片
                if (!string.IsNullOrEmpty(storeImagePath))
                {
                    var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, storeImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // 4.2 上傳新的圖片
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", "Stores");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.storeImage.FileName);
                var newImagePath = Path.Combine(uploadPath, uniqueFileName);
                storeImagePath = $"/Images/Stores/{uniqueFileName}";  // 將相對路徑存入資料庫

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
                _context.SaveChanges();

                return Json(new { success = true, message = "您已成功註冊賣家身分" });
            }
            else
            {
                // 更新現有賣場資料
                store.Company = model.storeName;  // 更新賣場名稱
                store.StoreIntroduction = model.storeIntroduction;  // 更新賣場簡介
                store.StoreImagePath = storeImagePath;  // 更新圖片路徑

                _context.SaveChanges();

                return Json(new { success = true, message = "賣家基本資料更新成功" });
            }
        }

        //ProductLaunch動作函式生賣家中心商品上架畫面
        public IActionResult ProductLaunch(int? productSid = null)
        {
            ViewData["Title"] = "GOOD EGG 賣家中心-商品上架";
            // 查詢當前登入用戶的 StoreSid
            var aspUserId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(aspUserId))
			{
				return RedirectToAction("Login", "Account", new { area = "Identity" });
			}
			var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
			var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

			var model = new ProductLaunchViewModel();

            if (store != null)
            {
                model.IsStoreSetup = true; // 設置為 true 代表賣場已設置

                // 如果傳入的 productSid 有值，設置到模型中
                model.productSid = productSid ?? 0;

                // 查詢該賣家的當前上架產品 (PublicStatusNo = 1)，按LaunchTime降序排列
                model.CurrentProducts = _context.Products
                    .Where(p => p.PublicStatusNo == 1 && p.StoreSid == store.StoreSid)
                    .OrderByDescending(p => p.LaunchTime)  // 按照LaunchTime降序排列
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

                // 查詢該賣家的審核中產品 (PublicStatusNo = 2)，按LaunchTime降序排列
                model.ReviewingProducts = _context.Products
                    .Where(p => p.PublicStatusNo == 2 && p.StoreSid == store.StoreSid)
                    .OrderByDescending(p => p.LaunchTime)  // 按照LaunchTime降序排列
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
            else
            {
                // 沒有賣場資料，將錯誤訊息放入 TempData
                TempData["ErrorMessage"] = "您尚未填寫賣場基本資料，要填寫完才能開通賣場功能!";
                model.IsStoreSetup = false; // 設置為 false 表示賣場未設置
            }


            // 判斷是否是 AJAX 請求
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    currentProducts = model.CurrentProducts,
                    reviewingProducts = model.ReviewingProducts
                });
            }

            // 查詢所有子分類並傳送到前端
            ViewBag.Subcategories = _context.ProductSubcategories.Select(s => new
			{
				s.SubcategoryNo,
				s.SubcategoryName
			}).ToList();

			return View(model);
        }
	}
}
