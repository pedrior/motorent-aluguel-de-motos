using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UpdateCNH;

internal sealed class UpdateCNHCommandValidator : AbstractValidator<UpdateCNHCommand>
{
    public UpdateCNHCommandValidator()
    {
        RuleFor(x => x.Number)
            .CNHNumber();

        RuleFor(x => x.Category)
            .CNHCategory();

        RuleFor(x => x.ExpDate)
            .CNHExpDate();
    }
}