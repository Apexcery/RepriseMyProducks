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
    public class StoreController : Controller
    {
        private readonly StoreDb _context;

        public StoreController(StoreDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories
                .Where(x => x.Active)
                .ToListAsync());
        }


        //Id = Category Id
        public async Task<IActionResult> ViewProducts(int? Id)
        {
            if (Id == null)
                return BadRequest();

            if (_context.Categories.FirstOrDefault(x => x.Id == Id) == null)
                return NotFound();

            var products = await _context.Products
                .Where(x =>
                    x.CategoryId == Id &&
                    x.Active)
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    StockLevel = x.StockLevel,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name
                })
                .ToListAsync();

            return View("Products", products);
        }
    }
}
