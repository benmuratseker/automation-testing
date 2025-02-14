using CarvedRock.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.InnerLoop.Tests.Utilities;

// public class CustomApiFactory : WebApplicationFactory<Program>
public class CustomApiFactory (SharedFixture fixture): WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("innerloop-test");
        builder.ConfigureTestServices(services =>
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { }));
        
        //
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LocalContext>));
            services.Remove(dbContextDescriptor!);
            
            var ctx = services.SingleOrDefault(d => d.ServiceType == typeof(LocalContext));
            services.Remove(ctx!);
            
            //add back the container-based dbContext
            services.AddDbContext<LocalContext>(opts =>
                opts.UseSqlite($"Data Source{SharedFixture.DatabaseName}")
                    .UseQueryTrackingBehavior(QueryTrackingBehavior
                        .NoTracking));
        });
    }
}