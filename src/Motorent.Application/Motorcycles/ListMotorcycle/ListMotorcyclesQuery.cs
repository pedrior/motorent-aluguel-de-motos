using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Application.Motorcycles.ListMotorcycle;

public sealed record ListMotorcyclesQuery : IQuery<PageResponse<MotorcycleResponse>>
{
    public int Page { get; init; }
    
    public int Limit { get; init; }
    
    public string? Sort { get; init; }
    
    public string? Order { get; init; }
    
    public string? Search { get; init; }
}