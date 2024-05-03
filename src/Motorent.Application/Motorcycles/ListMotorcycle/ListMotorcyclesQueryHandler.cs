using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles.Repository;

namespace Motorent.Application.Motorcycles.ListMotorcycle;

internal sealed class ListMotorcyclesQueryHandler(IMotorcycleRepository motorcycleRepository)
    : IQueryHandler<ListMotorcyclesQuery, PageResponse<MotorcycleSummaryResponse>>
{
    public async Task<Result<PageResponse<MotorcycleSummaryResponse>>> Handle(ListMotorcyclesQuery query,
        CancellationToken cancellationToken)
    {
        var motorcycles = await motorcycleRepository.ListAsync(
            query.Page,
            query.Limit,
            query.Sort,
            query.Order,
            query.Search, 
            cancellationToken);

        if (motorcycles.Count is 0)
        {
            return PageResponse<MotorcycleSummaryResponse>.Empty(query.Page, query.Limit);
        }

        var totalMotorcycles = await motorcycleRepository.CountAsync(query.Search, cancellationToken);
        var response = new PageResponse<MotorcycleSummaryResponse>
        {
            Page = query.Page,
            Limit = query.Limit,
            TotalItems = totalMotorcycles,
            TotalPages = (int)Math.Ceiling((double)totalMotorcycles / query.Limit),
            Items = motorcycles.Adapt<IEnumerable<MotorcycleSummaryResponse>>()
        };

        return response;
    }
}