using CarvedRock.Data;
using CarvedRock.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.InnerLoop.Tests.Utilities;

public class SharedFixture : IAsyncLifetime
{
    public const string DatabaseName = "InMemTestDb;Mode=Memory;Cache=Shared;";
    public List<Product>? OriginalProducts { get; private set; }
    private LocalContext? _dbContext;
    
    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<LocalContext>()
            .UseSqlite($"Data Source={DatabaseName}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        _dbContext = new LocalContext(options);
        

        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        await _dbContext.Database.OpenConnectionAsync();
        await _dbContext.Database.MigrateAsync();
        
        _dbContext.InitializeTestData(20);
        
        OriginalProducts = await _dbContext.Products.ToListAsync();
    }

    public async Task DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
    }
}

[CollectionDefinition(nameof(InnerLoopCollection))]
public class InnerLoopCollection : ICollectionFixture<SharedFixture>
{
    //This class has no code, and is never created. Its purpose is simply to be the place
    //to apply [CollectionDefinition] and all the ICollectionFixture<> interface
}