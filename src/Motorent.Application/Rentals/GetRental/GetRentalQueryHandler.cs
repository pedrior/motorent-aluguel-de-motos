using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Rentals.Common.Errors;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Rentals.GetRental;

internal sealed class GetRentalQueryHandler(
    IRentalRepository rentalRepository,
    IMotorcycleRepository motorcycleRepository)
    : IQueryHandler<GetRentalQuery, RentalResponse>
{
    public async Task<Result<RentalResponse>> Handle(GetRentalQuery query, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.FindAsync(new RentalId(query.Id), cancellationToken);
        if (rental is null)
        {
            return RentalErrors.NotFound;
        }

        var motorcycle = await motorcycleRepository.FindAsync(rental.MotorcycleId, cancellationToken);
        if (motorcycle is null)
        {
            throw new ApplicationException($"Motorcycle with id {rental.MotorcycleId} not found.");
        }

        return rental.Adapt<RentalResponse>() with
        {
            Motorcycle = new RentalMotorcycleResponse
            {
                Id = motorcycle.Id.ToString(),
                Model = motorcycle.Model,
                Year = motorcycle.Year.Value,
                LicensePlate = motorcycle.LicensePlate.Value
            }
        };
    }
}