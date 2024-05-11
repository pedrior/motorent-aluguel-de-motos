using System.Text;
using System.Text.Json;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    private static readonly Encoding Encoding = Encoding.UTF8;
    
    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, SerializationOptions.Options);
    
    private static HttpRequestMessage Post(string path, object request) =>
        new(HttpMethod.Post, $"/api/{path}")
        {
            Content = new StringContent(Serialize(request), Encoding, "application/json")
        };
    
    private static HttpRequestMessage Get(string path) => new(HttpMethod.Get, $"/api/{path}");
    
    private static HttpRequestMessage Put(string path, object request) =>
        new(HttpMethod.Put, $"/api/{path}")
        {
            Content = new StringContent(Serialize(request), Encoding, "application/json")
        };
    
    private static HttpRequestMessage Delete(string path) => new(HttpMethod.Delete, $"/api/{path}");
}