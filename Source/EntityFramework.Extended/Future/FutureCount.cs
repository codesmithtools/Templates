using System;
using System.Data.Common;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.Extensions;

namespace EntityFramework.Future
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
        private static readonly Lazy<MethodInfo> _countMethod = new Lazy<MethodInfo>(FindCountMethod);

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        public FutureCount(IQueryable query, Action loadAction)
            : base(query, loadAction)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="underlyingValue">The underlying value.</param>
        public FutureCount(int underlyingValue)
            : base(underlyingValue)
        { }

        /// <summary>
        /// Gets the data command for this query.
        /// </summary>
        /// <param name="dataContext">The data context to get the command from.</param>
        /// <returns>The requested command object.</returns>
        protected override FuturePlan GetPlan(ObjectContext dataContext)
        {
            IFutureQuery futureQuery = this;
            var source = futureQuery.Query;

            // create count expression
            var expression = Expression.Call(
              typeof(Queryable),
              "Count",
              new[] { source.ElementType },
              source.Expression);

            source = source.Provider.CreateQuery(expression);

            var q = source as ObjectQuery;
            if (q == null)
                throw new InvalidOperationException("The future query is not of type ObjectQuery.");

            var plan = new FuturePlan
            {
                CommandText = q.ToTraceString(),
                Parameters = q.Parameters
            };

            return plan;
        }

        private static MethodInfo FindCountMethod()
        {
            var type = typeof(Queryable);

            return (from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    where m.Name == "Count"
                      && m.IsGenericMethod
                      && m.GetParameters().Length == 1
                    select m).FirstOrDefault();
        }
    }


}
