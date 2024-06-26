using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters.ValueObjects;

[TestSubject(typeof(Document))]
public sealed class DocumentTests
{
    public static readonly IEnumerable<object[]> ValidDocuments = new List<object[]>
    {
        new object[] { "27137033000134" },
        new object[] { "97877245000133" },
        new object[] { "81018729000197" },
        new object[] { "71.782.006/0001-06" },
        new object[] { "19.384.101/0001-31" }
    };

    public static readonly IEnumerable<object[]> InvalidDocuments = new List<object[]>
    {
        new object[] { "1903929700012" },
        new object[] { "43947303000116" },
        new object[] { "76337515123143" },
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