using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Rentals.Common.Errors;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Rentals.UpdateReturnDate;

internal sealed class UpdateReturnDateCommandHandler(
    IRentalRepository rentalRepository,
    IRentalPenaltyService rentalPenaltyService) : ICommandHandler<UpdateReturnDateCommand>
{
    public async Task<Result<Success>> Handle(UpdateReturnDateCommand command, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.FindAsync(new RentalId(command.RentalId), cancellationToken);
        if (rental is null)
        {
            return RentalErrors.NotFound;
        }

        return await rental.ChangeReturnDate(command.ReturnDate, rentalPenaltyService)
            .ThenAsync(() => rentalRepository.UpdateAsync(rental, cancellationToken));
    }
}