using Motorent.Application.Renters.UpdatePersonalInfo;
using Motorent.Contracts.Renters.Requests;

namespace Motorent.Presentation.Renters;

internal sealed class UpdatePersonalInfo : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("renters/personal-info", (
                UpdatePersonalInfoRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<UpdatePersonalInfoCommand>(),
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .RequireAuthorization()
            .WithName("UpdatePersonalInfo")
            .WithTags("Renters")
            .WithSummary("Updates the renter's personal information")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();
    }
}