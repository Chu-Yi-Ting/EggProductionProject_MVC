using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class CartsAPIController : Controller
    {
        private readonly EggPlatformContext _context;

        public CartsAPIController(EggPlatformContext context)
        {
            _context = context;
        }

        public IActionResult Carts()
        {
            //var carts = _context.Carts.ToList();
            var carts = _context.Products
                        //.Include(c => c.ProductS) 
                        .ToList();
            return Json(carts);
        }

        //public IActionResult GetCarts()
        //{
        //    var carts = _context.Carts
        //                        .ToList();

        //    return Json(carts);
        //}


        //public class CartProductDto
        //{
        //    public int CartSid { get; set; }
        //    public int? MemberSid { get; set; }
        //    public int? ProductSid { get; set; }
        //    public int? Qty { get; set; }
        //    public string ProductNo { get; set; }
        //    public string ProductName { get; set; }
        //    public decimal? Price { get; set; }
        //    public int? Stock { get; set; }
        //}

        //public IActionResult GetCarts()
        //{
        //    var cartsWithProducts = _context.Carts
        //                                    .Join(_context.Products,
        //                                          cart => cart.ProductSid,
        //                                          product => product.ProductSid,
        //                                          (cart, product) => new CartProductDto
        //                                          {
        //                                              CartSid = cart.CartSid,
        //                                              MemberSid = cart.MemberSid,
        //                                              ProductSid = cart.ProductSid,
        //                                              Qty = cart.Qty,
        //                                              ProductNo = product.ProductNo,
        //                                              ProductName = product.ProductName,
        //                                              Price = product.Price,
        //                                              Stock = product.Stock
        //                                          })
        //                                    .ToList();

        //    return Json(cartsWithProducts);
        //}

        //public class CartProductDto
        //{
        //    public int CartSid { get; set; }
        //    public int? MemberSid { get; set; }
        //    public int? ProductSid { get; set; }
        //    public int? Qty { get; set; }
        //    public string ProductNo { get; set; }
        //    public string ProductName { get; set; }
        //    public decimal? Price { get; set; }
        //    public int? Stock { get; set; }
        //    public int? StoreSid { get; set; }
        //    public string Company { get; set; }
        //    public byte[] StoreImg { get; set; }
        //}


        //public class CartProductDto
        //{
        //    public int CartSid { get; set; }
        //    public int? MemberSid { get; set; }
        //    public int? ProductSid { get; set; }
        //    public int? Qty { get; set; }
        //    public string ProductNo { get; set; }
        //    public string ProductName { get; set; }
        //    public decimal? Price { get; set; }
        //    public int? Stock { get; set; }
        //    public decimal? DiscountPercent { get; set; }
        //    public int? StoreSid { get; set; }
        //    public string Company { get; set; }
        //    public byte[] StoreImg { get; set; }
        //    public string SubcategoryName { get; set; } // 新增 SubcategoryName 属性
        //}

        //public IActionResult GetCarts()
        //{
        //    var cartsWithProducts = _context.Carts
        //                                    .Join(_context.Products,
        //                                          cart => cart.ProductSid,
        //                                          product => product.ProductSid,

        //                                          (cart, product) => new { cart, product })

        //                                    .Join(_context.Stores,
        //                                          combined => combined.product.StoreSid,
        //                                          store => store.StoreSid,
        //                                          (combined, store) => new CartProductDto
        //                                          {
        //                                              CartSid = combined.cart.CartSid,
        //                                              MemberSid = combined.cart.MemberSid,
        //                                              ProductSid = combined.cart.ProductSid,
        //                                              Qty = combined.cart.Qty,
        //                                              ProductNo = combined.product.ProductNo,
        //                                              ProductName = combined.product.ProductName,
        //                                              Price = combined.product.Price,
        //                                              Stock = combined.product.Stock,
        //                                              StoreSid = store.StoreSid,
        //                                              Company = store.Company,
        //                                              StoreImg = store.StoreImg
        //                                          })
        //                                    .ToList();

        //    return Json(cartsWithProducts);
        //}

        //public IActionResult GetCarts(int m)
        //{
        //    var cartsWithProducts = _context.Carts
        //                                    .Where(c => c.MemberSid == m)
        //                                    .Join(_context.Products,
        //                                          cart => cart.ProductSid,
        //                                          product => product.ProductSid,
        //                                          (cart, product) => new { cart, product })
        //                                    .Join(_context.Stores,
        //                                          combined => combined.product.StoreSid,
        //                                          store => store.StoreSid,
        //                                          (combined, store) => new { combined.cart, combined.product, store })
        //                                    .Join(_context.ProductSubcategories,
        //                                          combined => combined.product.SubcategoryNo,
        //                                          subcategory => subcategory.SubcategoryNo,
        //                                          (combined, subcategory) => new CartProductDto
        //                                          {
        //                                              CartSid = combined.cart.CartSid,
        //                                              MemberSid = combined.cart.MemberSid,
        //                                              ProductSid = combined.cart.ProductSid,
        //                                              Qty = combined.cart.Qty,
        //                                              ProductNo = combined.product.ProductNo,
        //                                              ProductName = combined.product.ProductName,
        //                                              Price = combined.product.Price,
        //                                              Stock = combined.product.Stock,
        //                                              DiscountPercent= combined.product.DiscountPercent,
        //                                              StoreSid = combined.store.StoreSid,
        //                                              Company = combined.store.Company,
        //                                              StoreImg = combined.store.StoreImg,
        //                                              SubcategoryName = subcategory.SubcategoryName // 获取 SubcategoryName
        //                                          })
        //                                    .ToList();

        //    return Json(cartsWithProducts);
        //}

        public class MemberFilterDto
        {
            public int Msid { get; set; }
        }

        //[HttpPost]
        //public IActionResult CartsList([FromBody] MemberFilterDto m)
        //{
        //    var cartsWithProducts = _context.Carts
        //                                    .Where(c => c.MemberSid == m.Msid)
        //                                    .Join(_context.Products,
        //                                          cart => cart.ProductSid,
        //                                          product => product.ProductSid,
        //                                          (cart, product) => new { cart, product })
        //                                    .Join(_context.Stores,
        //                                          combined => combined.product.StoreSid,
        //                                          store => store.StoreSid,
        //                                          (combined, store) => new { combined.cart, combined.product, store })
        //                                    .Join(_context.ProductSubcategories,
        //                                          combined => combined.product.SubcategoryNo,
        //                                          subcategory => subcategory.SubcategoryNo,
        //                                          (combined, subcategory) => new CartProductDto
        //                                          {
        //                                              CartSid = combined.cart.CartSid,
        //                                              MemberSid = combined.cart.MemberSid,
        //                                              ProductSid = combined.cart.ProductSid,
        //                                              Qty = combined.cart.Qty,
        //                                              ProductNo = combined.product.ProductNo,
        //                                              ProductName = combined.product.ProductName,
        //                                              Price = combined.product.Price,
        //                                              Stock = combined.product.Stock,
        //                                              DiscountPercent = combined.product.DiscountPercent,
        //                                              StoreSid = combined.store.StoreSid,
        //                                              Company = combined.store.Company,
        //                                              StoreImg = combined.store.StoreImg,
        //                                              SubcategoryName = subcategory.SubcategoryName // 获取 SubcategoryName
        //                                          })
        //                                    .ToList();

        //    return Json(cartsWithProducts);
        //}

        public class StoreDto
        {
            public int StoreSid { get; set; }
            public string Company { get; set; }
            public byte[] StoreImg { get; set; }
            public List<ProductDto> Products { get; set; }
        }

        public class ProductDto
        {
            public int CartSid { get; set; }
            public int MemberSid { get; set; }
            public int ProductSid { get; set; }
            public int Qty { get; set; }
            public string ProductNo { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public decimal? DiscountPercent { get; set; }
            public string SubcategoryName { get; set; }
        }

        [HttpPost]
        public IActionResult CartsList([FromBody] MemberFilterDto filter)
        {
            // 查询数据并连接相关表
            var cartsWithProducts = _context.Carts
                                            .Where(c => c.MemberSid == filter.Msid)
                                            .Join(_context.Products,
                                                  cart => cart.ProductSid,
                                                  product => product.ProductSid,
                                                  (cart, product) => new { cart, product })
                                            .Join(_context.Stores,
                                                  combined => combined.product.StoreSid,
                                                  store => store.StoreSid,
                                                  (combined, store) => new { combined.cart, combined.product, store })
                                            .Join(_context.ProductSubcategories,
                                                  combined => combined.product.SubcategoryNo,
                                                  subcategory => subcategory.SubcategoryNo,
                                                  (combined, subcategory) => new
                                                  {
                                                      combined.store,
                                                      combined.product,
                                                      combined.cart,
                                                      subcategory.SubcategoryName
                                                  })
                                            .ToList();

            // 分组并转换为目标格式
            var groupedByStore = cartsWithProducts
                                  .GroupBy(x => new
                                  {
                                      x.store.StoreSid,
                                      x.store.Company,
                                      x.store.StoreImg
                                  })
                                  .Select(g => new StoreDto
                                  {
                                      StoreSid = g.Key.StoreSid,
                                      Company = g.Key.Company,
                                      StoreImg = g.Key.StoreImg,
                                      Products = g.Select(x => new ProductDto
                                      {
                                          CartSid = x.cart.CartSid,
                                          MemberSid = (int)x.cart.MemberSid,
                                          ProductSid = x.product.ProductSid,
                                          Qty = (int)x.cart.Qty,
                                          ProductNo = x.product.ProductNo,
                                          ProductName = x.product.ProductName,
                                          Price = (decimal)x.product.Price,
                                          Stock = (int)x.product.Stock,
                                          DiscountPercent = x.product.DiscountPercent,
                                          SubcategoryName = x.SubcategoryName
                                      }).ToList()
                                  }).ToList();

            return Json(groupedByStore);
        }


        [HttpPost]
        public IActionResult UpdateCartQuantity([FromBody] CartUpdateRequest request)
        {
            // 查找指定的 Cart 实体
            var cart = _context.Carts.SingleOrDefault(c => c.CartSid == request.CartSid);

            if (cart != null)
            {
                if (request.Qty < 1)
                {
                    // 处理不允许的数量值
                    _context.Carts.Remove(cart);
                    _context.SaveChanges();
                }

                // 更新 Qty 属性
                cart.Qty = request.Qty;

                // 保存更改到数据库
                _context.SaveChanges();

                // 返回成功响应
                return Ok();
            }
            else
            {
                // 处理 Cart 不存在的情况
                return NotFound($"Cart with CartSid {request.CartSid} not found.");
            }
        }

        public class CartUpdateRequest
        {
            public int CartSid { get; set; }
            public int Qty { get; set; }
        }

    }
}

