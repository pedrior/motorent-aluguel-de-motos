using System.Text.Json.Serialization;

namespace Motorent.Contracts.Renters.Responses;

public sealed record RenterCNHResponse
{
    public string Status { get; init; } = null!;
    
    public string Number { get; init; } = null!;

    public string Category { get; init; } = null!;
    
    [JsonPropertyName("exp")]
    public DateOnly ExpirationDate { get; init; }
    
    public string? FrontImage { get; init; }
    
    public string? BackImage { get; init; }
}