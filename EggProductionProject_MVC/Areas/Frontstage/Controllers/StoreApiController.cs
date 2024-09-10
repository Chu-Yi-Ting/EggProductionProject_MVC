using EggProductionProject_MVC.Areas.Frontstage.DTO;
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

        //新增上架商品
        [HttpPost]
        public IActionResult ProductLaunch(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 查找用户的商店
                var aspUserId = _userManager.GetUserId(User);
                var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
                var store = _context.Stores.FirstOrDefault(s => s.MemberSid == member.MemberSid);

                if (store != null)
                {
                    // 创建新商品
                    var product = new Product
                    {
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
                        LaunchTime = DateOnly.FromDateTime(DateTime.Now),
                        PublicStatusNo = 2 // 默认为非公开
                    };

                    // 保存商品到数据库
                    _context.Products.Add(product);
                    _context.SaveChanges();

                    // 返回成功和新商品的相关信息
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

            // 返回失败信息
            return Json(new { success = false, message = "提交商品失败，请重试。" });
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


        //取得商品給編輯商品modal使用
        //      [HttpGet]
        //public IActionResult GetProductById(int productSid)
        //{
        //	var product = (from p in _context.Products
        //				   join s in _context.ProductSubcategories
        //				   on p.SubcategoryNo equals s.SubcategoryNo
        //				   join i in _context.ProductItems
        //				   on p.ItemNo equals i.ItemNo
        //				   where p.ProductSid == productSid
        //				   select new ProductViewModel
        //				   {
        //					   productSid = p.ProductSid,
        //					   productName = p.ProductName,
        //					   price = p.Price,
        //					   subcategoryNo = p.SubcategoryNo,
        //					   subcategoryName = s.SubcategoryName,  // 從聯結的ProductSubcategory表中獲取SubcategoryName
        //					   itemNo = p.ItemNo,
        //					   itemName = i.ItemName,  // 從聯結的ProductItems表中獲取ItemName
        //					   description = p.Description,
        //					   quantity = p.Quanitity,
        //					   weight = p.Weight,
        //					   component = p.Component,
        //					   discountPercent = p.DiscountPercent
        //				   }).FirstOrDefault();

        //	if (product != null)
        //	{
        //		return Ok(product);
        //	}
        //	return NotFound(new { success = false, message = "找不到該商品!!" });
        //}

        // 根據 productSid 獲取商品資料給編輯商品modal使用
        [HttpGet]
        public IActionResult GetProductById(int productSid)
        {
            var product = _context.Products
                .Where(p => p.ProductSid == productSid)
                .Select(p => new
                {
                    p.ProductSid,
                    p.ProductName,
                    p.Price,
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
        public IActionResult EditProduct(ProductViewModel model)
        {
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

            _context.SaveChanges();

            // 返回更新後的商品資料
            return Json(new { success = true, updatedProduct = product });
        }

        //刪除上架及審核中的商品
        [HttpPost]
		public IActionResult DeleteProduct([FromBody] DeleteRequestDTO _deleteRequestDTO)
		{
			var product = _context.Products.FirstOrDefault(p => p.ProductSid == _deleteRequestDTO.productSid);
			if (product != null)
			{
				_context.Products.Remove(product);
				_context.SaveChanges();
				return Json(new { success = true, selectedProductSid = _deleteRequestDTO.productSid, message = "商品已成功刪除!" });
			}
			return Json(new { success = false, message = "找不到該商品!" });
		}

	}
}

