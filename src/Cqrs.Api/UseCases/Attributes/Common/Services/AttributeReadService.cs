using System.Globalization;
using Cqrs.Api.Common.DataAccess.Persistence;
using Cqrs.Api.Common.Extensions;
using Cqrs.Api.UseCases.Attributes.Common.Models;
using Microsoft.EntityFrameworkCore;
using Attribute = Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities.Attribute;

namespace Cqrs.Api.UseCases.Attributes.Common.Services;

/// <summary>
/// Provides attribute related functionality.
/// </summary>
/// <param name="_dbContext">The cqrs read database context.</param>
public class AttributeReadService(CqrsReadDbContext _dbContext)
{
    /// <summary>
    /// Get the attributes and sub attributes with values for the given article ids.
    /// </summary>
    /// <param name="articleIds">The article ids to search for.</param>
    /// <param name="rootCategoryId">The root category id to search for.</param>
    /// <param name="productTypeIds">The product type ids to search for.</param>
    /// <returns>A list of tuples of the attribute and the attribute value DTOs.</returns>
    public async Task<List<(Attribute Attribute, List<AttributeValueDto> AttributeValueDtos)>> GetAttributesAndSubAttributesWithValuesAsync(
        List<int> articleIds,
        int rootCategoryId,
        List<int> productTypeIds)
    {
        var setProductTypeId = await _dbContext.AttributeBooleanValues
            .Where(value =>
                value.Value
                && articleIds.Contains(value.ArticleId)
                && value.Attribute!.ParentAttributeId == null
                && value.Attribute!.RootCategoryId == rootCategoryId)
            .Select(value => (int?)value.AttributeId)
            .FirstOrDefaultAsync();

        if (setProductTypeId is not null)
        {
            productTypeIds = [.. productTypeIds, setProductTypeId.Value];
        }

        productTypeIds = productTypeIds.Distinct().ToList();

        var attributes = await _dbContext.Attributes.RecursiveCteQuery(
            attribute => productTypeIds.Contains(attribute.Id),
            attribute => attribute.SubAttributes)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();

        List<int> attributeIds = attributes.ConvertAll(attribute => attribute.Id);

        var booleanValues = await _dbContext.AttributeBooleanValues
            .Where(value => articleIds.Contains(value.ArticleId) && attributeIds.Contains(value.AttributeId))
            .Select(value => new AttributeValueDto(value.AttributeId, value.ArticleId, value.Value.ToString()))
            .ToListAsync();

        var decimalValues = await _dbContext.AttributeDecimalValues
            .Where(value => articleIds.Contains(value.ArticleId) && attributeIds.Contains(value.AttributeId))
            .Select(value => new AttributeValueDto(value.AttributeId, value.ArticleId, value.Value.ToString(CultureInfo.InvariantCulture)))
            .ToListAsync();

        var intValues = await _dbContext.AttributeIntValues
            .Where(value => articleIds.Contains(value.ArticleId) && attributeIds.Contains(value.AttributeId))
            .Select(value => new AttributeValueDto(value.AttributeId, value.ArticleId, value.Value.ToString(CultureInfo.InvariantCulture)))
            .ToListAsync();

        var stringValues = await _dbContext.AttributeStringValues
            .Where(value => articleIds.Contains(value.ArticleId) && attributeIds.Contains(value.AttributeId))
            .Select(value => new AttributeValueDto(value.AttributeId, value.ArticleId, value.Value))
            .ToListAsync();

        var attributeValueDtos = booleanValues.Concat(decimalValues).Concat(intValues).Concat(stringValues);

        return attributes.ConvertAll(attribute =>
            (attribute, attributeValueDtos.Where(value => value.AttributeId == attribute.Id).ToList()));
    }
}
