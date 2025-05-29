using System.Globalization;
using Cqrs.Api.Common.DataAccess.Repositories;
using Cqrs.Api.UseCases.Articles.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities;
using Cqrs.Api.UseCases.Attributes.Common.Responses;
using Cqrs.Api.UseCases.Attributes.Common.Services;
using Cqrs.Api.UseCases.Attributes.Domain.Projections;
using ErrorOr;

namespace Cqrs.Api.UseCases.Attributes.Queries.GetLeafAttributes;

/// <summary>
/// Handles the attribute requests.
/// </summary>
public class GetLeafAttributesQueryHandler(
    Marten.IDocumentSession _session,
    ICachedReadRepository<AttributeMapping> _attributeMappingReadRepository,
    AttributeReadService _attributeReadService)
{
    /// <summary>
    /// Handles the GET request for category specific leafAttributes.
    /// </summary>
    /// <param name="query">The request.</param>
    /// <returns>A list of category specific leaf attributes of the article in the category tree.</returns>
    public async Task<ErrorOr<List<GetAttributesResponse>>> GetLeafAttributesAsync(GetLeafAttributesQuery query)
    {
        // 1. Load the projection from Marten
        var projectionId = $"{query.ArticleNumber}-{query.RootCategoryId}";
        var projection = await _session.LoadAsync<ArticleAttributeProjection>(projectionId);
        if (projection is null)
        {
            return ArticleErrors.ProjectionNotFound(projectionId);
        }

        var articleDtos = projection.Articles;

        // 2. Parse the attribute id from the request and get the attribute
        var attributeId = int.Parse(query.AttributeId, CultureInfo.InvariantCulture);

        // 3. Get attributes and sub-attributes with values using the projection's article ids
        var attributeDtos = await _attributeReadService.GetAttributesAndSubAttributesWithValuesAsync(
            articleDtos.ConvertAll(a => a.ArticleId),
            query.RootCategoryId,
            [attributeId]);

        // 4. Get the requested attribute and return an error if it is unknown
        var attribute = attributeDtos.Select(tuple => tuple.Attribute).FirstOrDefault(x => x.Id == attributeId);

        if (attribute is null)
        {
            return AttributeErrors.AttributeIdsNotFound([attributeId], query.RootCategoryId);
        }

        // 5. Use the projection to determine if any article has a characteristic id > 0
        var hasCharacteristic = articleDtos.Exists(article => article.CharacteristicId > 0);

        // 6. Get all attribute mappings from Marten (if they are stored in Marten, otherwise fallback to repository)
        // If AttributeMapping is also projected in Marten, use _session.Query<AttributeMapping>().ToListAsync()
        var attributeMappings = await _attributeMappingReadRepository.GetAllAsync();

        // 7. Convert the attribute to a response and return it
        return AttributeConverter.ConvertAllLeafAttributes(
            hasCharacteristic,
            attribute,
            attributeDtos,
            articleDtos,
            attributeMappings);
    }
}
