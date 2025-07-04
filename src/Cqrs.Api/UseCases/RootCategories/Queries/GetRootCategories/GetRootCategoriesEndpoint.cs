using System.Net;
using Cqrs.Api.Common.Constants;
using Cqrs.Api.Common.DataAccess.Repositories;
using Cqrs.Api.Common.Endpoints;
using Cqrs.Api.UseCases.RootCategories.Common.Persistence.Entities;

namespace Cqrs.Api.UseCases.RootCategories.Queries.GetRootCategories;

/// <inheritdoc />
public class GetRootCategoriesEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("rootCategories", GetRootCategoriesAsync)
            .WithTags(EndpointTags.ROOT_CATEGORIES)
            .WithSummary("Returns a list of all valid root categories without their children.")
            .Produces<IEnumerable<GetRootCategoryResponse>>()
            .ProducesProblem((int)HttpStatusCode.InternalServerError)
            .WithOpenApi()
            .CacheOutput(builder => builder.Expire(TimeSpan.FromDays(1)));
    }

    private static async Task<IResult> GetRootCategoriesAsync(ICachedReadRepository<RootCategory> rootCategoryReadRepository)
    {
        // Like Greg Young said: "It is possible to use the application services even in the query side".
        // We need to do this because we want to use the cache and the business logic guard that the root categories are seeded.
        // No need to implement ES and DDD here as it is already Optimised for Performance and the root categories are not expected to change often.
        var rootCategories = await rootCategoryReadRepository.GetAllAsync();

        var response = rootCategories.Select(rootCategory =>
            new GetRootCategoryResponse(
                rootCategory.Id,
                rootCategory.LocaleCode,
                rootCategory.LocaleCode is LocaleCode.de_DE));

        return Results.Ok(response);
    }
}
