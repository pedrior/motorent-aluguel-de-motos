using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Enums;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.Register;

internal sealed class RegisterMotorcycleCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    ILicensePlateService licensePlateService)
    : ICommandHandler<RegisterMotorcycleCommand, MotorcycleResponse>
{
    public async Task<Result<MotorcycleResponse>> Handle(RegisterMotorcycleCommand command,
        CancellationToken cancellationToken)
    {
        var brand = Brand.FromName(command.Brand);
        var year = Year.Create(command.Year);
        var dailyPrice = Money.Create(command.DailyPrice);
        var licensePlate = LicensePlate.Create(command.LicensePlate);

        var errors = ErrorCombiner.Combine(year, dailyPrice, licensePlate);
        if (errors.Any())
        {
            return errors;
        }

        var result = Motorcycle.CreateAsync(
            id: MotorcycleId.New(),
            model: command.Model,
            brand: brand,
            year: year.Value,
            dailyPrice: dailyPrice.Value,
            licensePlate: licensePlate.Value,
            licensePlateService: licensePlateService,
            cancellationToken: cancellationToken);

        return await result
            .ThenAsync(motorcycle => motorcycleRepository.AddAsync(motorcycle, cancellationToken))
            .Then(motorcycle => motorcycle.Adapt<MotorcycleResponse>());
    }
}