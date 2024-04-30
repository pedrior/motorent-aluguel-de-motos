using Motorent.Domain.Common.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles.Repository;

public interface IMotorcycleRepository : IRepository<Motorcycle, MotorcycleId>;