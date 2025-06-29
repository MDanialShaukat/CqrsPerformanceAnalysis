using Cqrs.Api.Common.DataAccess.Persistence;
using Cqrs.Api.UseCases.Articles.Errors;
using Cqrs.Api.UseCases.Articles.Persistence.Entities;
using Cqrs.Api.UseCases.Attributes.Domain.Projections;
using Cqrs.Api.UseCases.Categories.Common.Errors;
using Cqrs.Api.UseCases.Categories.Common.Persistence.Entities;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Cqrs.Api.UseCases.Categories.Commands.UpdateCategoryMapping;

/// <summary>
/// Provides functionality to update the category mapping for an article.
/// </summary>
public class UpdateCategoryMappingCommandHandler(CqrsWriteDbContext _dbContext,
    Marten.IDocumentSession _session)
{
    /// <summary>
    /// Updates the category mapping for an article.
    /// </summary>
    /// <param name="command">Provides the information for which category mapping should be updated.</param>
    /// <returns>An <see cref="ErrorOr.Error"/> or the new mapped <see cref="Category"/> of the article.</returns>
    public async Task<ErrorOr<Category>> UpdateCategoryMappingAsync(UpdateCategoryMappingCommand command)
    {
        // 1 Load from Marten State - ES and DDD
        var projectionId = $"{command.ArticleNumber}-{command.RootCategoryId}";
        var projection = await _session.LoadAsync<ArticleAttributeProjection>(projectionId);
        if (projection is null)
        {
            return ArticleErrors.ProjectionNotFound(projectionId);
        }

        List<Article> articles = projection.Articles
            .Select(dto => new Article(command.ArticleNumber, dto.CharacteristicId)
            {
                Id = dto.ArticleId,
                Categories = null,
                AttributeBooleanValues = null,
                AttributeDecimalValues = null,
                AttributeIntValues = null,
                AttributeStringValues = null
            })
            .ToList();

        // 2. Retrieve the requested category
        var category = await GetByNumberAndRootCategoryId(command.RootCategoryId, command.CategoryNumber);

        // If no category was found return a not found error
        if (category is null)
        {
            return CategoryErrors.CategoryNotFound(command.CategoryNumber, command.RootCategoryId);
        }

        // 3. Update the category mapping for the articles and return the new associated category
        await UpdateCategoryMappingForArticlesAsync(articles, category, command.RootCategoryId);

        return category;
    }

    private async Task UpdateCategoryMappingForArticlesAsync(
        List<Article> articles,
        Category newCategory,
        int rootCategoryId)
    {
        foreach (var article in articles)
        {
            // If Categories is null, initialize it
            if (article.Categories == null)
            {
                article.Categories = new List<Category> { newCategory };
            }
            else
            {
                // An article can have multiple categories on different roots but only one per root
                article.Categories.RemoveAll(category => category.RootCategoryId == rootCategoryId);
                article.Categories.Add(newCategory);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the categories by the category number and the root category id.
    /// </summary>
    /// <param name="rootCategoryId">The root category id to search for.</param>
    /// <param name="categoryNumber">The category number to search for.</param>
    /// <returns>A <see cref="Category"/> or <see langword="null"/> if not found.</returns>
    private async Task<Category?> GetByNumberAndRootCategoryId(int rootCategoryId, long categoryNumber)
    {
        return await _dbContext.Categories
            .SingleOrDefaultAsync(category =>
                category.RootCategoryId == rootCategoryId
                && category.CategoryNumber == categoryNumber);
    }
}
