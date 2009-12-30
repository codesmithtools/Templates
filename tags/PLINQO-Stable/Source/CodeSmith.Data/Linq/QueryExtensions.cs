using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeSmith.Data.Linq
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Gets the <see cref="DataContext"/> that created the query.
        /// </summary>
        /// <param name="query">The query to look for the <see cref="DataContext"/> on.</param>
        /// <returns>
        /// The instance of the <see cref="DataContext"/> that created the query. 
        /// If the query was not created by a <see cref="DataContext"/>, 
        /// <see langword="null"/> is returned. 
        /// </returns>
        public static DataContext GetDataContext(this IQueryable query)
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

        /// <summary>
        /// Determines whether the specified query supports a future query extension.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>
        /// 	<c>true</c> if query supports a future query; otherwise, <c>false</c>.
        /// </returns>
        public static bool SupportsFutureQuery(this IQueryable query)
        {
            var db = query.GetDataContext() as IFutureContext;
            return (db != null);
        }
    }
}
