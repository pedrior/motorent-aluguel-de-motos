using Motorent.Application.Motorcycles.GetMotorcycle;
using Motorent.Application.Motorcycles.ListMotorcycles;
using Motorent.Application.Motorcycles.RegisterMotorcycle;
using Motorent.Application.Motorcycles.RemoveMotorcycle;
using Motorent.Application.Motorcycles.UpdateLicensePlate;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Requests;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Presentation.Endpoints;

internal sealed class MotorcycleEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("motorcycles")
            .WithTags("Motorcycles")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("", ListMotorcycles)
            .AllowAnonymous()
            .WithName(nameof(ListMotorcycles))
            .WithSummary("Lista todas as motos cadastradas")
            .Produces<PageResponse<MotorcycleSummaryResponse>>();

        group.MapGet("{idOrLicensePlate}", GetMotorcycle)
            .AllowAnonymous()
            .WithName(nameof(GetMotorcycle))
            .WithSummary("Obtém uma moto pelo ID ou placa")
            .Produces<MotorcycleResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", RegisterMotorcycle)
            .WithName(nameof(RegisterMotorcycle))
            .WithSummary("Registra uma nova moto")
            .Produces<MotorcycleResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPut("{id}/license-plate", UpdateLicensePlate)
            .WithName(nameof(UpdateLicensePlate))
            .WithSummary("Atualiza a placa de uma moto pelo ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("{id}", DeleteMotorcycle)
            .WithName(nameof(DeleteMotorcycle))
            .WithSummary("Deleta uma moto pelo ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Task<IResult> ListMotorcycles(
        [AsParameters] ListMotorcyclesRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<ListMotorcyclesQuery>(), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }

    private static Task<IResult> GetMotorcycle(
        string idOrLicensePlate,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new GetMotorcycleQuery(idOrLicensePlate), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }

    private static Task<IResult> RegisterMotorcycle(
        RegisterMotorcycleRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<RegisterMotorcycleCommand>(), cancellationToken)
            .ToResponseAsync(response => Results.CreatedAtRoute(
                routeName: nameof(GetMotorcycle),
                routeValues: new
                {
                    idOrLicensePlate = response.Id
                },
                value: response));
    }

    private static Task<IResult> UpdateLicensePlate(
        Ulid id,
        UpdateLicensePlateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<UpdateLicensePlateCommand>() with
                {
                    Id = id
                },
                cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
    
    private static Task<IResult> DeleteMotorcycle(
        Ulid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new RemoveMotorcycleCommand(id), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
}