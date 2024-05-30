using Motorent.Application.Common.Abstractions.Messaging;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Messages;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.RegisterMotorcycle;

internal sealed class RegisterMotorcycleCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    ILicensePlateService licensePlateService,
    IMessageBus messageBus)
    : ICommandHandler<RegisterMotorcycleCommand, MotorcycleResponse>
{
    public async Task<Result<MotorcycleResponse>> Handle(RegisterMotorcycleCommand command,
        CancellationToken cancellationToken)
    {
        var year = Year.Create(command.Year);
        var licensePlate = LicensePlate.Create(command.LicensePlate);

        var errors = ErrorCombiner.Combine(year, licensePlate);
        if (errors.Any())
        {
            return errors;
        }

        var result = Motorcycle.CreateAsync(
            id: MotorcycleId.New(),
            model: command.Model,
            year: year.Value,
            licensePlate: licensePlate.Value,
            licensePlateService: licensePlateService,
            cancellationToken: cancellationToken);

        return await result
            .ThenAsync(motorcycle => motorcycleRepository.AddAsync(motorcycle, cancellationToken))
            .ThenAsync(async motorcycle =>
            {
                await PublishMotorcycleRegisteredMessage(motorcycle, cancellationToken);
                return motorcycle;
            })
            .Then(motorcycle => motorcycle.Adapt<MotorcycleResponse>());
    }

    private Task PublishMotorcycleRegisteredMessage(Motorcycle motorcycle, CancellationToken cancellationToken)
    {
        return messageBus.PublishAsync(new MotorcycleRegisteredMessage(
                MotorcycleId: motorcycle.Id.Value,
                Model: motorcycle.Model,
                Year: motorcycle.Year.Value,
                CreatedAt: DateTimeOffset.UtcNow
            ),
            cancellationToken);
    }
}