using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Application.Auth.Register;

public sealed record RegisterCommand : ICommand<TokenResponse>, ITransactional
{
    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string GivenName { get; init; }

    public required string FamilyName { get; init; }

    public required DateOnly Birthdate { get; init; }

    public required string Document { get; init; }

    public required string CNHNumber { get; init; }

    public required string CNHCategory { get; init; }

    public required DateOnly CNHExpDate { get; init; }
}