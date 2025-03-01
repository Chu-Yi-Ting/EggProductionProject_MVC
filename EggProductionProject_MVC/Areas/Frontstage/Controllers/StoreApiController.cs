﻿using EggProductionProject_MVC.Areas.Frontstage.DTO;
using EggProductionProject_MVC.Areas.Frontstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
	public class StoreApiController : Controller
	{
		private readonly EggPlatformContext _context;
		private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;  // 使用Identity User Manager
        public StoreApiController(EggPlatformContext context, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager) 
		{
			_context = context;
			_hostingEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        //產生產品編號方法
        public async Task<string> GenerateProductNoAsync(int subcategoryNo)
        {
            // 獲取當前日期
            var today = DateTime.Now.ToString("yyyyMMdd");

            // 獲取當天的最大產品编號
            var prefix = subcategoryNo == 3 ? "P" : "E";
            var maxProductNo = await _context.Products
                .Where(p => p.ProductNo.StartsWith($"{prefix}{today}"))
                .OrderByDescending(p => p.ProductNo)
                .Select(p => p.ProductNo)
                .FirstOrDefaultAsync();

            int sequenceNumber = 1; // 預設序號為0001

            //有最大序號商品的話
            if (maxProductNo != null)
            {
                // 提取序號部分加1
                var sequencePart = maxProductNo.Substring(maxProductNo.Length - 4);
                if (int.TryParse(sequencePart, out var sequence))
                {
                    sequenceNumber = sequence + 1;
                }
            }

            // 返回生成的產品編號，格式：EYYYYMMDD0001 或 PYYYYMMDD0001
            return $"{prefix}{today}{sequenceNumber:D4}";
        }

        //商品上架功能
        [HttpPost]
        public async Task<IActionResult> ProductLaunch(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 查詢操作者的商店
                var aspUserId = _userManager.GetUserId(User);
                var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
                var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

                if (store != null)
                {
                    // 生成產品编號
                    var productNo = await GenerateProductNoAsync(model.subcategoryNo);

                    // 新增商品
                    var product = new Product
                    {
                        ProductNo = productNo,
                        ProductName = model.productName,
                        Price = model.price,
                        Stock = model.stock,
                        SubcategoryNo = model.subcategoryNo,
                        ItemNo = model.itemNo,
                        Description = model.description,
                        Origin = "台灣",
                        Quanitity = model.quantity,
                        Weight = model.weight,
                        Component = model.component,
                        StoreSid = store.StoreSid,
                        DiscountPercent = model.discountPercent,
                        LaunchTime = DateTime.Now,
                        PublicStatusNo = 2 // 預設為非公開狀態
                    };

                    // 保存商品到資料庫
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    // 保存裁剪後的圖片到資料庫
                    if (Request.Form.Files["croppedImage"] != null)
                    {
                        var croppedImage = Request.Form.Files["croppedImage"];
                        string uniqueFileName = $"{Guid.NewGuid()}_{croppedImage.FileName}";
                        string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", "Products", uniqueFileName);
                        string productImagePath = $"/Images/Products/{uniqueFileName}";

                        using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await croppedImage.CopyToAsync(fileStream);
                        }

                        // 保存圖片路径到 ProductImages 資料表
                        var productImageEntry = new ProductImage
                        {
                            ProductSid = product.ProductSid,
                            ProductImagePath = productImagePath,
                            ImageDescription = model.productName,  // 這裡使用productName替代ImageDescription
                            UploadTime = DateTime.Now
                        };

                        _context.ProductImages.Add(productImageEntry);
                        await _context.SaveChangesAsync();
                    }

                    // 返回成功和新商品的相關訊息
                    return Json(new
                    {
                        success = true,
                        newProduct = new
                        {
                            productSid = product.ProductSid,
                            productName = product.ProductName,
                            price = product.Price,
                            stock = product.Stock,
                            launchTime = product.LaunchTime.HasValue ? product.LaunchTime.Value.ToString("yyyy-MM-dd") : ""
                        }
                    });
                }
            }

            // 返回失敗信息
            return Json(new { success = false, message = "提交商品失敗，請重試!" });
        }

        //根據商品分類提供相對應品項分類資料
        [HttpGet]
        public IActionResult GetItemsBySubcategory(int subcategoryNo)
        {
            // 根據商品分類ID查詢相關的品項分類
            var items = _context.ProductItems
                .Where(i => i.SubcategoryNo == subcategoryNo) // 根據分類ID過濾品項
                .Select(i => new
                {
                    i.ItemNo,
                    i.ItemName
                })
                .ToList();

            return Json(items);
        }

        // 根據 productSid 獲取商品資料給編輯商品modal使用
        [HttpGet]
        public IActionResult GetProductById(int productSid)
        {
            var image = _context.ProductImages.Where(I => I.ProductSid == productSid).FirstOrDefault();
            string productImagePath;
            if (image == null)
            {
                // 如果沒有圖片資料，使用預設圖片
                productImagePath = "/images/noimage2.jpg";
            }
            else
            {
                // 如果有圖片資料，使用圖片的路徑
                productImagePath = image.ProductImagePath;
            }

            var product = _context.Products.Where(p => p.ProductSid == productSid)
                .Select(p => new
                {
                    p.ProductSid,
                    p.ProductName,
                    p.Price,
                    ProductImagePath = productImagePath,
                    p.SubcategoryNo,
                    SubcategoryName = p.SubcategoryNoNavigation.SubcategoryName,  // 假設 Subcategory 是外鍵
                    p.ItemNo,
                    ItemName = p.ItemNoNavigation.ItemName,  // 假設 Item 是外鍵
                    p.Description,
                    p.Quanitity,
                    p.Weight,
                    p.Component,
                    p.DiscountPercent
                }).FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            return Json(product);
        }

        //編輯上架商品
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            //查詢返回相同產品編號的商品資料
            var product = _context.Products.FirstOrDefault(p => p.ProductSid == model.productSid);

            if (product == null)
            {
                return Json(new { success = false, message = "商品未找到" });
            }           

            // 更新商品資料
            product.ProductName = model.productName;
            product.Price = model.price;            
            product.SubcategoryNo = model.subcategoryNo;
            product.ItemNo = model.itemNo;
            product.Description = model.description;
            product.Quanitity = model.quantity;
            product.Weight = model.weight;
            product.Component = model.component;
            product.DiscountPercent = model.discountPercent;

            // 處理圖片更新，查詢已存在的圖片資料放入existingImage
            var existingImage = _context.ProductImages.FirstOrDefault(img => img.ProductSid == model.productSid);
            // 保留舊圖片路徑
            string productImagePath = existingImage?.ProductImagePath;

            // 只有當有新圖片上傳時才進行更新操作
            if (model.productImage != null && model.productImage.Length > 0)
            {
                // 1. 刪除舊圖片（若存在）
                if (!string.IsNullOrEmpty(productImagePath))
                {
                    var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, productImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // 2. 上傳新的圖片
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", "Products");
                var uniqueFileName = $"{Guid.NewGuid()}_{model.productImage.FileName}";
                var newImagePath = Path.Combine(uploadPath, uniqueFileName);
                productImagePath = $"/Images/Products/{uniqueFileName}";  // 將相對路徑存入資料庫

                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await model.productImage.CopyToAsync(stream);
                }

                // 3. 更新資料庫中的圖片記錄
                if (existingImage != null)
                {
                    existingImage.ProductImagePath = productImagePath;
                    existingImage.ImageDescription = model.productName; // 更新圖片描述為商品名稱
                    existingImage.UploadTime = DateTime.Now; // 更新圖片上傳時間
                }
                else
                {
                    var newImage = new ProductImage
                    {
                        ProductSid = product.ProductSid,
                        ProductImagePath = productImagePath,
                        ImageDescription = model.productName, // 圖片描述為商品名稱
                        UploadTime = DateTime.Now
                    };

                    _context.ProductImages.Add(newImage);
                }
            }

            _context.SaveChanges();

            // 返回更新後的商品資料，手動挑選需要的屬性返回
            var updatedProduct = new
            {
                productSid = product.ProductSid,
                productName = product.ProductName,
                price = product.Price,
                stock = product.Stock,
                launchTime = product.LaunchTime.HasValue ? product.LaunchTime.Value.ToString("yyyy-MM-dd") : null
            };

            // 返回更新後的商品資料
            return Json(new { success = true, updatedProduct });
        }

        //軟刪除上架及審核中的商品
        [HttpPost]
		public IActionResult DeleteProduct([FromBody] DeleteRequestDTO _deleteRequestDTO)
		{
			var product = _context.Products.FirstOrDefault(p => p.ProductSid == _deleteRequestDTO.productSid);
			if (product != null)
			{
                // 更新商品公開狀態為禁用（編號3）
                product.PublicStatusNo = 3;
                _context.Products.Update(product);
                _context.SaveChanges();
                return Json(new { success = true, selectedProductSid = _deleteRequestDTO.productSid, message = "商品已成功刪除!" });
			}
			return Json(new { success = false, message = "找不到該商品!" });
		}

        //賣場名稱驗證
        [HttpGet]
        public JsonResult CheckStoreName(string storeName, int storeSid)
        {
            var exists = _context.Stores.Any(s => s.Company == storeName && s.StoreSid != storeSid);
            return Json(new { exists });
        }

        //商品名稱驗證
        [HttpGet]
        public JsonResult CheckProductName(string productName, int productSid = 0)
        {
            var exists = _context.Products.Any(p => p.ProductName == productName && p.ProductSid != productSid);
           
            return Json(new { exists });
        }
    }
}

