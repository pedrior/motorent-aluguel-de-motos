namespace Motorent.Contracts.Motorcycles.Requests;

public sealed record UpdateLicensePlateRequest
{
    public string LicensePlate { get; init; } = null!;
}