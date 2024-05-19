namespace Motorent.Contracts.Renters.Responses;

public sealed record RenterProfileResponse
{
    public string Document { get; init; } = null!;
    
    public string Email { get; init; } = null!;
    
    public string FullName { get; init; } = null!;
    
    public DateOnly Birthdate { get; init; }
       
    public RenterCNHResponse CNH { get; init; } = null!;
}