using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Motorcycles.UpdateLicensePlate;

public sealed record ChangeLicensePlateCommand : ICommand, ITransactional
{
    public required Ulid Id { get; init; }
    
    public required string LicensePlate { get; init; }
}