using System.Globalization;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Traditional.Api.Common.DataAccess.Persistence;
using Traditional.Api.Common.DataAccess.Repositories;
using Traditional.Api.UseCases.Attributes.Common.Errors;
using Traditional.Api.UseCases.Attributes.Common.Persistence.Entities;
using Traditional.Api.UseCases.Attributes.Common.Responses;
using Traditional.Api.UseCases.Attributes.Common.Services;

namespace Traditional.Api.UseCases.Attributes.GetLeafAttributes;

/// <summary>
/// Handles the attribute requests.
/// </summary>
public class GetLeafAttributesHandler(
    TraditionalDbContext _dbContext,
    AttributeService _attributeService,
    ICachedRepository<AttributeMapping> _attributeMappingReadRepository)
{
    /// <summary>
    /// Handles the GET request for category specific leafAttributes.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>A list of category specific leaf attributes of the article in the category tree.</returns>
    public async Task<ErrorOr<List<GetAttributesResponse>>> GetLeafAttributesAsync(GetLeafAttributesRequest request)
    {
        // 1. Fetch the article DTOs
        var dtoOrError = await _attributeService.GetArticleDtosAndMappedCategoryIdAsync(request);

        if (dtoOrError.IsError)
        {
            return dtoOrError.Errors;
        }

        var (articleDtos, _) = dtoOrError.Value;

        // 2. Parse the attribute id from the request and get the attribute
        var attributeId = int.Parse(request.AttributeId, CultureInfo.InvariantCulture);

        var attributeDtos = await _attributeService.GetAttributesAndSubAttributesWithValuesAsync(
                articleDtos.ConvertAll(a => a.ArticleId),
                request.RootCategoryId,
                [attributeId]);

        // 4. Get the requested attribute and return an error if it is unknown
        var attribute = attributeDtos.Select(tuple => tuple.Attribute).FirstOrDefault(x => x.Id == attributeId);

        if (attribute is null)
        {
            return AttributeErrors.AttributeIdsNotFound([attributeId], request.RootCategoryId);
        }

        // 4. Convert the attribute to a response and return it
        return AttributeConverter.ConvertAllLeafAttributes(
            await _dbContext.Articles.AnyAsync(article => article.ArticleNumber == request.ArticleNumber && article.CharacteristicId > 0),
            attribute,
            attributeDtos,
            articleDtos,
            await _attributeMappingReadRepository.GetAllAsync());
    }
}
