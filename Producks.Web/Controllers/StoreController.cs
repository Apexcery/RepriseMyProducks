using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Producks.Data;
using Producks.Web.Models;
using Producks.Web.ViewModels;

namespace Producks.Web.Controllers
{
    public class StoreController : Controller
    {
        private readonly StoreDb _context;
        private readonly HttpClient _client = new HttpClient();
        private const string UndercuttersBaseUrl = "http://undercutters.azurewebsites.net/api";
        private List<Models.Undercutters.Category> UndercuttersCategories;
        private List<Models.Undercutters.Product> UndercuttersProducts;
        private List<Models.Undercutters.Brand> UndercuttersBrands;

        public StoreController(StoreDb context)
        {
            _context = context;

            LoadUndercuttersCategories();
            LoadUndercuttersProducts();
            LoadUndercuttersBrands();
        }

        #region LoadUndercuttersData

            public void LoadUndercuttersCategories()
            {
                var response = _client.GetAsync($"{UndercuttersBaseUrl}/category").Result;
                if (!response.IsSuccessStatusCode)
                    return;
                
                UndercuttersCategories = JsonConvert.DeserializeObject<List<Models.Undercutters.Category>>(response.Content.ReadAsStringAsync().Result);
            }
            public void LoadUndercuttersProducts()
            {
                var response = _client.GetAsync($"{UndercuttersBaseUrl}/product").Result;
                if (!response.IsSuccessStatusCode)
                    return;
                
                UndercuttersProducts = JsonConvert.DeserializeObject<List<Models.Undercutters.Product>>(response.Content.ReadAsStringAsync().Result);
            }
            public void LoadUndercuttersBrands()
            {
                var response = _client.GetAsync($"{UndercuttersBaseUrl}/brand").Result;
                if (!response.IsSuccessStatusCode)
                    return;
                
                UndercuttersBrands = JsonConvert.DeserializeObject<List<Models.Undercutters.Brand>>(response.Content.ReadAsStringAsync().Result);
            }

        #endregion

        public async Task<IActionResult> Index()
        {
            var brands = await _context.Brands
                .Where(x => x.Active)
                .Select(x => new BrandsViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .Union(UndercuttersBrands.Select(x => new BrandsViewModel
                {
                    Id = x.Id,
                    Name = $"{x.Name} (Undercutters)"
                }))
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
                .Union(UndercuttersCategories.Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = $"{x.Name} (Undercutters)",
                    Description = x.Description
                }))
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(new StoreDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewProducts([Bind("BrandId, CategoryId")] StoreDto filter)
        {
            if (_context.Products.All(x => x.BrandId != filter.BrandId || x.CategoryId != filter.CategoryId))
            {
                var undercuttersProducts = UndercuttersProducts
                    .Where(x => x.BrandId == filter.BrandId && x.CategoryId == filter.CategoryId)
                    .Select(x => new ProductViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        InStock = x.InStock,
                        BrandName = x.BrandName,
                        CategoryName = x.CategoryName,
                        Undercutters = true
                    }).ToList();

                if (!undercuttersProducts.Any())
                    return NotFound();
                
                return View("Products", undercuttersProducts);
            }
            
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
                    CategoryName = x.Category.Name,
                    Undercutters = false
                })
                .ToListAsync();

            return View("Products", products);
        }
    }
}
