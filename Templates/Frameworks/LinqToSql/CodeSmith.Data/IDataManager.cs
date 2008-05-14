using System;
using System.Data.Linq;
using System.Collections.Generic;

using CodeSmith.Data.Rules;

namespace CodeSmith.Data
{
    /// <summary>
    /// An interface defining a data manager.
    /// </summary>
    /// <typeparam name="TContext">The type of the DataContext.</typeparam>
    public interface IDataManager<TContext> : IDataManager
        where TContext : DataContext
    {
        /// <summary>
        /// Gets the DataContext for this manager.
        /// </summary>
        /// <value>The DataContext.</value>
        TContext Context { get; }
    }

    /// <summary>
    /// An interface defining a data manager.
    /// </summary>
    public interface IDataManager
    {
        RuleManager Rules { get; }
    }
}
