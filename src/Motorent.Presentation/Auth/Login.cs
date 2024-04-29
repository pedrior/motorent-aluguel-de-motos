using Motorent.Application.Auth.Login;
using Motorent.Contracts.Auth.Requests;

namespace Motorent.Presentation.Auth;

internal sealed class Login : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", (
                LoginRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<LoginCommand>(),
                    cancellationToken)
                .ToResponseAsync(Results.Ok))
            .AllowAnonymous();
    }
}