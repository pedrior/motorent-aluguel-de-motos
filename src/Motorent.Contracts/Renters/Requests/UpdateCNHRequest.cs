namespace Motorent.Contracts.Renters.Requests;

public sealed record UpdateCNHRequest
{
    public string Number { get; init; } = null!;
    
    public string Category { get; init; } = null!;

    public DateOnly ExpDate { get; init; }
}