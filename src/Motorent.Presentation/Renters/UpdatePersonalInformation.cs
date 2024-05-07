using Motorent.Application.Renters.UpdatePersonalInformation;
using Motorent.Contracts.Renters.Requests;

namespace Motorent.Presentation.Renters;

internal sealed class UpdatePersonalInformation : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("renters/personal-information", (
                UpdatePersonalInformationRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<UpdatePersonalInformationCommand>(),
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .RequireAuthorization();
    }
}