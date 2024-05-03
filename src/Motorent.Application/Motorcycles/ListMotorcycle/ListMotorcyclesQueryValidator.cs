namespace Motorent.Application.Motorcycles.ListMotorcycle;

internal sealed class ListMotorcyclesQueryValidator : AbstractValidator<ListMotorcyclesQuery>
{
    private static readonly string[] SortOptions = ["asc", "desc"];
    private static readonly string[] OrderOptions = ["model", "brand", "license_plate"];

    public ListMotorcyclesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Must be greater than or equal to 1");

        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Must be greater than or equal to 1")
            .LessThanOrEqualTo(30)
            .WithMessage("Must be less than or equal to 30");

        RuleFor(x => x.Order)
            .Must(v => OrderOptions.Contains(v, StringComparer.OrdinalIgnoreCase))
            .Unless(x => string.IsNullOrWhiteSpace(x.Order))
            .WithMessage($"Must be one of the following: {string.Join(", ", OrderOptions)}");

        RuleFor(x => x.Sort)
            .Must(v => SortOptions.Contains(v, StringComparer.OrdinalIgnoreCase))
            .Unless(x => string.IsNullOrWhiteSpace(x.Sort))
            .WithMessage($"Must be one of the following: {string.Join(", ", SortOptions)}");

        RuleFor(x => x.Search)
            .MaximumLength(30)
            .Unless(x => string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("Must be less or equal to 30 characters");
    }
}