using Cqrs.Api.UseCases.Attributes.Commands.UpdateAttributeValues;
using Cqrs.Api.UseCases.Attributes.Domain.Events;

namespace Cqrs.Api.UseCases.Attributes.Domain.Aggregates
{
    /// <summary>
    /// Represents an aggregate for managing article attributes.
    /// </summary>
    public class ArticleAttributeAggregate
    {
        private readonly List<object> _events = new();

        /// <summary>
        /// Gets the article number associated with this aggregate.
        /// </summary>
        public string ArticleNumber { get; private set; }

        /// <summary>
        /// Gets the root category ID associated with this aggregate.
        /// </summary>
        public int RootCategoryId { get; private set; }

        /// <summary>
        /// Gets the list of events associated with this aggregate.
        /// </summary>
        public IReadOnlyList<object> Events => _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleAttributeAggregate"/> class.
        /// </summary>
        /// <param name="articleNumber">The article number.</param>
        /// <param name="rootCategoryId">The root category ID.</param>
        public ArticleAttributeAggregate(string articleNumber, int rootCategoryId)
        {
            ArticleNumber = articleNumber;
            RootCategoryId = rootCategoryId;
        }

        /// <summary>
        /// Updates the attributes of the article and records an event.
        /// </summary>
        /// <param name="newAttributeValues">The new attribute values to update.</param>
        public void UpdateAttributes(NewAttributeValue[] newAttributeValues)
        {
            var evt = new AttributeValuesUpdatedEvent(ArticleNumber, RootCategoryId, newAttributeValues);
            _events.Add(evt);
        }

        /// <summary>
        /// Clears all recorded events for this aggregate.
        /// </summary>
        public void ClearEvents() => _events.Clear();
    }
}
