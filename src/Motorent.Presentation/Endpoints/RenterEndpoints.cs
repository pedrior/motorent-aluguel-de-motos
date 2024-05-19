using Motorent.Application.Renters.GetRenterProfile;
using Motorent.Application.Renters.UpdateCNH;
using Motorent.Application.Renters.UpdatePersonalInfo;
using Motorent.Application.Renters.UploadCNHImage;
using Motorent.Contracts.Renters.Requests;
using Motorent.Contracts.Renters.Responses;

namespace Motorent.Presentation.Endpoints;

internal sealed class RenterEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("renters")
            .WithTags("Renters")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("", GetProfile)
            .WithName(nameof(GetProfile))
            .WithSummary("Obtém o perfil do locatário")
            .Produces<RenterProfileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("", UpdatePersonalInfo)
            .WithName(nameof(UpdatePersonalInfo))
            .WithSummary("Atualiza as informações pessoais do locatário")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("cnh", UpdateCNH)
            .WithName(nameof(UpdateCNH))
            .WithSummary("Atualiza a CNH do locatário")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPut("cnh-image", UploadCNHImage)
            .DisableAntiforgery()
            .WithName(nameof(UploadCNHImage))
            .WithSummary("Envia a imagem da CNH do locatário")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status503ServiceUnavailable);
    }

    private static Task<IResult> GetProfile(ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new GetRenterProfileQuery(), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }

    private static Task<IResult> UpdatePersonalInfo(
        UpdatePersonalInfoRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<UpdatePersonalInfoCommand>(), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }

    private static Task<IResult> UpdateCNH(
        UpdateCNHRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<UpdateCNHCommand>(), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }

    private static Task<IResult> UploadCNHImage(
        IFormFile image,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new UploadCNHImageCommand
            {
                Image = new FormFileProxy(image)
            }, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
}