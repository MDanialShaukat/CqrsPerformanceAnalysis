using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCommon.Constants;
using Traditional.Api.UseCases.RootCategories.Common.Persistence.Entities;
using Traditional.Api.UseCases.RootCategories.GetRootCategories;
using Traditional.Tests.TestCommon.BaseTest;

namespace Traditional.Tests.UseCases.RootCategories;

public class GetRootCategoriesEndpointTests(TraditionalApiFactory factory)
    : BaseTestWithSharedTraditionalApiFactory(factory)
{
    [Fact]
    public async Task GetRootCategories_WhenDbIsSeeded_ShouldReturnRootCategories()
    {
        // Arrange
        await using var dbContext = ResolveTraditionalDbContext();

        var expectedRootCategories = (await dbContext.RootCategories.ToListAsync())
            .Select(rootCategory => new GetRootCategoryResponse(
                rootCategory.Id,
                rootCategory.LocaleCode,
                rootCategory.LocaleCode == LocaleCode.de_DE));

        // Act
        var response = await HttpClient.GetAsync(EndpointRoutes.RootCategories.GET_ROOT_CATEGORIES);

        // Assert
        response.EnsureSuccessStatusCode();
        var rootCategories = await response.Content.ReadFromJsonAsync<List<GetRootCategoryResponse>>();

        rootCategories.Should().BeEquivalentTo(expectedRootCategories);
    }

    [Fact]
    public async Task GetRootCategories_WhenDbIsNotSeeded_ShouldReturnProblemDetailsWithInternalServerError()
    {
        // Arrange
        await using var dbContext = ResolveTraditionalDbContext();
        await dbContext.RootCategories.ExecuteDeleteAsync();

        // Act
        var response = await HttpClient.GetAsync(EndpointRoutes.RootCategories.GET_ROOT_CATEGORIES);

        // Assert
        await ValidateResponse(response, HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
    }

    private static async Task ValidateResponse(
        HttpResponseMessage response,
        HttpStatusCode expectedStatusCode,
        string expectedTitle)
    {
        response.StatusCode.Should().Be(expectedStatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)expectedStatusCode);
        problemDetails.Title.Should().Be(expectedTitle);

        // Currently not used in the application
        problemDetails.Detail.Should().Be("If the problem persists, please contact the maintainer of this service and provide the traceId.");
        problemDetails.Instance.Should().BeNullOrWhiteSpace();
    }
}
