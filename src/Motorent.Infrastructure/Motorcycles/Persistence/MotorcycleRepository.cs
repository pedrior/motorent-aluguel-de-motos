using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Motorcycles.Persistence;

internal sealed class MotorcycleRepository(DataContext context) 
    : Repository<Motorcycle, MotorcycleId>(context), IMotorcycleRepository;