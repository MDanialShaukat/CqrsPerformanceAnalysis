using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Traditional.Api.Common.DataAccess.Persistence;
using Traditional.Api.Common.DataAccess.Repositories;
using Traditional.Api.Common.Exceptions;
using Traditional.Api.UseCases.RootCategories.Common.Persistence.Entities;
using Traditional.Tests.TestCommon.BaseTest;

namespace Traditional.Tests.Common.Persistence;

public class CachedRepositoryTests : BaseTestWithSharedTraditionalApiFactory
{
    private readonly TraditionalDbContext _dbContext;
    private readonly ICachedRepository<RootCategory> _repository;

    public CachedRepositoryTests(TraditionalApiFactory factory)
        : base(factory)
    {
        _dbContext = ResolveTraditionalDbContext();
        _repository = factory.Services.GetRequiredService<ICachedRepository<RootCategory>>();
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ShouldReturnAllItems()
    {
        // Arrange
        var expected = await _dbContext.RootCategories.ToListAsync();

        // Act
        var actual = await _repository.GetAllAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoItemsExist_ShouldThrowException()
    {
        // Arrange
        await _dbContext.RootCategories.ExecuteDeleteAsync();

        // Act
        Func<Task> action = async () => await _repository.GetAllAsync();

        // Assert
        await action.Should().ThrowAsync<SeededTableIsEmptyException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalled_ShouldReturnItem()
    {
        // Arrange
        var expected = await _dbContext.RootCategories.FirstAsync();

        // Act
        var actual = await _repository.GetByIdAsync(expected.Id);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var expected = await _dbContext.RootCategories.MaxAsync(e => e.Id) + 1;

        // Act
        var actual = await _repository.GetByIdAsync(expected);

        // Assert
        actual.Should().BeNull();
    }
}
