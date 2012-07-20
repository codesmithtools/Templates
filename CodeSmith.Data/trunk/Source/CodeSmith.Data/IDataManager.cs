using System.Data.Linq;
using CodeSmith.Data.Rules;

namespace CodeSmith.Data
{
    /// <summary>
    /// An interface defining a data manager.
    /// </summary>
    /// <typeparam name="TContext">The type of the <see cref="DataContext"/>.</typeparam>
    public interface IDataManager<TContext> : IDataManager
        where TContext : DataContext
    {
        /// <summary>
        /// Gets the <see cref="DataContext"/> for this manager.
        /// </summary>
        /// <value>The <see cref="DataContext"/>.</value>
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