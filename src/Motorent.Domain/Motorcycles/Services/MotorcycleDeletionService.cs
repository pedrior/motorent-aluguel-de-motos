using Motorent.Domain.Motorcycles.Errors;
using Motorent.Domain.Rentals;

namespace Motorent.Domain.Motorcycles.Services;

public sealed class MotorcycleDeletionService : IMotorcycleDeletionService
{
    public Result<Success> Delete(Motorcycle motorcycle, IEnumerable<Rental> rentals)
    {
        if (rentals.Any())
        {
            return MotorcycleErrors.CannotDeletedRentedMotorcycle;
        }
        
        motorcycle.Delete();
        return Success.Value;
    }
}