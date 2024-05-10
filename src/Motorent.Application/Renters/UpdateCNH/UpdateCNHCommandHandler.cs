using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.UpdateCNH;

internal sealed class UpdateCNHCommandHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    ICNHService cnhService) : ICommandHandler<UpdateCNHCommand>
{
    public async Task<Result<Success>> Handle(UpdateCNHCommand command, CancellationToken cancellationToken)
    {
        var cnh = CNH.Create(
            number: command.Number,
            category: CNHCategory.FromName(command.Category, ignoreCase: true),
            expirationDate: command.ExpDate);

        if (cnh.IsFailure)
        {
            return cnh.Errors;
        }

        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        return await renter.ChangeCNHAsync(cnh.Value, cnhService, cancellationToken)
            .ThenAsync(() => renterRepository.UpdateAsync(renter, cancellationToken));
    }
}