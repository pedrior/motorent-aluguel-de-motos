using Motorent.Application.Common.Mappings;
using Motorent.Application.Motorcycles.Common.Mappings;
using Motorent.Application.Motorcycles.ListMotorcycle;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Motorcycles.ListMotorcycle;

[TestSubject(typeof(ListMotorcyclesQueryHandler))]
public sealed class ListMotorcyclesQueryHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();

    private readonly ListMotorcyclesQueryHandler sut;

    private readonly ListMotorcyclesQuery query = new()
    {
        Page = 1,
        Limit = 15,
        Sort = null,
        Order = null,
        Search = null
    };

    public ListMotorcyclesQueryHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new CommonMappings());
        TypeAdapterConfig.GlobalSettings.Apply(new MotorcycleMappings());
        
        sut = new ListMotorcyclesQueryHandler(motorcycleRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnMotorcycleResponsePage()
    {
        // Arrange
        Motorcycle[] motorcycles =
        [
            (await Factories.Motorcycle.CreateAsync(id: MotorcycleId.New())).Value,
            (await Factories.Motorcycle.CreateAsync(id: MotorcycleId.New())).Value,
            (await Factories.Motorcycle.CreateAsync(id: MotorcycleId.New())).Value,
            (await Factories.Motorcycle.CreateAsync(id: MotorcycleId.New())).Value
        ];

        A.CallTo(() => motorcycleRepository.ListAsync(
                query.Page,
                query.Limit,
                query.Sort,
                query.Order,
                query.Search,
                A<CancellationToken>.Ignored))
            .Returns(motorcycles);
        
        A.CallTo(() => motorcycleRepository.CountAsync(query.Search, A<CancellationToken>.Ignored))
            .Returns(motorcycles.Length);
        
        // Act
        var result = await sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().BeSuccess();
        var response = result.Value;
        
        response.Page.Should().Be(query.Page);
        response.Limit.Should().Be(query.Limit);
        response.TotalItems.Should().Be(motorcycles.Length);
        response.TotalPages.Should().Be(1);
        response.Items.Should().HaveCount(motorcycles.Length);
    }
    
    [Fact]
    public async Task Handle_WhenNoMotorcyclesFound_ShouldReturnEmptyPage()
    {
        // Arrange
        A.CallTo(() => motorcycleRepository.ListAsync(
                query.Page,
                query.Limit,
                query.Sort,
                query.Order,
                query.Search,
                A<CancellationToken>.Ignored))
            .Returns([]);
        
        // Act
        var result = await sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeEquivalentTo(PageResponse<MotorcycleSummaryResponse>.Empty(query.Page, query.Limit));
        
        A.CallTo(() => motorcycleRepository.CountAsync(query.Search, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }
}