using System.Text.Json;

namespace Motorent.Api.IntegrationTests.TestUtils.Extensions;

internal static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, SerializerOptions)
            ?? throw new Exception($"Failed to deserialize '{content}' to '{typeof(T).Name}'");
    }
}