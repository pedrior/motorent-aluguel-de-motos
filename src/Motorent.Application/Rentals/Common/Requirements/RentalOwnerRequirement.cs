using Motorent.Application.Common.Abstractions.Security;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Rentals.Common.Requirements;

internal sealed record RentalOwnerRequirement(RentalId RentalId) : IRequirement;