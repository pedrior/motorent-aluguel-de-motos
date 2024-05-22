using System.Text;
using System.Text.Json;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    
    private static HttpRequestMessage Post(string path, object request)
    {
        return new HttpRequestMessage(HttpMethod.Post, $"/api/{path}")
        {
            Content = ToContent(request)
        };
    }

    private static HttpRequestMessage Get(string path) => new(HttpMethod.Get, $"/api/{path}");

    private static HttpRequestMessage Put(string path, object request)
    {
        return new HttpRequestMessage(HttpMethod.Put, $"/api/{path}")
        {
            Content = ToContent(request)
        };
    }

    private static HttpRequestMessage Delete(string path) => new(HttpMethod.Delete, $"/api/{path}");
    
    private static StringContent ToContent(object request) => new(
        content: JsonSerializer.Serialize(request, SerializerOptions),
        encoding: Encoding.UTF8,
        mediaType: "application/json");
}