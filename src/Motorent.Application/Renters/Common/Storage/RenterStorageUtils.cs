using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.Common.Storage;

internal static class RenterStorageUtils
{
    public static Uri GetCNHImagePath(RenterId renterId, string extension) => 
        new($@"renters\{renterId}\cnh{extension}", UriKind.Relative);
}