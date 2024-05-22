using Motorent.Domain.Common.Entities;
using Motorent.Domain.Motorcycles.Errors;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles;

public sealed class Motorcycle : Entity<MotorcycleId>, IAggregateRoot, IAuditable
{
    private Motorcycle(MotorcycleId id) : base(id)
    {
    }

    public string Model { get; private set; } = null!;
    
    public Year Year { get; private set; } = null!;
    
    public LicensePlate LicensePlate { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public static async Task<Result<Motorcycle>> CreateAsync(
        MotorcycleId id,
        string model,
        Year year,
        LicensePlate licensePlate,
        ILicensePlateService licensePlateService,
        CancellationToken cancellationToken = default)
    {
        if (!await licensePlateService.IsUniqueAsync(licensePlate, cancellationToken))
        {
            return MotorcycleErrors.LicensePlateNotUnique(licensePlate);
        }

        return new Motorcycle(id)
        {
            Model = model,
            Year = year,
            LicensePlate = licensePlate
        };
    }
    
    public async Task<Result<Success>> ChangeLicensePlateAsync(
        LicensePlate licensePlate,
        ILicensePlateService licensePlateService,
        CancellationToken cancellationToken = default)
    {
        if (licensePlate == LicensePlate)
        {
            return Success.Value;
        }
        
        if (!await licensePlateService.IsUniqueAsync(licensePlate, cancellationToken))
        {
            return MotorcycleErrors.LicensePlateNotUnique(licensePlate);
        }

        LicensePlate = licensePlate;

        return Success.Value;
    }
}