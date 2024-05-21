using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Rentals.Persistence;

internal sealed class RentalRepository(DataContext context) : Repository<Rental, RentalId>(context), IRentalRepository;