using System.Text.Json;

namespace Motorent.Api.IntegrationTests.TestUtils;

internal static class SerializationOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
}