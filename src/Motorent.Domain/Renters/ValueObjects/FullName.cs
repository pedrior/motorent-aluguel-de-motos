namespace Motorent.Domain.Renters.ValueObjects;

public sealed class FullName(string givenName, string familyName) : ValueObject
{
    public string GivenName { get; private init; } = givenName;
    
    public string FamilyName { get; private init; } = familyName;

    public override string ToString() => $"{GivenName} {FamilyName}";
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return GivenName;
        yield return FamilyName;
    }
}