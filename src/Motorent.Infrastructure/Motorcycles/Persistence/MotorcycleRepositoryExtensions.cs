using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Motorcycles;

namespace Motorent.Infrastructure.Motorcycles.Persistence;

internal static class MotorcycleRepositoryExtensions
{
    public static IQueryable<Motorcycle> ApplySearchFilter(this IQueryable<Motorcycle> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        return query.Where(m => EF.Functions.ILike(m.Model, $"%{search}%")
                                || EF.Functions.ILike((string)(object)m.Brand, $"%{search}%")
                                || EF.Functions.ILike((string)(object)m.LicensePlate, $"%{search}%"));
    }

    public static IQueryable<Motorcycle> ApplyOrder(this IQueryable<Motorcycle> query, string? sort, string? order)
    {
        var asc = sort?.Equals("asc", StringComparison.InvariantCultureIgnoreCase) ?? true;
        return order?.ToLowerInvariant() switch
        {
            "brand" => asc ? query.OrderBy(m => m.Brand) : query.OrderByDescending(m => m.Brand),
            "license_plate" => asc ? query.OrderBy(m => m.LicensePlate) : query.OrderByDescending(m => m.LicensePlate),
            _ => asc ? query.OrderBy(m => m.Model) : query.OrderByDescending(m => m.Model)
        };
    }
}