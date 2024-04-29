using Motorent.Application.Common.Abstractions.Security;

namespace Motorent.Application.Common.Security;

internal sealed record RoleRequirement(string Role) : IRequirement;