using Motorent.Application.Rentals.GetRental;
using Motorent.Application.Rentals.ListRentals;
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

        group.MapGet("", ListRentals)
            .WithName(nameof(ListRentals))
            .WithSummary("Lista os alugueis")
            .Produces<IEnumerable<RentalResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapGet("{id}", GetRental)
            .WithName("GetRental")
            .WithSummary("Obtém um aluguel pelo ID")
            .Produces<RentalResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

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
        return sender.Send(new RentCommand
            {
                Plan = request.Plan.Trim(),
                MotorcycleId = Ulid.TryParse(request.MotorcycleId, out var id) ? id : Ulid.Empty
            }, cancellationToken)
            .ToResponseAsync(response => Results.Created(uri: null as Uri, value: response));
    }

    private static Task<IResult> ListRentals(
        [AsParameters] ListRentalsRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.Adapt<ListRentalsQuery>(), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }

    private static Task<IResult> GetRental(Ulid id, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new GetRentalQuery { Id = id }, cancellationToken)
            .ToResponseAsync(Results.Ok);
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