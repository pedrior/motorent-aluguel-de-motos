using Motorent.Application.Auth.Login;
using Motorent.Contracts.Auth.Requests;
using Motorent.Contracts.Auth.Responses;

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
            .AllowAnonymous()
            .WithName("Login")
            .WithTags("Auth")
            .WithSummary("Autentica um usuário no sistema")
            .Produces<TokenResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi();
    }
}