namespace Cqrs.Api.Common.DataAccess.EventStore
{
    /// <summary>
    /// Represents the base class for all events in the event store.
    /// </summary>
    public abstract class BaseEvent
    {
        /// <summary>
        /// Gets the unique identifier for the event.
        /// </summary>
        public Guid EventId { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Gets the timestamp indicating when the event occurred.
        /// </summary>
        public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the identifier of the user or system that triggered the event.
        /// </summary>
        public string? TriggeredBy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        protected BaseEvent() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class with the specified trigger information.
        /// </summary>
        /// <param name="triggeredBy">The identifier of the user or system that triggered the event.</param>
        protected BaseEvent(string? triggeredBy)
        {
            TriggeredBy = triggeredBy;
        }
    }
}
