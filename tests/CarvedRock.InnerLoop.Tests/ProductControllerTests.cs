using System.Net;
using System.Text.Json;
using Bogus;
using CarvedRock.Core;
using CarvedRock.InnerLoop.Tests.Utilities;
using Microsoft.AspNetCore.Mvc;
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
    
    [Fact]
    public async Task Get_Products_Success()
    {
        // Arrange
        var client = factory.CreateClient();
        var products = await client.GetJsonResultAsync<IEnumerable<ProductModel>>("/product?category=all", HttpStatusCode.OK, outputHelper);
        
        // Assert
        Assert.Equal(6, products.Count());
    }
    
    [Fact]
    public async Task Get_Products_TroubleGivesProblemDetails()
    {
        // Arrange
        var client = factory.CreateClient();
        var problemDetail = await client.GetJsonResultAsync<ProblemDetails>("/product?category=trouble", HttpStatusCode.InternalServerError, outputHelper);
        
        // Assert
        Assert.NotNull(problemDetail.Title);
        Assert.NotNull(problemDetail.Detail);
        Assert.Contains("traceId", problemDetail.Extensions.Keys);
    }
    
    [Fact]
    public async Task PostProductValidationFailure()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Authorization", "Erik Smith");
        client.DefaultRequestHeaders.Add("X-Test-idp", "Google");

        var newProduct = _newProductFaker.Generate();
        newProduct.Name = ""; // invalid

        var problem = await client.PostForJsonResultAsync<ProblemDetails>
            ("/product", newProduct, HttpStatusCode.BadRequest, outputHelper);

        Assert.NotNull(problem);
        Assert.Equal("One or more validation errors occurred.", problem.Detail);
        Assert.Contains("Name", problem.Extensions.Keys);
        Assert.Contains("Name is required.", problem.Extensions["Name"]!.ToString());
    }
    
    private readonly Faker<NewProductModel> _newProductFaker = new Faker<NewProductModel>()
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Category, f => f.PickRandom("boots", "equip", "kayak"))
        .RuleFor(p => p.Price, (f, p) =>
            p.Category == "boots" ? f.Random.Double(50, 300) :
            p.Category == "equip" ? f.Random.Double(20, 150) :
            p.Category == "kayak" ? f.Random.Double(100, 500) : 0)
        .RuleFor(p => p.ImgUrl, f => f.Image.PicsumUrl());
}