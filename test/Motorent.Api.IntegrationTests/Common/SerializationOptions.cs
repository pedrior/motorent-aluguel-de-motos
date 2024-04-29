using System.Text.Json;

namespace Motorent.Api.IntegrationTests.Common;

internal static class SerializationOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
}