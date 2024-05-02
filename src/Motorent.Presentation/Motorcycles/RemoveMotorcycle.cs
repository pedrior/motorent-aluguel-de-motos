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
            .RequireAuthorization();
    }
}