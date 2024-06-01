using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Rentals.ListRentals;

internal sealed class ListRentalsQueryHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IRentalRepository rentalRepository,
    IMotorcycleRepository motorcycleRepository) : IQueryHandler<ListRentalsQuery, IEnumerable<RentalSummaryResponse>>
{
    public async Task<Result<IEnumerable<RentalSummaryResponse>>> Handle(ListRentalsQuery query,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}.");
        }

        if (!renter.HasAnyRental)
        {
            return Enumerable.Empty<RentalSummaryResponse>().ToList();
        }

        var summaries = new List<RentalSummaryResponse>();
        foreach (var rentalId in renter.RentalIds)
        {
            var rental = await GetRentalAsync(rentalId, cancellationToken);
            var motorcycle = await GetMotorcycleAsync(rental.MotorcycleId, cancellationToken);

            summaries.Add(rental.Adapt<RentalSummaryResponse>() with
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

        return summaries;
    }

    private async Task<Rental> GetRentalAsync(RentalId rentalId, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.FindAsync(rentalId, cancellationToken);
        if (rental is null)
        {
            throw new ApplicationException($"Rental with ID '{rentalId}' not found.");
        }

        return rental;
    }

    private async Task<Motorcycle> GetMotorcycleAsync(MotorcycleId motorcycleId, CancellationToken cancellationToken)
    {
        var motorcycle = await motorcycleRepository.FindAsync(motorcycleId, cancellationToken);
        if (motorcycle is null)
        {
            throw new ApplicationException($"Motorcycle with ID '{motorcycleId}' not found.");
        }

        return motorcycle;
    }
}