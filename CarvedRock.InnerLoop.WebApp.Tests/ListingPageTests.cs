using System.Net;
using CarvedRock.InnerLoop.WebApp.Tests.Utilities;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.WebApp.Tests;

[Collection(nameof(InnerLoopCollection))]
public class ListingPageTests(CustomWebAppFactory factory, ITestOutputHelper outputHelper) : IClassFixture<CustomWebAppFactory>
{
    [InlineData("boots")]
    [InlineData("kayak")]
    [InlineData("equip")]
    [InlineData("nothing")]
    [Theory]
    public async Task GetListingPage(string category)
    {
        var client = factory.CreateClient();
        var pageResponse = await client.GetAsync($"/listing?cat={category}");
        var page = await HtmlHelpers.GetDocumentAsync(pageResponse);
        
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        
        outputHelper.WriteLine(page.Body!.OuterHtml);

        var productNameCells = page.QuerySelectorAll("tr>td:first-child");
        var productNames = productNameCells.Select(c => c.TextContent.Trim());
        var buttons = page.QuerySelectorAll("tr>td>button");
        
        Assert.Equal(factory.SharedFixture.OriginalProducts.Count(p => p.Category == category), productNames.Count());

        foreach (var expectedProduct in factory.SharedFixture.OriginalProducts.Where(p => p.Category == category))
        {
            Assert.Contains(expectedProduct.Name, productNames.ToList());
        }

        foreach (var button in buttons)
        {
            Assert.Contains("Add to Cart", button.TextContent);
        }
    }
}