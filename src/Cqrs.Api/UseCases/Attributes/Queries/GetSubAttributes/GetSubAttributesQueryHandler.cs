using Cqrs.Api.Common.DataAccess.Repositories;
using Cqrs.Api.UseCases.Articles.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities;
using Cqrs.Api.UseCases.Attributes.Common.Responses;
using Cqrs.Api.UseCases.Attributes.Common.Services;
using Cqrs.Api.UseCases.Attributes.Domain.Projections;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Cqrs.Api.UseCases.Attributes.Queries.GetSubAttributes;

/// <summary>
/// Handles the attribute requests.
/// </summary>
public class GetSubAttributesQueryHandler(
    Marten.IDocumentSession _session,
    ICachedReadRepository<AttributeMapping> _attributeMappingReadRepository,
    AttributeReadService _attributeReadService)
{
    /// <summary>
    /// Handles the GET request for category specific subAttributes.
    /// </summary>
    /// <param name="query">The request.</param>
    /// <returns>A list of the configurations and the ids of the sub-attributes of all passed attributes and their values for the requested article in the requested category tree.</returns>
    public async Task<ErrorOr<List<GetAttributesResponse>>> GetSubAttributesAsync(GetSubAttributesQuery query)
    {
        // 1. Load the projection from Marten
        var projectionId = $"{query.ArticleNumber}-{query.RootCategoryId}";
        var projection = await _session.LoadAsync<ArticleAttributeProjection>(projectionId);
        if (projection is null)
        {
            return ArticleErrors.ProjectionNotFound(projectionId);
        }

        var articleDtos = projection.Articles;

        // 2. Parse the attribute ids from the request and get the attributes
        var attributeIds = query.AttributeIds.Split(",").Select(int.Parse).ToList();

        var attributeDtos = await _attributeReadService.GetAttributesAndSubAttributesWithValuesAsync(
                articleDtos.ConvertAll(articleDto => articleDto.ArticleId),
                query.RootCategoryId,
                attributeIds);

        // 3. Get the unknown attribute ids and return an error if there are any
        var unknownAttributeIds = attributeIds
            .Except(attributeDtos.Select(tuple => tuple.Attribute.Id))
            .ToList();

        if (unknownAttributeIds.Count is not 0)
        {
            return AttributeErrors.AttributeIdsNotFound(unknownAttributeIds, query.RootCategoryId);
        }

        // 4. Convert the attributes to responses and return them
        List<GetAttributesResponse> responseDtos = new(attributeIds.Count);
        var attributeMappings = await _attributeMappingReadRepository.GetAllAsync();

        foreach (var tuple in attributeIds.Select(attributeId => attributeDtos.First(tuple => tuple.Attribute.Id == attributeId)))
        {
            var responseDto = AttributeConverter.ConvertAttributeToResponse(
                articleDtos.Exists(x => x.ArticleId == Convert.ToInt32(query.ArticleNumber, System.Globalization.CultureInfo.InvariantCulture) && x.CharacteristicId > 0),
                tuple.Attribute,
                tuple.AttributeValueDtos,
                articleDtos,
                attributeMappings);

            responseDtos.Add(responseDto);
        }

        return responseDtos;
    }
}
