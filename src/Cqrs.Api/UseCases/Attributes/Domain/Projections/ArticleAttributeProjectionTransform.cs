using Cqrs.Api.UseCases.Attributes.Domain.Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Attribute = Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities.Attribute;

namespace Cqrs.Api.UseCases.Attributes.Domain.Projections
{
    /// <summary>
    /// Represents a projection transform for article attributes.
    /// </summary>
    public class ArticleAttributeProjectionTransform : IProjection
    {
        /// <summary>
        /// Applies the projection logic to the given streams of events.
        /// </summary>
        /// <param name="operations">The document operations interface for storing and retrieving projections.</param>
        /// <param name="streams">The list of event streams to process.</param>
        public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
        {
            foreach (var stream in streams)
            {
                foreach (var @event in stream.Events)
                {
                    if (@event.Data is AttributeValuesUpdatedEvent attributeEvent)
                    {
                        var projectionId = $"{attributeEvent.ArticleNumber}-{attributeEvent.RootCategoryId}";

                        var projection = operations.Load<ArticleAttributeProjection>(projectionId) ??
                            new ArticleAttributeProjection
                            {
                                ArticleNumber = attributeEvent.ArticleNumber,
                                RootCategoryId = attributeEvent.RootCategoryId,
                            };

                        projection.Articles = attributeEvent.Articles;
                        projection.MappedCategoryId = attributeEvent.MappedCategoryId;

                        foreach (var attr in attributeEvent.NewAttributeValues)
                        {
                            var attribute = attributeEvent.Attribute.Find(x => x.Id == attr.AttributeId);
                            var existing = projection.Attributes.Find(x => x.AttributeId == attr.AttributeId);
                            if (existing is null)
                            {
                                existing = new AttributeProjection
                                {
                                    AttributeId = attr.AttributeId,
                                    Values = attr.InnerValues,
                                    AttributeName = attribute?.Name,
                                    SubAttributes = attribute?.SubAttributes ?? new List<Attribute>(),
                                    MaxValues = attribute?.MaxValues ?? 0,
                                    MinValues = attribute?.MinValues ?? 0

                                };
                                existing.SubAttributes?.ForEach(x => x.ParentAttribute = null);
                                projection.Attributes.Add(existing);
                            }
                            else
                            {
                                existing.Values = attr.InnerValues;
                            }
                        }

                        operations.Store(projection);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously applies the projection logic to the given streams of events.
        /// </summary>
        /// <param name="operations">The document operations interface for storing and retrieving projections.</param>
        /// <param name="streams">The list of event streams to process.</param>
        /// <param name="cancellation">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken cancellation)
        {
            Apply(operations, streams);
            return Task.CompletedTask;
        }
    }
}
