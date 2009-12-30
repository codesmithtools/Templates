using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Provides for defering the execution of a count query to a batch of queries.
    /// </summary>
    /// <example>The following is an example of how to use FutureCount to page a 
    /// list and get the total count in one call.
    /// <code><![CDATA[
    /// var db = new TrackerDataContext { Log = Console.Out };
    /// // base query
    /// var q = db.Task.ByPriority(Priority.Normal).OrderBy(t => t.CreatedDate);
    /// // get total count
    /// var q1 = q.FutureCount();
    /// // get first page
    /// var q2 = q.Skip(0).Take(10).Future();
    /// // triggers sql execute as a batch
    /// var tasks = q2.ToList();
    /// int total = q1.Value;
    /// ]]>
    /// </code>
    /// </example>
    [DebuggerDisplay("IsLoaded={IsLoaded}, Value={ValueForDebugDisplay}")]
    public class FutureCount : FutureValue<int>
    {
        private static MethodInfo _countMethod;

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
            IFutureQuery futureQuery = this;
            var source = futureQuery.Query;

            // get static count method
            FindCountMethod();
            // create count expression
            var genericCount = _countMethod.MakeGenericMethod(new[] { source.ElementType });
            var expression = Expression.Call(null, genericCount, source.Expression);

            // get provider from DataContext
            Type contextType = dataContext.GetType();
            PropertyInfo providerProperty = contextType.GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            if (providerProperty == null)
                throw new FutureException("Failed to get the DataContext.Provider property.");

            object provider = providerProperty.GetValue(dataContext, null);
            if (provider == null)
                throw new FutureException("Failed to get the DataContext provider instance.");

            Type providerType = provider.GetType().GetInterface("IProvider");
            if (providerType == null)
                throw new FutureException("Failed to cast the DataContext provider to IProvider.");

            MethodInfo commandMethod = providerType.GetMethod("GetCommand", BindingFlags.Instance | BindingFlags.Public);
            if (commandMethod == null)
                throw new FutureException("Failed to get the GetCommand method from the DataContext provider.");

            // run the GetCommand method from the provider directly
            var commandObject = commandMethod.Invoke(provider, new object[] { expression });
            return commandObject as DbCommand;
        }

        private static void FindCountMethod()
        {
            if (_countMethod != null)
                return;

            var type = typeof(Queryable);

            _countMethod = (from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                            where m.Name == "Count"
                              && m.IsGenericMethod
                              && m.GetParameters().Length == 1
                            select m).FirstOrDefault();
        }
    }


}
