using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Rentals.Responses;

namespace Motorent.Application.Rentals.ListRentals;

public sealed record ListRentalsQuery : IQuery<PageResponse<RentalSummaryResponse>>
{
    public int Page { get; init; }
    
    public int Limit { get; init; }
}