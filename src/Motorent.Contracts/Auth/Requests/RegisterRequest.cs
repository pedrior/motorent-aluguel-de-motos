using System.Text.Json.Serialization;

namespace Motorent.Contracts.Auth.Requests;

public sealed record RegisterRequest
{
    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;

    public string GivenName { get; init; } = null!;

    public string FamilyName { get; init; } = null!;

    public DateOnly Birthdate { get; init; }
    
    public string Document { get; init; } = null!;
    
    [JsonPropertyName("cnh_number")]
    public string CNHNumber { get; init; } = null!;
    
    [JsonPropertyName("cnh_category")]
    public string CNHCategory { get; init; } = null!;
    
    [JsonPropertyName("cnh_exp_date")]
    public DateOnly CNHExpDate { get; init; }
}