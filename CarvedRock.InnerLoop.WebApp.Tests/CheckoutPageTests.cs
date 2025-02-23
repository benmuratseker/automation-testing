using System.Net;
using AngleSharp.Io;
using CarvedRock.InnerLoop.WebApp.Tests.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.WebApp.Tests;

[Collection(nameof(InnerLoopCollection))]
public class CheckoutPageTests(CustomWebAppFactory factory, ITestOutputHelper outputHelper) : IClassFixture<CustomWebAppFactory>
{
    [Fact]
    public async Task GetCheckoutPageAsAnonymousRedirectToSignIn()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        var pageResponse = await client.GetAsync("/checkout");
        
        Assert.Equal(HttpStatusCode.Redirect, pageResponse.StatusCode);
        
        var authority = pageResponse.Headers.Location!.Authority;
        Assert.Equal("demo.duendesoftware.com", authority);
    }

    [Fact]
    public async Task GetLoggedInEmptyCheckoutPage()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        client.DefaultRequestHeaders.Add("X-Authorization", "Erik Smith");
        client.DefaultRequestHeaders.Add("X-Test-idp", "Google");
        client.DefaultRequestHeaders.Add("X-Test-email", "erik@test.com");
        
        var pageResponse = await client.GetAsync("/checkout");
        
        Assert.Equal(HttpStatusCode.OK, pageResponse.StatusCode);
    }

    [Fact]
    public async Task GetLoggedInCheckoutPageWithItems()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        client.DefaultRequestHeaders.Add("X-Authorization", "Erik Smith");
        client.DefaultRequestHeaders.Add("X-Test-idp", "Google");
        client.DefaultRequestHeaders.Add("X-Test-email", "erik@test.com");

        var (cookieHeader, expectedGrandTotal) =
            GetCookieHeaderValueAndGrandTotal();
        client.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookieHeader);
        
        var pageResponse = await client.GetAsync("/cart");
        var page = await HtmlHelpers.GetDocumentAsync(pageResponse);
        
        Assert.Equal(HttpStatusCode.OK, pageResponse.StatusCode);
        Assert.Equal($"{expectedGrandTotal:C}", page.QuerySelector("[id=grand-total]")!.TextContent);
        
        outputHelper.WriteLine(page.Body!.OuterHtml);
    }

    private (string cookieHeader, double expectedGrandTotal) GetCookieHeaderValueAndGrandTotal()
    {
        throw new NotImplementedException();
    }
}