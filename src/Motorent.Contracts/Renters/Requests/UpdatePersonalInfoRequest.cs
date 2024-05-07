namespace Motorent.Contracts.Renters.Requests;

public sealed record UpdatePersonalInfoRequest
{
    public string GivenName { get; init; } = null!;
    
    public string FamilyName { get; init; } = null!;
    
    public DateOnly Birthdate { get; init; }
}