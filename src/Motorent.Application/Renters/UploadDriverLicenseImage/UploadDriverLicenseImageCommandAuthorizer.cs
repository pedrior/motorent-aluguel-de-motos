using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UploadDriverLicenseImage;

internal sealed class UploadDriverLicenseImageCommandAuthorizer 
    : IAuthorizer<UploadDriverLicenseImageCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UploadDriverLicenseImageCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}