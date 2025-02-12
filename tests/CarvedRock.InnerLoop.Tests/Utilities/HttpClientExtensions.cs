using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.Tests.Utilities;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions _jsonPrintOptions = new() { WriteIndented = true };
    
    private static readonly JsonSerializerOptions _jsonDeserializeOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<T> GetJsonResultAsync<T>(
        this HttpClient httpClient, string requestUri,
        HttpStatusCode expectedStatusCode, ITestOutputHelper testOutputHelper)
    {
        var response = await httpClient.GetAsync(requestUri);
        return await DeserializeAndCheckResponse<T>(response,
            expectedStatusCode, testOutputHelper);
    }

    public static async Task<T> PostForJsonResultAsync<T>(
        this HttpClient httpClient, string requestUri, object content,
        HttpStatusCode expectedStatusCode, ITestOutputHelper testOutputHelper)
    {
        var response = await httpClient.PostAsJsonAsync(requestUri, content);
        return await DeserializeAndCheckResponse<T>(response, expectedStatusCode, testOutputHelper);
    }
    public static async Task<T> DeserializeAndCheckResponse<T>(
        HttpResponseMessage response, HttpStatusCode expectedStatusCode,
        ITestOutputHelper testOutputHelper)
    {
        var stringContent = await response.Content.ReadAsStringAsync();
        try
        {
            var result = JsonSerializer.Deserialize<T>(stringContent, _jsonDeserializeOptions);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.NotNull(result);
            return result;
        }
        catch (Exception)
        {
            WriteOutput(stringContent, testOutputHelper);
            throw;
        }
    }

    private static void WriteOutput(string stringContent,
        ITestOutputHelper testOutputHelper)
    {
        string? outputText;
        try
        {
            var jsonContent = JsonDocument.Parse(stringContent);
            outputText = JsonSerializer.Serialize(jsonContent, _jsonPrintOptions);
        }
        catch
        {
            outputText = stringContent;
        }
        testOutputHelper.WriteLine(outputText);
    }
}