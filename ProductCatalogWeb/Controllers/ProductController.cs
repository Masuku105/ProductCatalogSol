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
                SearchQuery = search,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        [HttpGet("/Product/GetProductsJson")]
        public async Task<IActionResult> GetProductsJson(string search = "", int page = 1)
        {
            var all = await _productService.GetAllAsync() ?? new List<ProductDto>();

            if (!string.IsNullOrWhiteSpace(search))
                all = all.Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase));

            const int pageSize = 6;
            var totalItems = all.Count();
            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                products = paged,
                currentPage = page,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var created = await _productService.CreateProductAsync(model);
            if (created == null)
            {
                ModelState.AddModelError("", "Failed to create product");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> GetEditModal(int id)
        {
            var prod = await _productService.GetProductByIdAsync(id);
            if (prod == null) return NotFound();
            var dto = new ProductUpdateDto
            {
                Id = prod.Id,
                Title = prod.Title,
                Description = prod.Description,
                Price = prod.Price,
                Image = prod.Image
            };
            return PartialView("_EditProductModal", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateDto model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditProductModal", model);

            var updated = await _productService.UpdateProductAsync(model);
            if (updated == null)
                ModelState.AddModelError("", "Failed to update product");

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
