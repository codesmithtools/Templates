namespace CodeSmith.Data
{
    /// <summary>
    /// The entity manager interface.
    /// </summary>
    /// <typeparam name="TManager">The type of the manager.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IDataManager"/>
    /// <seealso cref="IEntityKey"/>
    public interface IEntityManager<TManager, TEntity>
        where TManager : IDataManager
        where TEntity : class
    {
        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>The manager.</value>
        TManager Manager { get; }

        /// <summary>
        /// Gets an entity by the primary key.
        /// </summary>
        /// <param name="key">The key for the entity.</param>
        /// <returns>
        /// An instance of the entity or null if not found.
        /// </returns>
        TEntity GetByKey(IEntityKey key);
    }
}