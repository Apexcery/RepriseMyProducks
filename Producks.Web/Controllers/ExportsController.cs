using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Producks.Data;
using Producks.Web.Models;

namespace Producks.Web.Controllers
{
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly StoreDb _context;

        public ExportsController(StoreDb context)
        {
            _context = context;
        }

        // GET: api/Brands
        [HttpGet("api/Brands")]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _context.Brands
                .Where(x => x.Active)
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
            return Ok(brands);
        }

        // Get: api/Categories
        [HttpGet("api/Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Where(x => x.Active)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToListAsync();
            return Ok(categories);
        }

        // Get: api/Product/{categoryId}/{brandId}
        [HttpGet("api/Product/{categoryId}/{brandId}")]
        public async Task<IActionResult> GetProduct(int categoryId, int brandId)
        {
            var products = await _context.Products
                .Where(x =>
                    x.BrandId == brandId &&
                    x.CategoryId == categoryId &&
                    x.Active)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    StockLevel = x.StockLevel,
                    BrandId = x.BrandId,
                    CategoryId = x.CategoryId
                }).ToListAsync();
            return Ok(products);
        }
    }
}
