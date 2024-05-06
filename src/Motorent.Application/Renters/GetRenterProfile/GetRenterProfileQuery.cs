using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Renters.Responses;

namespace Motorent.Application.Renters.GetRenterProfile;

public sealed record GetRenterProfileQuery : IQuery<RenterProfileResponse>;