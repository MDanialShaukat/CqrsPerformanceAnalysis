using JetBrains.Annotations;

namespace Cqrs.Api.UseCases.Categories.Commands.UpdateCategoryMapping;

/// <summary>
/// Represents the response for updating the category mappings of an article.
/// </summary>
/// <param name="CategoryNumber">The new category number of the categories associated with the article.</param>
/// <param name="CategoryPath">The new category path of the categories associated with the article.</param>
[PublicAPI]
public record UpdatedCategoryMappingResponse(
    long CategoryNumber,
    string CategoryPath);
