using Motorent.Application.Renters.UploadCNHImage;

namespace Motorent.Presentation.Renters;

internal sealed class UploadCNHImage : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("renters/cnh-validation-images", (
                IFormFile image,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    new UploadCNHImageCommand
                    {
                        Image = new FormFileProxy(image)
                    },
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithName("UploadCNHImage")
            .WithSummary("Envia a imagem da CNH")
            .WithTags("Renters")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .WithOpenApi();
    }
}