namespace Motorent.Contracts.Motorcycles.Requests;

public sealed record ChangeLicensePlateRequest
{
    public string LicensePlate { get; init; } = null!;
}