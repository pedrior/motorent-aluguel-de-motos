using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Motorcycles.UpdateDailyPrice;

internal sealed class UpdateDailyPriceCommandAuthorizer : IAuthorizer<UpdateDailyPriceCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdateDailyPriceCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Admin);
    }
}