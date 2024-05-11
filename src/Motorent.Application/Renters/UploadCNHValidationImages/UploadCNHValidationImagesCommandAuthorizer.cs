using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UploadCNHValidationImages;

internal sealed class UploadCNHValidationImagesCommandAuthorizer 
    : IAuthorizer<UploadCNHValidationImagesCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UploadCNHValidationImagesCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}