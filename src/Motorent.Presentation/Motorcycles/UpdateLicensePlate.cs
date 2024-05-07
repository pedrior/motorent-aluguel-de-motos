using Motorent.Application.Motorcycles.UpdateLicensePlate;
using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Presentation.Motorcycles;

internal sealed class UpdateLicensePlate : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("motorcycles/{id}/license-plate", (
                Ulid id,
                UpdateLicensePlateRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<UpdateLicensePlateCommand>() with
                    {
                        Id = id
                    },
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .RequireAuthorization();
    }
}