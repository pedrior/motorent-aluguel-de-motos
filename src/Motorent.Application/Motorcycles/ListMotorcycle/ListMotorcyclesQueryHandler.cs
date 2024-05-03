using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles.Repository;

namespace Motorent.Application.Motorcycles.ListMotorcycle;

internal sealed class ListMotorcyclesQueryHandler(IMotorcycleRepository motorcycleRepository)
    : IQueryHandler<ListMotorcyclesQuery, PageResponse<MotorcycleResponse>>
{
    public async Task<Result<PageResponse<MotorcycleResponse>>> Handle(ListMotorcyclesQuery query,
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
            return PageResponse<MotorcycleResponse>.Empty(query.Page, query.Limit);
        }

        var totalMotorcycles = await motorcycleRepository.CountAsync(query.Search, cancellationToken);
        var response = new PageResponse<MotorcycleResponse>
        {
            Page = query.Page,
            Limit = query.Limit,
            TotalItems = totalMotorcycles,
            TotalPages = (int)Math.Ceiling((double)totalMotorcycles / query.Limit),
            Items = motorcycles.Adapt<IEnumerable<MotorcycleResponse>>()
        };

        return response;
    }
}