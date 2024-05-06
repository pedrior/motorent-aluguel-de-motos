using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Renters.UpdatePersonalInformation;

public sealed record UpdatePersonalInformationCommand : ICommand, ITransactional
{
    public required string GivenName { get; init; }

    public required string FamilyName { get; init; }

    public required DateOnly Birthdate { get; init; }
}