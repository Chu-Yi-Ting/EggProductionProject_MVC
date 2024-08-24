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


        public class CartProductDto
        {
            public int CartSid { get; set; }
            public int? MemberSid { get; set; }
            public int? ProductSid { get; set; }
            public int? Qty { get; set; }
            public string ProductNo { get; set; }
            public string ProductName { get; set; }
            public decimal? Price { get; set; }
            public int? Stock { get; set; }
        }

        public IActionResult GetCarts()
        {
            var cartsWithProducts = _context.Carts
                                            .Join(_context.Products,
                                                  cart => cart.ProductSid,
                                                  product => product.ProductSid,
                                                  (cart, product) => new CartProductDto
                                                  {
                                                      CartSid = cart.CartSid,
                                                      MemberSid = cart.MemberSid,
                                                      ProductSid = cart.ProductSid,
                                                      Qty = cart.Qty,
                                                      ProductNo = product.ProductNo,
                                                      ProductName = product.ProductName,
                                                      Price = product.Price,
                                                      Stock = product.Stock
                                                  })
                                            .ToList();

            return Json(cartsWithProducts);
        }
    }
}

