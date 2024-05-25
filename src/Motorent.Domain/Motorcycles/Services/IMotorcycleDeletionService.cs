using Motorent.Domain.Rentals;

namespace Motorent.Domain.Motorcycles.Services;

public interface IMotorcycleDeletionService
{
    Result<Success> Delete(Motorcycle motorcycle, IEnumerable<Rental> rentals);
}