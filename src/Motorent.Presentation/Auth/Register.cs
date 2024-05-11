using Motorent.Application.Auth.Register;
using Motorent.Contracts.Auth.Requests;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Presentation.Auth;

internal sealed class Register : IEndpoint
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
            .AllowAnonymous()
            .WithName("Register")
            .WithTags("Auth")
            .WithSummary("Registers a new user in the system")
            .Produces<TokenResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi();
    }
}