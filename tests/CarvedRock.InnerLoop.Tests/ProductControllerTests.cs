using System.Net;
using System.Text.Json;
using CarvedRock.Core;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarvedRock.InnerLoop.Tests;

public class ProductControllerTests(WebApplicationFactory<Program> factory, 
    ITestOutputHelper outputHelper) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetProducts_Success()
    {
        // Arrange
        var client = factory.CreateClient();
        var response = await client.GetAsync("/product?category=all");
        var content = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductModel>>(content);
        
        outputHelper.WriteLine(content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("boots", content);
    }
}