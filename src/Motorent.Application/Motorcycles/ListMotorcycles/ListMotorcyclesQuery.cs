using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Application.Motorcycles.ListMotorcycles;

public sealed record ListMotorcyclesQuery : IQuery<PageResponse<MotorcycleSummaryResponse>>
{
    public int Page { get; init; }
    
    public int Limit { get; init; }
    
    public string? Sort { get; init; }
    
    public string? Order { get; init; }
    
    public string? Search { get; init; }
}