using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Rentals.ListRentals;

internal sealed class ListRentalsQueryHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IRentalRepository rentalRepository,
    IMotorcycleRepository motorcycleRepository) : IQueryHandler<ListRentalsQuery, PageResponse<RentalSummaryResponse>>
{
    public async Task<Result<PageResponse<RentalSummaryResponse>>> Handle(ListRentalsQuery query,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}.");
        }

        var totalRentals = await rentalRepository.CountRentalsByRenterAsync(renter.Id, cancellationToken);
        if (totalRentals is 0)
        {
            return PageResponse<RentalSummaryResponse>.Empty(query.Page, query.Limit);
        }

        var rentals = await rentalRepository.ListRentalsByRenterAsync(
            renter.Id, query.Page, query.Limit, cancellationToken);

        var response = new List<RentalSummaryResponse>();
        foreach (var rental in rentals)
        {
            var motorcycle = await motorcycleRepository.FindAsync(rental.MotorcycleId, cancellationToken);
            if (motorcycle is null)
            {
                throw new ApplicationException($"Motorcycle with id {rental.MotorcycleId} not found.");
            }

            response.Add(rental.Adapt<RentalSummaryResponse>() with
            {
                Motorcycle = new RentalMotorcycleResponse
                {
                    Id = motorcycle.Id.ToString(),
                    Model = motorcycle.Model,
                    Year = motorcycle.Year.Value,
                    LicensePlate = motorcycle.LicensePlate.Value
                }
            });
        }

        return new PageResponse<RentalSummaryResponse>
        {
            Page = query.Page,
            Limit = query.Limit,
            TotalItems = totalRentals,
            TotalPages = (int)Math.Ceiling((double)totalRentals / query.Limit),
            Items = response
        };
    }
}