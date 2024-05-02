using Motorent.Application.Auth.Register;
using Motorent.Contracts.Auth.Requests;

namespace Motorent.Presentation.Auth;

internal sealed class RegisterMotorcycle : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", (
                RegisterRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<RegisterCommand>(),
                    cancellationToken)
                .ToResponseAsync(response => Results.Created(
                    uri: null as Uri,
                    value: response)))
            .AllowAnonymous();
    }
}