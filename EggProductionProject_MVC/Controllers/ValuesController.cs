using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly EggPlatformContext _context;

        public ValuesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: api/Values
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductImages)
                .Select(product => new
                {
                    product.ProductSid,
                    Images = product.ProductImages.Select(i => new { i.ProductImagePath, i.ImageDescription, i.UploadTime }),
                    product.ProductName,
                    product.Price,
                    product.Stock,
                    product.SubcategoryNo,
                    product.ItemNo,
                    product.StoreSid,
                    product.Description,
                    product.Origin,
                    product.Quanitity,
                    product.Weight,
                    product.Component,
                    product.DiscountPercent
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}
  
