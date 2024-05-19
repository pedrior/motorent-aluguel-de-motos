namespace Motorent.Contracts.Renters.Responses;

public sealed record RenterDriverLicenseResponse
{
    public string Status { get; init; } = null!;
    
    public string Number { get; init; } = null!;

    public string Category { get; init; } = null!;
    
    public DateOnly Expiry { get; init; }
    
    public string? ImageUrl { get; init; }
}