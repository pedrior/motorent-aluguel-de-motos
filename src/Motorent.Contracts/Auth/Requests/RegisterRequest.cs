namespace Motorent.Contracts.Auth.Requests;

public sealed record RegisterRequest
{
    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;

    public string GivenName { get; init; } = null!;

    public string FamilyName { get; init; } = null!;

    public DateOnly Birthdate { get; init; }
    
    public string Document { get; init; } = null!;
    
    public string DriverLicenseNumber { get; init; } = null!;
    
    public string DriverLicenseCategory { get; init; } = null!;
    
    public DateOnly DriverLicenseExpiry { get; init; }
}