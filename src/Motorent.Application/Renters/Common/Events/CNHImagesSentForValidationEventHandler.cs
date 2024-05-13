using Motorent.Domain.Renters.Events;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Renters.Common.Events;

internal sealed class CNHImagesSentForValidationEventHandler(IRenterRepository renterRepository)
    : IEventHandler<CNHImagesSentForValidationEvent>
{
    public async Task Handle(CNHImagesSentForValidationEvent e, CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindAsync(e.RenterId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter {e.RenterId} not found");
        }

        // Apenas aprova ou rejeita aleatoriamente por enquanto
        
        var shouldApprove = Random.Shared.Next(0, 11) switch
        {
            > 3 => true,
            _ => false
        };

        var result = shouldApprove
            ? renter.SetCNHApprovedStatus()
            : renter.SetCNHRejectedStatus();

        await result.ThenAsync(() => renterRepository.UpdateAsync(renter, cancellationToken))
            .Else(error => throw new ApplicationException(error.First().ToString()));
    }
}