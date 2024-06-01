using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Rentals.Responses;

namespace Motorent.Application.Rentals.ListRentals;

public sealed record ListRentalsQuery : IQuery<IEnumerable<RentalSummaryResponse>>;