using Motorent.Application.Auth.Login;
using Motorent.Application.Auth.Register;
using Motorent.Contracts.Auth.Requests;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Presentation.Endpoints;

internal sealed class AuthEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("auth")
            .WithTags("Auth")
            .WithOpenApi();

        group.MapPost("login", Login)
            .WithName(nameof(Login))
            .WithSummary("Realiza o login de um usuário no sistema")
            .Produces<TokenResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("register", Register)
            .WithName(nameof(Register))
            .WithSummary("Registra um novo usuário no sistema")
            .Produces<TokenResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static Task<IResult> Login(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<LoginCommand>(), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }
    
    private static Task<IResult> Register(
        RegisterRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<RegisterCommand>(), cancellationToken)
            .ToResponseAsync(response => Results.Created(uri: null as Uri, value: response));
    }
}