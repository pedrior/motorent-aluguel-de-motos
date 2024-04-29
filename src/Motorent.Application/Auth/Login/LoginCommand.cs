using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Application.Auth.Login;

public sealed record LoginCommand : ICommand<TokenResponse>
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}