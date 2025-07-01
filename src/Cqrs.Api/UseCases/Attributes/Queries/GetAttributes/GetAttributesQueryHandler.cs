using Cqrs.Api.Common.BaseRequests;
using Cqrs.Api.UseCases.Articles.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Responses;
using Cqrs.Api.UseCases.Attributes.Domain.Projections;
using ErrorOr;

namespace Cqrs.Api.UseCases.Attributes.Queries.GetAttributes;

/// <summary>
/// Handles the <see cref="BaseQuery"/> request.
/// </summary>
public class GetAttributesQueryHandler(
    Marten.IDocumentSession _session)
{
    private const string TRUE_STRING = "true";

    /// <summary>
    /// Handles the GET request for category specific attributes.
    /// </summary>
    /// <param name="query">The request.</param>
    /// <returns>A list of category specific attributes of the article in the category tree.</returns>
    public async Task<ErrorOr<List<GetAttributesResponse>>> GetAttributesAsync(BaseQuery query)
    {
        // 1 Load from Marten State - ES and DDD
        var projectionId = $"{query.ArticleNumber}-{query.RootCategoryId}";
        var projection = await _session.LoadAsync<ArticleAttributeProjection>(projectionId);
        if (projection is null)
        {
            return ArticleErrors.ProjectionNotFound(projectionId);
        }

        var articleDtos = projection.Articles;
        var responseDtos = new List<GetAttributesResponse>();

        GetAttributesResponse? attributeWithMostTrueValues = null;
        int mostTrueValues = 0;

        // Precompute true string comparison once
        const StringComparison comparison = StringComparison.OrdinalIgnoreCase;

        foreach (var attribute in projection.Attributes)
        {
            #pragma warning disable S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension
            var trueCount = attribute.Values.Count(x => x.Values.Any(v => string.Equals(v, TRUE_STRING, comparison)));
            #pragma warning restore S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension

            var responseDto = new GetAttributesResponse(
                AttributeId: attribute.AttributeId,
                AttributeName: attribute.AttributeName ?? string.Empty,
                Type: attribute.Type ?? string.Empty,
                MaxValues: attribute.MaxValues)
            {
                Values = attribute.Values,
                MinValues = attribute.MinValues,
                SubAttributes = attribute.SubAttributes
            };

            responseDtos.Add(responseDto);

            if (trueCount > mostTrueValues)
            {
                mostTrueValues = trueCount;
                attributeWithMostTrueValues = responseDto;
            }
        }

        // Prepare true/empty VariantAttributeValues for reuse
        var trueAttributeValues = articleDtos
            .Select(a => new VariantAttributeValues(a.CharacteristicId, [TRUE_STRING]))
            .ToList();

        var emptyAttributeValues = articleDtos
            .Select(a => new VariantAttributeValues(a.CharacteristicId, []))
            .ToList();

        // Assign values to responses
        foreach (var response in responseDtos)
        {
            if (response == attributeWithMostTrueValues && mostTrueValues > 0)
            {
                response.Values = trueAttributeValues;
            }
            else
            {
                response.Values = emptyAttributeValues;
            }
        }

        return responseDtos;
    }
}
