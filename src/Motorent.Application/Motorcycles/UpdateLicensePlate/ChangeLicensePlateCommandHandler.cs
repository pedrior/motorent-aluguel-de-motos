using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.UpdateLicensePlate;

internal sealed class ChangeLicensePlateCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    ILicensePlateService licensePlateService)
    : ICommandHandler<ChangeLicensePlateCommand>
{
    public async Task<Result<Success>> Handle(ChangeLicensePlateCommand command,
        CancellationToken cancellationToken)
    {
        var motorcycle = await motorcycleRepository.FindAsync(new MotorcycleId(command.Id), cancellationToken);
        if (motorcycle is null)
        {
            return MotorcycleErrors.NotFound;
        }

        var licensePlate = LicensePlate.Create(command.LicensePlate);
        if (licensePlate.IsFailure)
        {
            return licensePlate.Errors;
        }

        var result = await motorcycle.ChangeLicensePlateAsync(
            licensePlate.Value,
            licensePlateService,
            cancellationToken);

        return await result.ThenAsync(() => motorcycleRepository.UpdateAsync(motorcycle, cancellationToken));
    }
}