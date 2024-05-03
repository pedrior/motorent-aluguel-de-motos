namespace Motorent.Contracts.Common.Responses;

public sealed record PageResponse<T> where T : class
{ 
    public required int Page { get; init; }
    
    public required int Limit { get; init; }
    
    public required int TotalItems { get; init; }
    
    public required int TotalPages { get; init; }
    
    public bool HasPreviousPage => Page > 1;
    
    public bool HasNextPage => Page < TotalPages;

    public required IEnumerable<T> Items { get; init; } = [];
    
    public static PageResponse<T> Empty(int page, int limit) => new()
    {
        Page = page,
        Limit = limit,
        TotalItems = 0,
        TotalPages = 0,
        Items = []
    };
}