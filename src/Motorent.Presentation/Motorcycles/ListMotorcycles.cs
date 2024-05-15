using Motorent.Application.Motorcycles.ListMotorcycle;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Requests;
using Motorent.Contracts.Motorcycles.Responses;

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
            .AllowAnonymous()
            .WithName("ListMotorcycle")
            .WithTags("Motorcycles")
            .WithSummary("Lista todas as motos cadastradas no sistema")
            .Produces<PageResponse<MotorcycleSummaryResponse>>()
            .WithOpenApi();
    }
}