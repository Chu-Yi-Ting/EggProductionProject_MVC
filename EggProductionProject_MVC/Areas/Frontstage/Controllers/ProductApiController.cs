﻿using EggProductionProject_MVC.Areas.Frontstage.DTO;
using EggProductionProject_MVC.Areas.Frontstage.Models;
using EggProductionProject_MVC.Areas.Frontstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
	public class ProductApiController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;  // 使用Identity User Manager

        public ProductApiController (EggPlatformContext context, IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Products([FromBody] SearchProductDTO _searchProductDTO)
        {
            //根據商品次分類編號讀取相關商品資料及商品圖片
            var products = _searchProductDTO.subcategoryNo == 0
        ? _context.Products
            .Select(p => new ProductWithImagesDTO
            {
                productSid = p.ProductSid,
                productNo = p.ProductNo,
                productName = p.ProductName,
                price = p.Price,
                stock = p.Stock,
                subcategoryNo = p.SubcategoryNo,
                itemNo = p.ItemNo,
                storeSid = p.StoreSid,
                description = p.Description,
                origin = p.Origin,
                quantity = p.Quanitity,
                launchTime = p.LaunchTime,
                productImagePath = _context.ProductImages
                    .Where(img => img.ProductSid == p.ProductSid)
                    .Select(img => img.ProductImagePath)
                    .FirstOrDefault(),
                imageDescription = _context.ProductImages
                    .Where(img => img.ProductSid == p.ProductSid)
                    .Select(img => img.ImageDescription)
                    .FirstOrDefault(),
                uploadTime = _context.ProductImages
                    .Where(img => img.ProductSid == p.ProductSid)
                    .Select(img => img.UploadTime)
                    .FirstOrDefault()
            })
        // 處理分類篩選
        : _context.Products
            .Where(p => p.SubcategoryNo == _searchProductDTO.subcategoryNo)
            .Select(p => new ProductWithImagesDTO
            {
                productSid = p.ProductSid,
                productNo = p.ProductNo,
                productName = p.ProductName,
                price = p.Price,
                stock = p.Stock,
                subcategoryNo = p.SubcategoryNo,
                itemNo = p.ItemNo,
                storeSid = p.StoreSid,
                description = p.Description,
                origin = p.Origin,
                quantity = p.Quanitity,
                launchTime = p.LaunchTime,
                productImagePath = _context.ProductImages
                    .Where(img => img.ProductSid == p.ProductSid)
                    .Select(img => img.ProductImagePath)
                    .FirstOrDefault(),
                imageDescription = _context.ProductImages
                    .Where(img => img.ProductSid == p.ProductSid)
                    .Select(img => img.ImageDescription)
                    .FirstOrDefault(),
                uploadTime = _context.ProductImages
                    .Where(img => img.ProductSid == p.ProductSid)
                    .Select(img => img.UploadTime)
                    .FirstOrDefault()
            });

           
            //價格區間篩選商品
            if (_searchProductDTO.minPrice.HasValue)
            {
                products = products.Where(p => p.price >= _searchProductDTO.minPrice.Value);
            }
            if (_searchProductDTO.maxPrice.HasValue)
            {
                products = products.Where(p => p.price <= _searchProductDTO.maxPrice.Value);
            }


            //關鍵字搜尋
            if (!string.IsNullOrEmpty(_searchProductDTO.keyword))
            {
                products = products.Where(s => s.productName.Contains(_searchProductDTO.keyword));
            }

            //這段程式碼根據 _searchDTO.sortBy 的值決定如何排序資料。
            switch (_searchProductDTO.sortBy) 
            {
                case "price": //如果 sortBy 的值是 "price"，則根據 price 進行排序。
                    products = _searchProductDTO.sortType == "asc" ? products.OrderBy(s => s.price) : products.OrderByDescending(s => s.price);
                    break;
                case "subcategoryNo": //如果 sortBy 的值是 "subcategoryNo"，則根據 subcategoryNo 進行排序。
                    products = _searchProductDTO.sortType == "asc" ? products.OrderBy(s => s.subcategoryNo) : products.OrderByDescending(s => s.subcategoryNo);
                    break;
                case "launchTime": //如果 sortBy 的值是 "launchTime"，則根據 launchTime 進行排序。
                    products = _searchProductDTO.sortType == "asc" ? products.OrderBy(s => s.launchTime) : products.OrderByDescending(s => s.launchTime);
                    break;
                default: //如果 sortBy 的值不是 price 或 subcategoryNo 或 launchTime，則預設根據 productSid 進行排序。
                    products = _searchProductDTO.sortType == "asc" ? products.OrderBy(s => s.productSid) : products.OrderByDescending(s => s.productSid);
                    break;
            }

            //總共有多少筆資料
            //計算篩選後的資料總數，並將其存儲在 totalCount 變數中。
            int totalCount = products.Count();
            //取得每頁顯示的資料筆數，這個值來自 _searchProductDTO.pageSize。
            int pageSize = _searchProductDTO.pageSize;
            //取得當前的頁數，這個值來自 _searchProductDTO.page。
            int page = _searchProductDTO.page;
            //計算總共有幾頁，公式是將總資料數除以每頁的資料筆數，並使用 Math.Ceiling 函數將結果進行無條件進位。
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //這段程式碼進行分頁操作，使用 Skip((page - 1) * pageSize) 跳過前面的資料。使用 Take(pageSize) 取得當前頁的資料。
            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            //設定回傳的資料
            //1.建立一個新的 ProductsPagingDTO 物件，這個物件將用來存放分頁的結果。
            ProductsPagingDTO pagingDTO = new ProductsPagingDTO();
            //將計算出的總頁數存放在 pagingDTO.TotalPages 中。
            pagingDTO.TotalPages = totalPages;
            //將篩選、排序、分頁後的資料轉換成列表，並存放在 pagingDTO.ProductsResult 中。
            pagingDTO.ProductsResult = products.ToList();

            //最後，將 pagingDTO 物件轉換成 JSON 格式，並回傳給前端。

            return Json(pagingDTO);
        }


        //商品詳細分頁的加入購物車功能
        [HttpPost]
        public IActionResult AddToCart([FromBody] CartViewModel cartData)
        {
            if (cartData != null)
            {
                var aspUserId = _userManager.GetUserId(User);
                var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUserId);
                // 將 cartData 寫入資料庫邏輯
                var cart = new Cart
                {
                    MemberSid = member.MemberSid,
                    ProductSid = cartData.productSid,
                    Qty = cartData.qty
                };

                // 假設你有 dbContext 實例
                _context.Carts.Add(cart);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
