namespace Cqrs.Api.Common.DataAccess.Persistence.Interfaces
{
    /// <summary>
    /// Represents an event store that allows appending and loading events for a specific stream.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Appends an event to the specified stream asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to append.</typeparam>
        /// <param name="streamId">The unique identifier of the stream.</param>
        /// <param name="event">The event to append to the stream.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AppendEventAsync<TEvent>(string streamId, TEvent @event);

        /// <summary>
        /// Loads all events from the specified stream asynchronously.
        /// </summary>
        /// <param name="streamId">The unique identifier of the stream.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of events.</returns>
        Task<IReadOnlyList<object>> LoadEventsAsync(string streamId);
    }
}
