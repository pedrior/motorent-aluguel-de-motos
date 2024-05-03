using System.Text.RegularExpressions;
using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Renters.ValueObjects;

public sealed partial class EmailAddress : ValueObject
{
    public static readonly Error Invalid = Error.Validation("The email address is invalid.", code: "email");

    private EmailAddress()
    {
    }

    public string Value { get; private init; } = null!;

    public static Result<EmailAddress> Create(string value)
    {
        value = value.ToLowerInvariant();
        return EmailRegex().IsMatch(value)
            ? new EmailAddress { Value = value }
            : Invalid;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c" +
                    "\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z" +
                    "0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|" +
                    "[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-" +
                    "\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])",
        RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}