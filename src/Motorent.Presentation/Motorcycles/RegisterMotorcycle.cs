using Motorent.Application.Motorcycles.RegisterMotorcycle;
using Motorent.Contracts.Motorcycles.Requests;

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
                    routeName: "get-motorcycle",
                    routeValues: new { idOrLicensePlate = response.Id },
                    value: response)))
            .RequireAuthorization();
    }
}