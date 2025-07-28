using Microsoft.AspNetCore.Mvc;
using ProductCatalog.MvcClient.Services;
using ProductCatalogWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private const int PageSize = 6;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var allProducts = await _productService.GetAllAsync() ?? new List<ProductDto>();
            var categories = allProducts.Select(x => x.Category).Distinct().Select(c=> new CategoryDto { Category= c}).ToList();
    

          
            if (!string.IsNullOrWhiteSpace(search))
                allProducts = allProducts.Where(p => p.Title.ToLower().Contains(search.ToLower()));

            var totalItems = allProducts.Count();
            var PageSize = 6;
            var totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            var paged = allProducts
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var vm = new ProductListViewModel
            {
                Products = paged,
                Categories = categories,
                SearchQuery = search,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        [HttpGet("/Product/GetProductsJson")]
        public async Task<IActionResult> GetProductsJson(string search = "", int page = 1, string category = "", string sort = "")
        {
            var all = await _productService.GetAllAsync() ?? new List<ProductDto>();

            if (!string.IsNullOrWhiteSpace(search))
                all = all.Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(category))
                all = all.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            // Sorting
            all = sort switch
            {
                "name_asc" => all.OrderBy(p => p.Title).ToList(),
                "name_desc" => all.OrderByDescending(p => p.Title).ToList(),
                "price_asc" => all.OrderBy(p => p.Price).ToList(),
                "price_desc" => all.OrderByDescending(p => p.Price).ToList(),
                _ => all
            };

            const int pageSize = 6;
            var totalItems = all.Count();
            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                products = paged.Select(p => new {
                    id = p.Id,
                    title = p.Title,
                    description = p.Description,
                    price = p.Price,
                    image = p.Image,
                    category = p.Category,
                    rating = new
                    {
                        rate = p.Rating?.Rate ?? 0,
                        count = p.Rating?.Count ?? 0
                    }
                }),
                currentPage = page,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            });

        }
        [HttpGet("/Product/GetCreateModal")]
        public IActionResult GetCreateModal() => PartialView("_CreateProductModal", new ProductCreateDto());

        [HttpGet("/Product/GetEditModal")]
        public async Task<IActionResult> GetEditModal(int id)
        {
            var p = await _productService.GetProductByIdAsync(id);
            return PartialView("_EditProductModal", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto model)
        {
            if (!ModelState.IsValid) return PartialView("_CreateProductModal", model);
            var results = await _productService.CreateProductAsync(model);
            if (results.Id != 0)
            {
                return RedirectToAction("Index");
            }
            return PartialView("_CreateProductModal", new ProductCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateDto model)
        {
            if (!ModelState.IsValid) return PartialView("_EditProductModal", model);
            var results = await _productService.UpdateProductAsync(model);
            if (results != null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("_EditProductModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
