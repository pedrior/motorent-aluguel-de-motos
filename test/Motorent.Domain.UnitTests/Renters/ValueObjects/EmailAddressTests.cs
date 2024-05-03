using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters.ValueObjects;

[TestSubject(typeof(EmailAddress))]
public sealed class EmailAddressTests
{
    public static readonly IEnumerable<object[]> ValidEmails = new List<object[]>
    {
        new object[] { "john@doe.com" },
        new object[] { "john.DOE@domain.com" },
        new object[] { "John_@domain.fr" },
        new object[] { "john12@doe.com" },
        new object[] { "john.d12@domain.ch" }
    };

    public static readonly IEnumerable<object[]> InvalidEmails = new List<object[]>
    {
        new object[] { "john@doe" },
        new object[] { "john.DOEdomain.com" },
        new object[] { "John @domain.fr" },
        new object[] { "john12@doe..com" },
        new object[] { "john.d12@ch" }
    };
    
    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void Create_WhenEmailIsValid_ShouldReturnEmail(string email)
    {
        // Arrange
        // Act
        var result = EmailAddress.Create(email);
        
        // Assert
        result.Should().BeSuccess();
        result.Value.Value.Should().Be(email.ToLowerInvariant());
    }
    
    [Theory]
    [MemberData(nameof(InvalidEmails))]
    public void Create_WhenEmailIsInvalid_ShouldReturnInvalid(string email)
    {
        // Arrange
        // Act
        var result = EmailAddress.Create(email);
        
        // Assert
        result.Should().BeFailure(EmailAddress.Invalid);
    }
}