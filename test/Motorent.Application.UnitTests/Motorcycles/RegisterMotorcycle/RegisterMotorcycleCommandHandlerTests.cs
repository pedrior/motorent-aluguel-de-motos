using Motorent.Application.Common.Mappings;
using Motorent.Application.Motorcycles.Common.Mappings;
using Motorent.Application.Motorcycles.RegisterMotorcycle;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.TestUtils.Constants;

namespace Motorent.Application.UnitTests.Motorcycles.RegisterMotorcycle;

[TestSubject(typeof(RegisterMotorcycleCommandHandler))]
public sealed class RegisterMotorcycleCommandHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();
    private readonly ILicensePlateService licensePlateService = A.Fake<ILicensePlateService>();

    private readonly RegisterMotorcycleCommandHandler sut;

    private static readonly RegisterMotorcycleCommand Command = new()
    {
        Model = Constants.Motorcycle.Model,
        Brand = Constants.Motorcycle.Brand.Name,
        Year = Constants.Motorcycle.Year.Value,
        DailyPrice = Constants.Motorcycle.DailyPrice.Value,
        LicensePlate = Constants.Motorcycle.LicensePlate.Value
    };

    public static readonly IEnumerable<object[]> FailedCommands =
    [
        [Command with { Year = 0 }],
        [Command with { DailyPrice = -8m }],
        [Command with { LicensePlate = "ABC1234" }]
    ];

    public RegisterMotorcycleCommandHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new CommonMappings());
        TypeAdapterConfig.GlobalSettings.Apply(new MotorcycleMappings());

        sut = new RegisterMotorcycleCommandHandler(motorcycleRepository, licensePlateService);
    }

    [Fact]
    public async Task Handle_WhenCalledWithValidCommand_ShouldReturnMotorcycleResponse()
    {
        // Arrange
        var licensePlate = LicensePlate.Create(Command.LicensePlate).Value;
        A.CallTo(() => licensePlateService.IsUniqueAsync(licensePlate, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType<MotorcycleResponse>();
    }

    [Fact]
    public async Task Handle_WhenCalledWithValidCommand_ShouldAddMotorcycleToRepository()
    {
        // Arrange
        var licensePlate = LicensePlate.Create(Command.LicensePlate).Value;
        A.CallTo(() => licensePlateService.IsUniqueAsync(licensePlate, A<CancellationToken>._))
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => motorcycleRepository.AddAsync(A<Motorcycle>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory, MemberData(nameof(FailedCommands))]
    public async Task Handle_WhenCalledWithInvalidCommand_ShouldReturnFailure(RegisterMotorcycleCommand command)
    {
        // Arrange
        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleCreationFails_ShouldReturnFailure()
    {
        // Arrange
        A.CallTo(() => licensePlateService.IsUniqueAsync(A<LicensePlate>._, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
    }
}