using Microsoft.AspNetCore.Mvc.Testing;

namespace CarvedRock.InnerLoop.WebApp.Tests.Utilities;

public class CustomWebAppFactory(SharedFixture fixture) : WebApplicationFactory<Program>
{
    public SharedFixture SharedFixture => fixture;
}