using Microsoft.EntityFrameworkCore;
using Traditional.Api.UseCases.Articles.Persistence.Entities;
using Traditional.Api.UseCases.Attributes.Common.Persistence.Entities;
using Traditional.Api.UseCases.Attributes.Common.Persistence.Entities.AttributeValues;
using Traditional.Api.UseCases.Categories.Common.Persistence.Entities;
using Traditional.Api.UseCases.RootCategories.Common.Persistence.Entities;
using Attribute = Traditional.Api.UseCases.Attributes.Common.Persistence.Entities.Attribute;

namespace Traditional.Api.Common.DataAccess.Persistence;

/// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext" />
public class TraditionalDbContext(DbContextOptions<TraditionalDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets a db set of <see cref="Article"/>s.
    /// </summary>
    public DbSet<Article> Articles => Set<Article>();

    /// <summary>
    /// Gets a db set of <see cref="Attribute"/>s.
    /// </summary>
    public DbSet<Attribute> Attributes => Set<Attribute>();

    /// <summary>
    /// Gets a db set of <see cref="AttributeMapping"/>s.
    /// </summary>
    public DbSet<AttributeMapping> AttributeMappings => Set<AttributeMapping>();

    /// <summary>
    /// Gets a db set of <see cref="Category"/>s.
    /// </summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    /// Gets a db set of <see cref="RootCategory"/>s.
    /// </summary>
    public DbSet<RootCategory> RootCategories => Set<RootCategory>();

    /// <summary>
    /// Gets a db set of <see cref="AttributeBooleanValue"/>s.
    /// </summary>
    public DbSet<AttributeBooleanValue> AttributeBooleanValues => Set<AttributeBooleanValue>();

    /// <summary>
    /// Gets a db set of <see cref="AttributeStringValue"/>s.
    /// </summary>
    public DbSet<AttributeStringValue> AttributeStringValues => Set<AttributeStringValue>();

    /// <summary>
    /// Gets a db set of <see cref="AttributeIntValue"/>s.
    /// </summary>
    public DbSet<AttributeIntValue> AttributeIntValues => Set<AttributeIntValue>();

    /// <summary>
    /// Gets a db set of <see cref="AttributeDecimalValue"/>s.
    /// </summary>
    public DbSet<AttributeDecimalValue> AttributeDecimalValues => Set<AttributeDecimalValue>();

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureDecimalPrecisionAndEnumConversion(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TraditionalDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures the precision of decimal properties and converts enum properties to string.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    private static void ConfigureDecimalPrecisionAndEnumConversion(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if ((property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    && property.FindAnnotation("Relational:ColumnType") == null)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasColumnType("numeric(9,3)");
                }

                if (property.ClrType.IsEnum)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion<string>();
                }
            }
        }
    }
}
