using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Renters.ValueObjects;

public sealed class CNHValidationImages(Uri frontImageUrl, Uri backImageUrl) : ValueObject
{
    public Uri FrontImageUrl { get; private init; } = frontImageUrl;
    
    public Uri BackImageUrl { get; private init; } = backImageUrl;

    public override string ToString() => $"Front: {FrontImageUrl}, Back: {BackImageUrl}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FrontImageUrl;
        yield return BackImageUrl;
    }
}