using Motorent.Application.Motorcycles.GetMotorcycle;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Presentation.Motorcycles;

internal sealed class GetMotorcycle : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("motorcycles/{idOrLicensePlate}", (
                string idOrLicensePlate,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    new GetMotorcycleQuery(idOrLicensePlate),
                    cancellationToken)
                .ToResponseAsync(Results.Ok))
            .AllowAnonymous()
            .WithName("GetMotorcycle")
            .WithTags("Motorcycles")
            .WithSummary("Obtém uma moto cadastrada pelo ID ou placa")
            .Produces<MotorcycleResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }
}