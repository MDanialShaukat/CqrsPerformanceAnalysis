using Traditional.Api.Common.DataAccess.Entities;

namespace Traditional.Api.Common.DataAccess.Repositories;

/// <summary>
/// Repository for cached items of a specific type.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public interface ICachedRepository<TItem>
    where TItem : BaseEntity
{
    /// <summary>
    /// Gets all items.
    /// </summary>
    /// <returns>A list of items.</returns>
    Task<List<TItem>> GetAllAsync();

    /// <summary>
    /// Gets an item by its id.
    /// </summary>
    /// <param name="id">The id of the item.</param>
    /// <returns>An item, or null if it was not found.</returns>
    Task<TItem?> GetByIdAsync(int id);
}
