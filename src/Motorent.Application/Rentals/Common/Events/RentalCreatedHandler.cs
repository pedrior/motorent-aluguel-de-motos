using Microsoft.Extensions.Logging;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Events;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Rentals.Common.Events;

internal sealed class RentalCreatedHandler(
    IRentalRepository rentalRepository,
    IRenterRepository renterRepository,
    ILogger<RentalCreatedHandler> logger) : IEventHandler<RentalCreated>
{
    public async Task Handle(RentalCreated e, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling domain event {EventName} ({@EventContent}",
            e.GetType().Name, e);

        var rental = await GetRentalAsync(e.RentalId, cancellationToken);
        var renter = await GetRenterAsync(rental.RenterId, cancellationToken);

        var result = renter.AddRental(rental);
        if (result.IsFailure)
        {
            throw new ApplicationException(
                $"Failed to add rental with ID '{rental.Id}' to renter with ID '{renter.Id}'." +
                $"\nError: {result.FirstError}");
        }

        await renterRepository.UpdateAsync(renter, cancellationToken);
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

    private async Task<Renter> GetRenterAsync(RenterId renterId, CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindAsync(renterId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter with ID '{renterId}' not found.");
        }

        return renter;
    }
}