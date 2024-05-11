using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.Common.Storage;

internal static class RenterStorageUtils
{
    public static Uri GetFrontCNHImageUrl(RenterId renterId, string extension) => 
        new($@"renters\{renterId}\cnh\front{extension}", UriKind.Relative);
    
    public static Uri GetBackCNHImageUrl(RenterId renterId, string extension) =>
        new($@"renters\{renterId}\cnh\back{extension}", UriKind.Relative);
}