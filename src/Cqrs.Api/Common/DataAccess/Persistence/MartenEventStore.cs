using Cqrs.Api.Common.DataAccess.Persistence.Interfaces;
using Marten;

namespace Cqrs.Api.Common.DataAccess.Persistence
{
    /// <summary>
    /// Represents an event store implementation using Marten as the underlying persistence mechanism.
    /// </summary>
    public class MartenEventStore : IEventStore
    {
        private readonly IDocumentStore _documentStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MartenEventStore"/> class.
        /// </summary>
        /// <param name="documentStore">The document store used for event persistence.</param>
        public MartenEventStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        /// <summary>
        /// Appends an event to the specified stream.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to append.</typeparam>
        /// <param name="streamId">The identifier of the stream to which the event will be appended.</param>
        /// <param name="event">The event to append.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AppendEventAsync<TEvent>(string streamId, TEvent @event)
        {
            using var session = _documentStore.LightweightSession();
            session.Events.Append(streamId, new[] { @event }); // Wrap the event in an array to match the correct overload
            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Loads all events from the specified stream.
        /// </summary>
        /// <param name="streamId">The identifier of the stream from which to load events.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of events.</returns>
        public async Task<IReadOnlyList<object>> LoadEventsAsync(string streamId)
        {
            using var session = _documentStore.LightweightSession();
            var events = await session.Events.FetchStreamAsync(streamId);
            return events.Select(e => e.Data).ToList();
        }
    }
}
