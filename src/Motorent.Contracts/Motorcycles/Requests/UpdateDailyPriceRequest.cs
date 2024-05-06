namespace Motorent.Contracts.Motorcycles.Requests;

public sealed record UpdateDailyPriceRequest
{
    public decimal DailyPrice { get; init; }
}