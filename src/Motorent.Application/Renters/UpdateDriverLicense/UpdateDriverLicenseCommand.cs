using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Renters.UpdateDriverLicense;

public sealed record UpdateDriverLicenseCommand : ICommand, ITransactional
{
    public required string Number { get; init; }
    
    public required string Category { get; init; }
    
    public required DateOnly Expiry { get; init; }
}