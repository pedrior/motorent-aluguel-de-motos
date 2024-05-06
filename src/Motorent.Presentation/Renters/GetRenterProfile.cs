using Motorent.Application.Renters.GetRenterProfile;

namespace Motorent.Presentation.Renters;

internal sealed class GetRenterProfile : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("renters/profile", (
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    new GetRenterProfileQuery(), cancellationToken)
                .ToResponseAsync(Results.Ok))
            .RequireAuthorization();
    }
}