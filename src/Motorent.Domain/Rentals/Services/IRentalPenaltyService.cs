using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.Rentals.Services;

public interface IRentalPenaltyService
{
    Money Calculate(DateOnly currentReturnDate, DateOnly newReturnDate);
}