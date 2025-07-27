using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductCatalogAPI.Controllers;
using ProductCatalogService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using ProductCatalogService.Dtos;
namespace ProductCatalogUnitTests
{
    public class ProductCatalogTest
    {
        private readonly Mock<IProductCatalogService> _mockService;
        private readonly ProductCatalogController _controller;

        public ProductCatalogTest()
        {
            _mockService = new Mock<IProductCatalogService>();
            _controller = new ProductCatalogController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithProducts()
        {
            // Arrange
            _mockService.Setup(s => s.GetProductsAsync()).ReturnsAsync(new List<ProductDto>
        {
            new ProductDto { Id = 1, Title = "Test" }
        });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);

            var products = okResult.Value as List<ProductDto>;
            products.Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_WithValidId_ReturnsOk()
        {
            // Arrange
            var product = new ProductDto { Id = 1, Title = "Test Product" };
            _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.Get(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task Get_WithInvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetProductByIdAsync(999)).ReturnsAsync((ProductDto)null);

            var result = await _controller.Get(999);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_WithValidDto_ReturnsOk()
        {
            var createDto = new ProductCreateDto { Title = "New" };
            var created = new ProductDto { Id = 1, Title = "New" };

            _mockService.Setup(s => s.AddProductAsync(createDto)).ReturnsAsync(created);

            var result = await _controller.Create(createDto);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public async Task Create_WithInvalidDto_ReturnsBadRequest()
        {
            var createDto = new ProductCreateDto { Title = null };

            _mockService.Setup(s => s.AddProductAsync(createDto)).ReturnsAsync((ProductDto)null);

            var result = await _controller.Create(createDto);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Update_WithValidDto_ReturnsOk()
        {
            var updateDto = new ProductUpdateDto { Id = 1, Title = "Updated" };
            var updated = new ProductDto { Id = 1, Title = "Updated" };

            _mockService.Setup(s => s.UpdateProductAsync(updateDto)).ReturnsAsync(updated);

            var result = await _controller.Update(1, updateDto);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeEquivalentTo(updated);
        }

        [Fact]
        public async Task Update_WithIdMismatch_ReturnsBadRequest()
        {
            var updateDto = new ProductUpdateDto { Id = 2, Title = "Mismatch" };

            var result = await _controller.Update(1, updateDto);

            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("ID mismatch");
        }

        [Fact]
        public async Task Update_WithNonExistingProduct_ReturnsNotFound()
        {
            var updateDto = new ProductUpdateDto { Id = 1, Title = "Missing" };
            _mockService.Setup(s => s.UpdateProductAsync(updateDto)).ReturnsAsync((ProductDto)null);

            var result = await _controller.Update(1, updateDto);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WithExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(true);
        }

        [Fact]
        public async Task Delete_WithNonExistingId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteProductAsync(999)).ReturnsAsync(false);

            var result = await _controller.Delete(999);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
