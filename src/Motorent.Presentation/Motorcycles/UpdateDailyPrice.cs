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
            .RequireAuthorization();
    }
}