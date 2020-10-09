using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Producks.Data;
using Producks.Web.Models;
using Producks.Web.ViewModels;

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
            var brands = await _context.Brands
                .Where(x => x.Active)
                .Select(x => new BrandsViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();

            ViewBag.Brands = brands;

            var categories = await _context.Categories
                .Where(x => x.Active)
                .Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(new StoreDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewProducts([Bind("BrandId, CategoryId")] StoreDto filter)
        {
            var products = await _context.Products
                .Where(
                    x => x.Active &&
                    x.BrandId == filter.BrandId &&
                    x.CategoryId == filter.CategoryId)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    InStock = x.StockLevel > 0,
                    BrandName = x.Brand.Name,
                    CategoryName = x.Category.Name
                })
                .ToListAsync();

            return View("Products", products);
        }
    }
}
