using Motorent.Application.Renters.UpdateCNH;
using Motorent.Contracts.Renters.Requests;

namespace Motorent.Presentation.Renters;

internal sealed class UpdateCNH : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("renters/cnh", (
                UpdateCNHRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<UpdateCNHCommand>(),
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .RequireAuthorization()
            .WithName("UpdateCNH")
            .WithTags("Renters")
            .WithSummary("Updates the renter's CNH")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi();
    }
}