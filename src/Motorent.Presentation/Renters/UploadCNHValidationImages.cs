using Motorent.Application.Renters.UploadCNHValidationImages;

namespace Motorent.Presentation.Renters;

internal sealed class UploadCNHValidationImages : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("renters/cnh-validation-images", (
                IFormFile frontImage,
                IFormFile backImage,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    new UploadCNHValidationImagesCommand
                    {
                        FrontImage = new FormFileProxy(frontImage),
                        BackImage = new FormFileProxy(backImage)
                    },
                    cancellationToken)
                .ToResponseAsync(_ => Results.NoContent()))
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithName("UploadCNHValidationImages")
            .WithSummary("Envia as imagens da CNH para validação do locatário autenticado")
            .WithTags("Renters")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status503ServiceUnavailable);
            // .WithOpenApi(); https://github.com/dotnet/aspnetcore/issues/53831
    }
}