using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Application.Auth.Login;

internal sealed class LoginCommandHandler(IUserService userService, ISecurityTokenProvider securityTokenProvider)
    : ICommandHandler<LoginCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        return await userService.CheckPasswordAsync(command.Email, command.Password, cancellationToken)
            .ThenAsync(userId => securityTokenProvider.GenerateSecurityTokenAsync(userId, cancellationToken))
            .Then(securityToken => securityToken.Adapt<TokenResponse>());
    }
}