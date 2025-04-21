using Cqrs.Api.UseCases.Attributes.Common.Responses;

namespace Cqrs.Api.UseCases.Attributes.Domain.Projections
{
    /// <summary>
    /// Represents the projection of an attribute, including its ID, name, type, value constraints, and associated sub-attributes and values.
    /// </summary>
    public class AttributeProjection
    {
        /// <summary>
        /// Gets or sets the unique identifier of the attribute.
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        public string? AttributeName { get; set; }

        /// <summary>
        /// Gets or sets the type of the attribute.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of values allowed for the attribute.
        /// </summary>
        public int MaxValues { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of values required for the attribute.
        /// </summary>
        public int MinValues { get; set; }

        /// <summary>
        /// Gets or sets the list of sub-attributes associated with the attribute.
        /// </summary>
        public List<string> SubAttributes { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of variant attribute values associated with the attribute.
        /// </summary>
        public List<VariantAttributeValues> Values { get; set; } = [];
    }
}
