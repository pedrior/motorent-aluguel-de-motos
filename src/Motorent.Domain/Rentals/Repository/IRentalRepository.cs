using Motorent.Domain.Common.Repository;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.Rentals.Repository;

public interface IRentalRepository : IRepository<Rental, RentalId>;