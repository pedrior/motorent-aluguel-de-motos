using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters.ValueObjects;

[TestSubject(typeof(Document))]
public sealed class DocumentTests
{
    public static readonly IEnumerable<object[]> ValidDocuments = new List<object[]>
    {
        new object[] { "27137033000134" },
        new object[] { "97877245000133" },
        new object[] { "96.657.097/0001-89" },
        new object[] { "72.302.462/0001-74" }
    };

    public static readonly IEnumerable<object[]> InvalidDocuments = new List<object[]>
    {
        new object[] { "2713703300013" },
        new object[] { "96.657.09780001-89" },
        new object[] { "74798886000161" }
    };

    [Theory, MemberData(nameof(ValidDocuments))]
    public void Create_WhenDocumentIsValid_ShouldReturnDocument(string value)
    {
        // Arrange
        // Act
        var result = Document.Create(value);

        // Assert
        result.Should().BeSuccess(value);
    }
    
    [Theory, MemberData(nameof(InvalidDocuments))]
    public void Create_WhenDocumentIsInvalid_ShouldReturnInvalid(string value)
    {
        // Arrange
        // Act
        var result = Document.Create(value);

        // Assert
        result.Should().BeFailure(Document.Invalid);
    }
}