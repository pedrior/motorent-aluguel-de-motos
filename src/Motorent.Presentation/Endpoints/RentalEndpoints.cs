using Motorent.Application.Rentals.Rent;
using Motorent.Application.Rentals.UpdateReturnDate;
using Motorent.Contracts.Rentals.Requests;
using Motorent.Contracts.Rentals.Responses;

namespace Motorent.Presentation.Endpoints;

internal sealed class RentalEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rentals")
            .WithTags("Rentals")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapPost("", Rent)
            .WithName(nameof(Rent))
            .WithSummary("Cria um aluguel")
            .Produces<RentalResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

        group.MapPut("{id}/return-date", UpdateReturnDate)
            .WithName(nameof(UpdateReturnDate))
            .WithSummary("Atualiza a data de devolução do aluguel")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity);
    }

    private static Task<IResult> Rent(RentRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<RentCommand>(), cancellationToken)
            .ToResponseAsync(response => Results.Created(uri: null as Uri, value: response));
    }

    private static Task<IResult> UpdateReturnDate(
        Ulid id,
        UpdateReturnDateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<UpdateReturnDateCommand>() with
            {
                RentalId = id
            }, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
}