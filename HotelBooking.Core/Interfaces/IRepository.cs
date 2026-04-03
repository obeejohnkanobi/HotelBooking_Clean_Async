using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Core
{
    /// <summary>
    /// Generic abstraction for asynchronous CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity type handled by the repository.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>All entities in the repository.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets a single entity by identifier.
        /// </summary>
        /// <param name="id">Entity identifier.</param>
        /// <returns>The entity when found; otherwise <c>null</c>.</returns>
        Task<T> GetAsync(int id);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        Task EditAsync(T entity);

        /// <summary>
        /// Removes an entity by identifier.
        /// </summary>
        /// <param name="id">Entity identifier.</param>
        Task RemoveAsync(int id);
    }
}
