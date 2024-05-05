namespace Motorent.Contracts.Motorcycles.Requests;

public sealed record ListMotorcyclesRequest
{
    public int Page { get; init; } = 1;

    public int Limit { get; init; } = 15;
    
    public string? Sort { get; init; }
    
    public string? Order { get; init; }
    
    public string? Search { get; init; }
}