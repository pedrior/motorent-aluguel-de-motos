using Motorent.Application.Motorcycles.Get;

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
            .WithName("get-motorcycle");
    }
}