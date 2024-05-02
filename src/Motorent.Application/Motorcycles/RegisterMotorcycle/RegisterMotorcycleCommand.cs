using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Application.Motorcycles.RegisterMotorcycle;

public sealed record RegisterMotorcycleCommand : ICommand<MotorcycleResponse>, ITransactional
{
    public required string Model { get; init; }
    
    public required string Brand { get; init; }
    
    public required int Year { get; init; }
    
    public required decimal DailyPrice { get; init; }
    
    public required string LicensePlate { get; init; }
}