using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UploadCNHImage;

internal sealed class UploadCNHImageCommandAuthorizer 
    : IAuthorizer<UploadCNHImageCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UploadCNHImageCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}