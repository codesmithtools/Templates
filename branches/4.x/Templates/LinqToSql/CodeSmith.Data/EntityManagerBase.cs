using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace CodeSmith.Data
{
    /// <summary>
    /// A base class for entity managers.
    /// </summary>
    /// <typeparam name="TManager">The type of the manager.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class EntityManagerBase<TManager, TEntity> 
        : IEntityManager<TManager, TEntity> 
        where TManager : IDataManager
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManagerBase&lt;TManager, TEntity&gt;"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        protected EntityManagerBase(TManager manager)
        {
            _manager = manager;
            AddValidationRules();
        }

        /// <summary>
        /// Add validation rules.
        /// </summary>
        protected virtual void AddValidationRules()
        {

        }

        private TManager _manager;

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public TManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Gets an entity by the primary key.
        /// </summary>
        /// <param name="key">The key for the entity.</param>
        /// <returns>
        /// An instance of the entity or null if not found.
        /// </returns>
        /// <remarks>
        /// This method provides a common retrieval of an entity.
        /// </remarks>
        /// <exception cref="NotImplementedException"></exception>
        public virtual TEntity GetByKey(IEntityKey key)
        {
            throw new NotImplementedException();
        }

    }
}
