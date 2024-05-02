using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Motorcycles.Responses;

namespace Motorent.Application.Motorcycles.GetMotorcycle;

public sealed record GetMotorcycleQuery(string IdOrLicensePlate) : IQuery<MotorcycleResponse>;