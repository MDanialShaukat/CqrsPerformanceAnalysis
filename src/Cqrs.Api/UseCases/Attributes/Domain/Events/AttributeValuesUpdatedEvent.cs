using Cqrs.Api.Common.DataAccess.EventStore;
using Cqrs.Api.UseCases.Attributes.Commands.UpdateAttributeValues;

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
        /// Gets the collection of new attribute values.
        /// </summary>
        public NewAttributeValue[] NewAttributeValues { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValuesUpdatedEvent"/> class.
        /// </summary>
        /// <param name="articleNumber">The article number associated with the update.</param>
        /// <param name="rootCategoryId">The root category ID associated with the update.</param>
        /// <param name="newAttributeValues">The new attribute values.</param>
        /// <param name="triggeredBy">The user or process that triggered the event.</param>
        public AttributeValuesUpdatedEvent(
            string articleNumber,
            int rootCategoryId,
            NewAttributeValue[] newAttributeValues,
            string? triggeredBy = null)
            : base(triggeredBy)
        {
            ArticleNumber = articleNumber;
            RootCategoryId = rootCategoryId;
            NewAttributeValues = newAttributeValues;
        }
    }
}
