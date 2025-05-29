using System.Diagnostics.CodeAnalysis;
using Cqrs.Api.Common.BaseRequests;
using Cqrs.Api.Common.DataAccess.Persistence;
using Cqrs.Api.Common.DataAccess.Persistence.Interfaces;
using Cqrs.Api.UseCases.Articles.Errors;
using Cqrs.Api.UseCases.Attributes.Common.Models;
using Cqrs.Api.UseCases.Attributes.Domain.Aggregates;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Attribute = Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities.Attribute;

namespace Cqrs.Api.UseCases.Attributes.Commands.UpdateAttributeValues;

/// <summary>
/// Handles the attribute update command with Domain-Driven Design and Event Sourcing.
/// </summary>
[SuppressMessage("Design", "MA0042:Do not use blocking calls in an async method", Justification = "The task is awaited by Task.WhenAll().")]
public class UpdateAttributeValuesCommandHandler(
    CqrsWriteDbContext _dbContext,
    IEventStore _eventStore)
{
    /// <summary>
    /// Handles the request to update the attribute values of an article.
    /// </summary>
    /// <param name="command">The request.</param>
    /// <returns>An <see cref="ErrorOr.Error"/> or a <see cref="Updated"/> result.</returns>
    public async Task<ErrorOr<Updated>> UpdateAttributeValuesAsync(
    UpdateAttributeValuesCommand command)
    {

        // 1. Create DDD Aggregate & Raise Domain Event
        var dtoOrError = await GetArticleDtosAndMappedCategoryIdAsync(command);

        if (dtoOrError.IsError)
        {
            return dtoOrError.Errors;
        }

        var (articleDtos, mappedCategoryID) = dtoOrError.Value;

        var receivedAttributeIds = command.NewAttributeValues.Select(value => value.AttributeId).ToList();
        var attributes = await GetAttributesWithSubAttributesByIdOrMpIdAndByRootCategoryId(
                receivedAttributeIds,
                command.RootCategoryId)
            .ToListAsync();

        var aggregate = new ArticleAttributeAggregate(command.ArticleNumber, command.RootCategoryId, mappedCategoryID);
        aggregate.UpdateAttributes(articleDtos, attributes, command.NewAttributeValues);

        foreach (var domainEvent in aggregate.Events)
        {
            var streamId = $"{command.ArticleNumber}-{command.RootCategoryId}";
            await _eventStore.AppendEventAsync(streamId, domainEvent);
        }

        aggregate.ClearEvents();
        return Result.Updated;
    }

    /// <summary>
    /// Get the article DTOs and the mapped category id for the requested article number.
    /// </summary>
    /// <param name="query">The request.</param>
    /// <returns>A <see cref="ErrorOr.Error"/> or a tuple of the article DTOs and the mapped category id.</returns>
    private async Task<ErrorOr<(List<ArticleDto>, int CategoryId)>> GetArticleDtosAndMappedCategoryIdAsync(BaseQuery query)
    {
        var articleDtos = await _dbContext.Articles
            .Where(a => a.ArticleNumber == query.ArticleNumber)
            .Select(article => new ArticleDto(article.Id, article.CharacteristicId))
            .ToListAsync();

        if (articleDtos.Count == 0)
        {
            return ArticleErrors.ArticleNotFound(query.ArticleNumber);
        }

        var mappedCategoryId = await _dbContext.Categories
            .Where(category => category.RootCategoryId == query.RootCategoryId &&
                               category.Articles!.Any(article => article.ArticleNumber == query.ArticleNumber))
            .Select(category => (int?)category.Id)
            .SingleOrDefaultAsync();

        if (mappedCategoryId is null)
        {
            return ArticleErrors.MappedCategoriesForArticleNotFound(query.ArticleNumber, query.RootCategoryId);
        }

        return (articleDtos, mappedCategoryId.Value);
    }

    /// <summary>
    /// Gets the attributes with sub-attributes by the given <paramref name="attributeIds"/> and <paramref name="rootCategoryId"/>.
    /// </summary>
    /// <param name="attributeIds">The attribute ids to get the attributes for.</param>
    /// <param name="rootCategoryId">The root category id to get the attributes for.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Attribute"/>s.</returns>
    private IAsyncEnumerable<Attribute> GetAttributesWithSubAttributesByIdOrMpIdAndByRootCategoryId(IEnumerable<int> attributeIds, int rootCategoryId)
    {
        return _dbContext.Attributes
            .Where(attribute =>
                attribute.RootCategoryId == rootCategoryId && attributeIds.Contains(attribute.Id))
            .Include(a => a.SubAttributes)
            .ToAsyncEnumerable();
    }
}
