using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.UpdatePersonalInformation;

internal sealed class UpdatePersonalInformationCommandHandler(
    IUserContext userContext,
    IRenterRepository renterRepository) : ICommandHandler<UpdatePersonalInformationCommand>
{
    public async Task<Result<Success>> Handle(UpdatePersonalInformationCommand command,
        CancellationToken cancellationToken)
    {
        var fullName = new FullName(command.GivenName, command.FamilyName);
        var birthdate = Birthdate.Create(command.Birthdate);

        if (birthdate.IsFailure)
        {
            return birthdate.Errors;
        }

        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        renter.ChangePersonalInformation(fullName, birthdate.Value);
        
        await renterRepository.UpdateAsync(renter, cancellationToken);

        return Success.Value;
    }
}