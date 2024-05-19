namespace Motorent.Contracts.Renters.Requests;

public sealed record UpdateDriverLicenseRequest
{
    public string Number { get; init; } = null!;
    
    public string Category { get; init; } = null!;

    public DateOnly Expiry { get; init; }
}