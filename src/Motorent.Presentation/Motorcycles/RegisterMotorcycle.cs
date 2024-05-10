using Motorent.Application.Motorcycles.RegisterMotorcycle;
using Motorent.Contracts.Motorcycles.Requests;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Presentation.Motorcycles;

internal sealed class RegisterMotorcycle : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("motorcycles", (
                RegisterMotorcycleRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<RegisterMotorcycleCommand>(),
                    cancellationToken)
                .ToResponseAsync(response => Results.CreatedAtRoute(
                    routeName: "GetMotorcycle",
                    routeValues: new { idOrLicensePlate = response.Id },
                    value: response)))
            .RequireAuthorization()
            .WithName("RegisterMotorcycle")
            .WithTags("Motorcycles")
            .Produces<MotorcycleResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi();
    }
}