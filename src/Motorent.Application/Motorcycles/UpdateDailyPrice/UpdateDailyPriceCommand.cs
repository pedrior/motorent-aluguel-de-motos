using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Motorcycles.UpdateDailyPrice;

public sealed record UpdateDailyPriceCommand : ICommand, ITransactional
{ 
    public required Ulid Id { get; init; }
    
    public decimal DailyPrice { get; init; }
}