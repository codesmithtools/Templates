using System;
using System.Data.Linq;
using CodeSmith.Data.Rules;

namespace CodeSmith.Data
{
    /// <summary>
    /// The base class for the data manager.
    /// </summary>
    /// <typeparam name="TContext">The type of <see cref="DataContext"/>.</typeparam>
    public class DataManagerBase<TContext>
        : IDataManager<TContext>
        where TContext : DataContext
    {
        private readonly TContext _context;

        private readonly RuleManager _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManagerBase&lt;TContext&gt;"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DataManagerBase(TContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _rules = new RuleManager();
        }

        #region IDataManager<TContext> Members

        /// <summary>
        /// Gets the managers <see cref="DataContext"/>.
        /// </summary>
        /// <value>The current <see cref="DataContext"/>.</value>
        public TContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the current rules for the entities.
        /// </summary>
        /// <value>The entity rules.</value>
        [Obsolete("Use DataContext.RuleManager instead.")]
        public RuleManager Rules
        {
            get { return _rules; }
        }

        #endregion
    }
}