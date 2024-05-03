using Motorent.Application.Motorcycles.ListMotorcycle;
using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Presentation.Motorcycles;

internal sealed class ListMotorcycles : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("motorcycles", (
                [AsParameters] ListMotorcyclesRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<ListMotorcyclesQuery>(),
                    cancellationToken)
                .ToResponseAsync(Results.Ok))
            .AllowAnonymous();
    }
}