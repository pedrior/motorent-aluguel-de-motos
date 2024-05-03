namespace Motorent.Contracts.Motorcycles.Requests;

public sealed record ListMotorcyclesRequest
{
    public int Page { get; init; } = 1;

    public int Limit { get; init; } = 15;
    
    public string? Sort { get; init; } = "model";
    
    public string? Order { get; init; } = "asc";
    
    public string? Search { get; init; }
}