namespace Motorent.Application.Rentals.ListRentals;

internal sealed class ListRentalsQueryValidator : AbstractValidator<ListRentalsQuery>
{
    public ListRentalsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Must be greater than or equal to 1");

        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Must be greater than or equal to 1")
            .LessThanOrEqualTo(30)
            .WithMessage("Must be less than or equal to 30");
    }
}