using Microsoft.Extensions.Configuration;
using WireMock.Server;
using WireMock.Settings;

IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var settings = new WireMockServerSettings
{
    Urls = new[] { "https://localhost:9095/" },
    StartAdminInterface = true,
    ProxyAndRecordSettings = new ProxyAndRecordSettings
    {
        Url = "https://api.themoviedb.org",
        SaveMapping = true,
        SaveMappingToFile = true,
        SaveMappingForStatusCodePattern = "2xx",
        ExcludedHeaders = ["Authorization"]
    }
};

var server = WireMockServer.Start(settings);

using var client = new HttpClient(){BaseAddress = new Uri(server.Urls[0]) };
client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.GetValue<string>("MovieApiKey")}");

var response = await client.GetAsync("/3/movie/top_rated");