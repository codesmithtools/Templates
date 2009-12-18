using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Provides for defering the execution of a count query to a batch of queries.
    /// </summary>
    [DebuggerDisplay("IsLoaded={IsLoaded}, Value={ValueForDebugDisplay}")]
    public class FutureCount : FutureValue<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        public FutureCount(IQueryable query, Action loadAction)
            : base(query, loadAction, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        public FutureCount(IQueryable query, Action loadAction, CacheSettings cacheSettings)
            : base(query, loadAction, cacheSettings)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="underlyingValue">The underlying value.</param>
        public FutureCount(int underlyingValue)
            : base(underlyingValue)
        { }

        /// <summary>
        /// Gets the key used when caching the results.
        /// </summary>
        /// <returns></returns>
        protected override string GetKey()
        {
            // override the key because the sql is changed later
            return base.GetKey() + "_count";
        }

        /// <summary>
        /// Gets the data command for this query.
        /// </summary>
        /// <param name="dataContext">The data context to get the command from.</param>
        /// <returns>The requested command object.</returns>
        protected override DbCommand GetCommand(System.Data.Linq.DataContext dataContext)
        {
            var command = base.GetCommand(dataContext);
            if (command == null)
                return command;

            // rewrite the comand text to count(*)
            string commandText = command.CommandText;

            // hard coded search, matches pattern in SqlFormatter
            string fromText = commandText.Substring(commandText.IndexOf("\r\nFROM "));
            string countText = "SELECT COUNT(*) AS [value] " + fromText;

            command.CommandText = countText;

            return command;
        }
    }


}
