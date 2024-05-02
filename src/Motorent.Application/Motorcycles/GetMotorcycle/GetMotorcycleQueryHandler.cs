using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.GetMotorcycle;

internal sealed class GetMotorcycleQueryHandler(IMotorcycleRepository motorcycleRepository)
    : IQueryHandler<GetMotorcycleQuery, MotorcycleResponse>
{
    public async Task<Result<MotorcycleResponse>> Handle(GetMotorcycleQuery query,
        CancellationToken cancellationToken)
    {
        Motorcycle? motorcycle;
        if (Ulid.TryParse(query.IdOrLicensePlate, out var id))
        {
            motorcycle = await motorcycleRepository.FindAsync(new MotorcycleId(id), cancellationToken);
        }
        else
        {
            var licensePlate = LicensePlate.Create(query.IdOrLicensePlate);
            if (licensePlate.IsFailure)
            {
                return licensePlate.Errors;
            }

            motorcycle = await motorcycleRepository.FindByLicensePlateAsync(licensePlate.Value, cancellationToken);
        }

        return motorcycle is not null
            ? motorcycle.Adapt<MotorcycleResponse>()
            : MotorcycleErrors.NotFound;
    }
}