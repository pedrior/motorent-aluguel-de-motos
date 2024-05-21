using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Rentals.Common.Errors;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Rentals.Rent;

internal sealed class RentCommandHandler(
    IUserContext userContext,
    IRentalFactory rentalFactory,
    IRenterRepository renterRepository,
    IMotorcycleRepository motorcycleRepository,
    IRentalRepository rentalRepository) : ICommandHandler<RentCommand, RentalResponse>
{
    public async Task<Result<RentalResponse>> Handle(RentCommand command, CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}.");
        }

        var motorcycleId = new MotorcycleId(command.MotorcycleId);
        if (!await motorcycleRepository.ExistsAsync(motorcycleId, cancellationToken))
        {
            return RentalErrors.MotorcycleNotFound(motorcycleId);
        }

        var plan = RentalPlan.FromName(command.Plan, ignoreCase: true);
        
        return await rentalFactory.Create(renter, RentalId.New(), motorcycleId, plan)
            .ThenAsync(rental => rentalRepository.AddAsync(rental, cancellationToken))
            .ThenAsync(rental => ToResponseAsync(rental, cancellationToken));
    }

    private async Task<RentalResponse> ToResponseAsync(Rental rental, CancellationToken cancellationToken)
    {
        var motorcycle = await motorcycleRepository.FindAsync(rental.MotorcycleId, cancellationToken);
        if (motorcycle is null)
        {
            throw new ApplicationException($"Motorcycle not found for rental {rental.Id}.");
        }

        return rental.Adapt<RentalResponse>() with
        {
            Motorcycle = new RentalMotorcycleResponse
            {
                Id = motorcycle.Id.Value,
                Model = motorcycle.Model,
                Year = motorcycle.Year.Value,
                LicensePlate = motorcycle.LicensePlate.ToString()
            }
        };
    }
}