namespace Motorent.Application.Motorcycles.Common.Errors;

internal static class MotorcycleErrors
{
    public static readonly Error NotFound = Error.NotFound("Moto não encontrada.", code: "motorcycle.not_found");
}