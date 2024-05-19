using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.Common.Storage;

internal static class RenterStorageUtils
{
    public static Uri GetDriverLicenseImagePath(RenterId renterId, string extension) => 
        new($@"renters\{renterId}\driver-license{extension}", UriKind.Relative);
}