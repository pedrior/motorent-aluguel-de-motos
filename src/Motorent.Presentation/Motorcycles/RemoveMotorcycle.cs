using Motorent.Application.Motorcycles.RemoveMotorcycle;

namespace Motorent.Presentation.Motorcycles;

internal sealed class RemoveMotorcycle : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("motorcycles/{id}", (
                Ulid id,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    new RemoveMotorcycleCommand(id),
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .RequireAuthorization()
            .WithName("RemoveMotorcycle")
            .WithTags("Motorcycles")
            .WithSummary("Removes a motorcycle by its ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }
}