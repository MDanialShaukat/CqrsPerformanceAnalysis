using Cqrs.Api.UseCases.Categories.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Attribute = Cqrs.Api.UseCases.Attributes.Common.Persistence.Entities.Attribute;

namespace Cqrs.Api.UseCases.Attributes.Common.Persistence.Configuration
{
    /// <inheritdoc />
    internal class AttributeConfigurations : IEntityTypeConfiguration<Attribute>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Attribute> builder)
        {
            builder.HasKey(attribute => attribute.Id);

            builder.Property(attribute => attribute.Name).IsRequired();
            builder.Property(attribute => attribute.ValueType).IsRequired();
            builder.Property(attribute => attribute.MinValues).IsRequired();
            builder.Property(attribute => attribute.MaxValues).IsRequired();

            builder.Property(attribute => attribute.MarketplaceAttributeIds).IsRequired();

            builder.Property(attribute => attribute.AllowedValues).IsRequired(false);

            builder.Property(attribute => attribute.MinLength)
                .HasPrecision(24, 6)
                .HasColumnType("decimal(24,6)")
                .IsRequired(false);

            builder.Property(attribute => attribute.MaxLength)
                .HasPrecision(24, 6)
                .HasColumnType("decimal(24,6)")
                .IsRequired(false);

            const string sqlServerComputedColumn = @"SUBSTRING([marketplace_attribute_ids], 1, 
                    CAST((CHARINDEX(',', [marketplace_attribute_ids]) - 1 
                    + (1 - ROUND(CAST(CHARINDEX(',', [marketplace_attribute_ids]) AS FLOAT) 
                    / (1.0 * CHARINDEX(',', [marketplace_attribute_ids]) + 1), 0))) 
                    * (LEN([marketplace_attribute_ids]) + 1) AS INT))";

            builder
                .Property(attribute => attribute.ProductType)
                .HasComputedColumnSql(sqlServerComputedColumn, stored: true)
                .IsRequired();

            builder.Property(attribute => attribute.IsEditable).IsRequired();
            builder.Property(attribute => attribute.ExampleValues).IsRequired(false);
            builder.Property(attribute => attribute.Description).IsRequired(false);

            // Configure relationship to ParentAttribute with NoAction to avoid cycles
            builder.HasOne(attribute => attribute.ParentAttribute)
                .WithMany(attribute => attribute.SubAttributes)
                .HasForeignKey(attribute => attribute.ParentAttributeId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // Configure relationship to RootCategory (assuming this is okay with Cascade)
            builder.HasOne(attribute => attribute.RootCategory)
                .WithMany(rootCategory => rootCategory.Attributes)
                .HasForeignKey(attribute => attribute.RootCategoryId)
                .IsRequired();

            // Configure many-to-many relationship between Attributes and Categories
            builder
                .HasMany(a => a.Categories)
                .WithMany(c => c.Attributes)
                .UsingEntity<Dictionary<string, object>>(
                    "attributes_categories",
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("categories_id")
                        .OnDelete(DeleteBehavior.NoAction),  // Changed to NoAction
                    j => j
                        .HasOne<Attribute>()
                        .WithMany()
                        .HasForeignKey("attributes_id")
                        .OnDelete(DeleteBehavior.NoAction),  // Changed to NoAction
                    j =>
                    {
                        j.HasKey("attributes_id", "categories_id");
                    });

            builder.HasIndex(attribute => attribute.ProductType)
                .IsUnique(false);

            builder.HasIndex(attribute => attribute.MarketplaceAttributeIds)
                .IsUnique(false);

            builder.HasIndex(attribute => attribute.ValueType)
                .IsUnique(false);
        }
    }
}
