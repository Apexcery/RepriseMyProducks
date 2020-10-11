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
using Producks.Undercutters;
using Producks.Web.Models;
using Producks.Web.ViewModels;

namespace Producks.Web.Controllers
{
    public class StoreController : Controller
    {
        private readonly StoreDb _context;
        private List<Producks.Undercutters.Models.Category> UndercuttersCategories;
        private List<Producks.Undercutters.Models.Brand> UndercuttersBrands;
        private List<Producks.Undercutters.Models.Product> UndercuttersProducts;

        public StoreController(StoreDb context)
        {
            _context = context;

            UndercuttersCategories = UndercuttersAPI.GetCategories().Result;
            UndercuttersBrands = UndercuttersAPI.GetBrands().Result;
            UndercuttersProducts = UndercuttersAPI.GetProducts().Result;
        }

        public async Task<IActionResult> Index()
        {
            var categories = new List<SelectListItem>
            {
                new SelectListItem("Any", "-1")
            };
            categories.AddRange(await _context.Categories
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
                .ToListAsync());
            ViewBag.Categories = categories;

            var brands = new List<SelectListItem>
            {
                new SelectListItem("Any", "-1")
            };
            brands.AddRange(await _context.Brands
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
                .ToListAsync());
            ViewBag.Brands = brands;

            return View(new StoreDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewProducts([Bind("BrandId, CategoryId")] StoreDto filter)
        {
            var allProducksProducts = await _context.Products
                .Where(x => x.Active)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .ToListAsync();
            
            if (filter.CategoryId == -1 && filter.BrandId == -1)
            {
                var allProducts = allProducksProducts
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
                    .Concat(UndercuttersProducts.Select(x => new ProductViewModel
                    {
                        Id = x.Id,
                        Name = x.Description,
                        Description = x.Description,
                        Price = x.Price,
                        InStock = x.InStock,
                        BrandName = x.BrandName,
                        CategoryName = x.CategoryName,
                        Undercutters = true
                    }));
                
                return View("Products", allProducts);
            }

            if (filter.CategoryId != -1)
            {
                var allProducts = allProducksProducts
                    .Where(x => x.CategoryId == filter.CategoryId)
                    .Select(x =>
                    {
                        if (filter.BrandId != -1 && x.BrandId != filter.BrandId)
                        {
                            return null;
                        }
                        var product = new ProductViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Price = x.Price,
                            InStock = x.StockLevel > 0,
                            BrandName = x.Brand.Name,
                            CategoryName = x.Category.Name,
                            Undercutters = false
                        };
                        return product;
                    })
                    .Concat(UndercuttersProducts
                        .Where(x => x.CategoryId == filter.CategoryId)
                        .Select(x =>
                        {
                            if (filter.BrandId != -1 && x.BrandId != filter.BrandId)
                            {
                                return null;
                            }
                            var product = new ProductViewModel
                            {
                                Id = x.Id,
                                Name = x.Description,
                                Description = x.Description,
                                Price = x.Price,
                                InStock = x.InStock,
                                BrandName = x.BrandName,
                                CategoryName = x.CategoryName,
                                Undercutters = true
                            };
                            return product;
                        }))
                    .Where(x => x != null)
                    .ToList();

                return View("Products", allProducts);
            }

            if (filter.BrandId != -1)
            {
                var allProducts = allProducksProducts
                    .Where(x => x.BrandId == filter.BrandId)
                    .Select(x =>
                    {
                        if (filter.CategoryId != -1 && x.CategoryId != filter.CategoryId)
                        {
                            return null;
                        }
                        var product = new ProductViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Price = x.Price,
                            InStock = x.StockLevel > 0,
                            BrandName = x.Brand.Name,
                            CategoryName = x.Category.Name,
                            Undercutters = false
                        };
                        return product;
                    })
                    .Concat(UndercuttersProducts
                        .Where(x => x.BrandId == filter.BrandId)
                        .Select(x =>
                        {
                            if (filter.CategoryId != -1 && x.CategoryId != filter.CategoryId)
                            {
                                return null;
                            }
                            var product = new ProductViewModel
                            {
                                Id = x.Id,
                                Name = x.Description,
                                Description = x.Description,
                                Price = x.Price,
                                InStock = x.InStock,
                                BrandName = x.BrandName,
                                CategoryName = x.CategoryName,
                                Undercutters = true
                            };
                            return product;
                        }))
                    .Where(x => x != null)
                    .ToList();

                return View("Products", allProducts);
            }

            return NotFound();
        }
    }
}
