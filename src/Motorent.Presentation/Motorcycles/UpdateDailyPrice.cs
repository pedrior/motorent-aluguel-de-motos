using Motorent.Application.Motorcycles.UpdateDailyPrice;
using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Presentation.Motorcycles;

internal sealed class UpdateDailyPrice : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("motorcycles/{id}/daily-price", (
                Ulid id,
                UpdateDailyPriceRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<UpdateDailyPriceCommand>() with
                    {
                        Id = id
                    },
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .RequireAuthorization()
            .WithName("UpdateDailyPrice")
            .WithTags("Motorcycles")
            .WithSummary("Atualiza o preço diário de uma moto cadastrada pelo ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }
}