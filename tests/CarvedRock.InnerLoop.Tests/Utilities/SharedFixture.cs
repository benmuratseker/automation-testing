using CarvedRock.Data;
using CarvedRock.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace CarvedRock.InnerLoop.Tests.Utilities;

public class SharedFixture : IAsyncLifetime
{
    public const string DatabaseName = "InMemTestDb;Mode=Memory;Cache=Shared;";
    public string PostgresConnectionString => _dbContainer.GetConnectionString();
    public List<Product>? OriginalProducts { get; private set; }
    private LocalContext? _dbContext;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("carvedrock")
        .WithUsername("carvedrock")
        .WithPassword("innerloop-ftw")
        .Build();
    
    public async Task InitializeAsync()
    {
        //Postgress -------------
        await _dbContainer.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<LocalContext>()
            .UseNpgsql(PostgresConnectionString);
        _dbContext = new LocalContext(optionsBuilder.Options);
        
        
        //SQLite ----------------
        #region SQLite
        // var options = new DbContextOptionsBuilder<LocalContext>()
        //     .UseSqlite($"Data Source={DatabaseName}")
        //     .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        //     .Options;
        //
        // _dbContext = new LocalContext(options);
        //
        // await _dbContext.Database.EnsureDeletedAsync();
        // await _dbContext.Database.EnsureCreatedAsync();
        // await _dbContext.Database.OpenConnectionAsync();
       
        #endregion
        
        await _dbContext.Database.MigrateAsync();
        _dbContext.InitializeTestData(15);
        
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