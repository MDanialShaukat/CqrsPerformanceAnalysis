using Cqrs.Api.Common.BaseRequests;
using Cqrs.Api.UseCases.Articles.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Responses;
using Cqrs.Api.UseCases.Attributes.Domain.Projections;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Cqrs.Api.UseCases.Attributes.Queries.GetAttributes;

/// <summary>
/// Handles the <see cref="BaseQuery"/> request.
/// </summary>
public class GetAttributesQueryHandler(
    Marten.IDocumentSession _session,
    Serilog.ILogger _logger)
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
        _logger.Information("Loading projection for article number {ArticleNumber} and root category id {RootCategoryId}", query.ArticleNumber, query.RootCategoryId);
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

        foreach (var attribute in projection.Attributes)
        {
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

            var trueCount = attribute.Values.Count(x => x.Values.Contains(TRUE_STRING, StringComparer.OrdinalIgnoreCase));
            if (trueCount > mostTrueValues)
            {
                attributeWithMostTrueValues = responseDto;
                mostTrueValues = trueCount;
            }
        }

        if (attributeWithMostTrueValues is not null)
        {
            var hasTrueValues = mostTrueValues > 0;
            attributeWithMostTrueValues.Values = articleDtos
                .Select(a => new VariantAttributeValues(a.CharacteristicId, hasTrueValues ? [TRUE_STRING] : []))
                .ToList();
        }

        foreach (var response in responseDtos.Where(r => r != attributeWithMostTrueValues))
        {
            response.Values = articleDtos.Select(a => new VariantAttributeValues(a.CharacteristicId, [])).ToList();
        }

        return responseDtos;
    }
}
