namespace Motorent.Application.Motorcycles.Common.Errors;

internal static class MotorcycleErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Motorcycle not found.", code: "motorcycle.not_found");
}