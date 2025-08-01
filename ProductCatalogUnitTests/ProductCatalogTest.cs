﻿using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using ProductCatalogAPI.Controllers;
using ProductCatalogService;
using ProductCatalogService.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Text;

public class ProductCatalogTest
{
    private readonly Mock<IProductCatalogService> _mockService;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly ProductCatalogController _controller;

    public ProductCatalogTest()
    {
        _mockService = new Mock<IProductCatalogService>();
        _mockCache = new Mock<IDistributedCache>();
        _controller = new ProductCatalogController(_mockService.Object, _mockCache.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsFromCache_IfCacheExists()
    {
        // Arrange
        var cachedProducts = new List<ProductDto>
    {
        new ProductDto { Id = 1, Title = "Cached Product" }
    };

        string cachedJson = JsonConvert.SerializeObject(cachedProducts);
        byte[] cachedBytes = Encoding.UTF8.GetBytes(cachedJson);

        _mockCache.Setup(c => c.GetAsync("products:all", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(cachedBytes);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);

        var products = okResult.Value as List<ProductDto>;
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
        products[0].Title.Should().Be("Cached Product");

        _mockService.Verify(s => s.GetProductsAsync(), Times.Never);
    }


    [Fact]
    public async Task GetAll_ReturnsFreshData_AndCaches_IfCacheEmpty()
    {
        // Arrange
        var cacheKey = "products:all";

        // Simulate an empty cache (cache miss)
        _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[])null);

        // Mock fresh data from service
        var freshProducts = new List<ProductDto>
    {
        new ProductDto { Id = 2, Title = "Fresh Product" }
    };

        _mockService.Setup(s => s.GetProductsAsync())
                    .ReturnsAsync(freshProducts);

        // Mock setting cache manually (since SetStringAsync is an extension method)
        _mockCache.Setup(c => c.SetAsync(
            cacheKey,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);

        var products = okResult.Value as List<ProductDto>;
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
        products[0].Title.Should().Be("Fresh Product");

        _mockService.Verify(s => s.GetProductsAsync(), Times.Once);
        _mockCache.Verify();
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
