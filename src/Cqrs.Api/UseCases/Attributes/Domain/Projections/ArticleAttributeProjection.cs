using Cqrs.Api.UseCases.Attributes.Common.Models;

namespace Cqrs.Api.UseCases.Attributes.Domain.Projections
{
    /// <summary>
    /// Represents the projection of an article's attributes, including its unique identifier, article number, root category, and associated attributes.
    /// </summary>
    public class ArticleAttributeProjection
    {
        /// <summary>
        /// Gets the unique identifier for the article, which is a combination of the article number and root category ID.
        /// </summary>
        public string Id => $"{ArticleNumber}-{RootCategoryId}";

        /// <summary>
        /// Gets or sets the article number.
        /// </summary>
        public string? ArticleNumber { get; set; }

        /// <summary>
        /// Gets or sets the root category ID of the article.
        /// </summary>
        public int RootCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the list of articles associated with the projection.
        /// </summary>
        public List<ArticleDto> Articles { get; set; } = [];

        /// <summary>
        /// Gets or sets the mapped category ID.
        /// </summary>
        public int MappedCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the list of attributes associated with the article.
        /// </summary>
        public List<AttributeProjection> Attributes { get; set; } = [];
    }
}
