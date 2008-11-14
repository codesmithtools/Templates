using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.Linq.Provider;
using System.Data.Linq;

using CodeSmith.Data.Rules;

namespace CodeSmith.Data
{
    /// <summary>
    /// The base class for the data manager.
    /// </summary>
    /// <typeparam name="TContext">The type of DataContext.</typeparam>
    public class DataManagerBase<TContext>
        : IDataManager<TContext> where TContext : DataContext
    {
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

        private readonly TContext _context;

        /// <summary>
        /// Gets the managers DataContext.
        /// </summary>
        /// <value>The current DataContext.</value>
        public TContext Context
        {
            get { return _context; }
        }

        private readonly RuleManager _rules;

        /// <summary>
        /// Gets the current rules for the entities.
        /// </summary>
        /// <value>The entity rules.</value>
        [Obsolete("Use DataContext.RuleManager instead.")]
        public RuleManager Rules
        {
            get { return _rules; }
        }

    }
}
