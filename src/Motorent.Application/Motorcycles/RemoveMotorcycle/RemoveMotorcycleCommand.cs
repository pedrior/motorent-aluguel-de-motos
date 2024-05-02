using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Motorcycles.RemoveMotorcycle;

public sealed record RemoveMotorcycleCommand(Ulid Id) : ICommand, ITransactional;