using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.WebApp.Tests;

public class HomePageTests(WebApplicationFactory<Program> factory, ITestOutputHelper outputHelper) 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly List<string> _alwaysPresentNavItems =
        ["Footwear", "Kayaks", "Equipments", "Cart", "IdSrv"];
    
    [Fact]
    public async Task GetHomePage()
    {
        var client = factory.CreateClient();
        var homePageResponse = await client.GetAsync("/");
        
        Assert.Equal(HttpStatusCode.OK, homePageResponse.StatusCode);
        
        var homePageContent = await homePageResponse.Content.ReadAsStringAsync();
        outputHelper.WriteLine(homePageContent);
    }
}