using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Extension methods for future queries.
    /// </summary>
    public static class FutureExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.</returns>
        public static IEnumerable<T> Future<T>(this IQueryable<T> source) where T : class
        {
            if (source == null)
                return source;

            var db = GetDataContext(source) as IFutureContext;
            if (db == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            var futureQuery = db.AddFutureQuery(source);
            return futureQuery;
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of FutureCount that contains the result of the query.</returns>
        public static FutureCount FutureCount<T>(this IQueryable<T> source) where T : class
        {
            if (source == null)
                return new FutureCount(0);

            var db = GetDataContext(source) as IFutureContext;
            if (db == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            var futureQuery = db.AddFutureCountQuery(source);
            return futureQuery;
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.</returns>
        public static FutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> source) where T : class
        {
            if (source == null)
                return new FutureValue<T>(default(T));

            // make sure to only get the first value
            var firstQuery = source.Take(1);

            var db = GetDataContext(firstQuery) as IFutureContext;
            if (db == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            var futureQuery = db.AddFutureValueQuery(firstQuery);
            return futureQuery;
        }

        private static DataContext GetDataContext(IQueryable query)
        {
            Type type = query.GetType();
            FieldInfo field = type.GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return null;

            object value = field.GetValue(query);
            if (value == null)
                return null;

            var dataContext = value as DataContext;
            return dataContext;
        }
    }
}
