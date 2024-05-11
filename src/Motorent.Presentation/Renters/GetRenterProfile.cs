using Motorent.Application.Renters.GetRenterProfile;
using Motorent.Contracts.Renters.Responses;

namespace Motorent.Presentation.Renters;

internal sealed class GetRenterProfile : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("renters", (
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    new GetRenterProfileQuery(), cancellationToken)
                .ToResponseAsync(Results.Ok))
            .RequireAuthorization()
            .WithName("GetRenterProfile")
            .WithTags("Renters")
            .WithSummary("Gets the renter's profile")
            .Produces<RenterProfileResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();
    }
}