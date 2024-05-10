using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Renters.UpdateCNH;

public sealed record UpdateCNHCommand : ICommand, ITransactional
{
    public required string Number { get; init; }
    
    public required string Category { get; init; }
    
    public required DateOnly ExpDate { get; init; }
}