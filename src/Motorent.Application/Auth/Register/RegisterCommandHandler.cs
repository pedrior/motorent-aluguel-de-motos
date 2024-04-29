using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Application.Auth.Register;

internal sealed class RegisterCommandHandler(IUserService userService, ISecurityTokenProvider securityTokenProvider)
    : ICommandHandler<RegisterCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = userService.CreateUserAsync(
            command.Email,
            command.Password,
            roles: [UserRoles.Renter],
            claims: new Dictionary<string, string>
            {
                [UserClaimTypes.GivenName] = command.GivenName,
                [UserClaimTypes.FamilyName] = command.FamilyName,
                [UserClaimTypes.Birthdate] = command.Birthdate.ToString("yyyy-MM-dd")
            },
            cancellationToken: cancellationToken);

        return await result.ThenAsync(userId => GenerateSecurityTokenAsync(userId, cancellationToken))
            .Then(securityToken => securityToken.Adapt<TokenResponse>());
    }

    private Task<SecurityToken> GenerateSecurityTokenAsync(string userId, CancellationToken cancellationToken) =>
        securityTokenProvider.GenerateSecurityTokenAsync(userId, cancellationToken);
}