using Cqrs.Api.Common.DataAccess.EventStore;
using Cqrs.Api.UseCases.Attributes.Commands.UpdateAttributeValues;
using Cqrs.Api.UseCases.Attributes.Common.Models;
using Attribute = Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities.Attribute;

namespace Cqrs.Api.UseCases.Attributes.Domain.Events
{
    /// <summary>
    /// Event triggered when attribute values are updated for a specific article.
    /// </summary>
    public class AttributeValuesUpdatedEvent : BaseEvent
    {
        /// <summary>
        /// Gets the article number associated with the updated attribute values.
        /// </summary>
        public string ArticleNumber { get; }

        /// <summary>
        /// Gets the root category ID associated with the updated attribute values.
        /// </summary>
        public int RootCategoryId { get; }

        /// <summary>
        /// Gets the Articles associated with the updated attribute values.
        /// </summary>
        public List<ArticleDto> Articles { get; }

        /// <summary>
        /// Gets the Attribute associated with the updated attribute values.
        /// </summary>
        public List<Attribute> Attribute { get; }

        /// <summary>
        /// Gets the MappedCategory associated with the updated attribute values.
        /// </summary>
        public int MappedCategoryId { get; }

        /// <summary>
        /// Gets the collection of new attribute values.
        /// </summary>
        public NewAttributeValue[] NewAttributeValues { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValuesUpdatedEvent"/> class.
        /// </summary>
        /// <param name="articleNumber">The article number associated with the update.</param>
        /// <param name="rootCategoryId">The root category ID associated with the update.</param>
        /// <param name="mappedCategoryId">The mappedCategoryId.</param>
        /// <param name="articles">The Articles.</param>
        /// <param name="attributes">The Attributes.</param>
        /// <param name="newAttributeValues">The new attribute values.</param>
        /// <param name="triggeredBy">The user or process that triggered the event.</param>
        public AttributeValuesUpdatedEvent(
            string articleNumber,
            int rootCategoryId,
            int mappedCategoryId,
            List<ArticleDto> articles,
            List<Attribute> attributes,
            NewAttributeValue[] newAttributeValues,
            string? triggeredBy = null)
            : base(triggeredBy)
        {
            ArticleNumber = articleNumber;
            RootCategoryId = rootCategoryId;
            Articles = articles;
            Attribute = attributes;
            MappedCategoryId = mappedCategoryId;
            NewAttributeValues = newAttributeValues;
        }
    }
}
