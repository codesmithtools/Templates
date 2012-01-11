using System;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using CodeSmith.Data.Future;

namespace CodeSmith.Data.LinqToSql
{
    /// <summary>
    /// 
    /// </summary>
    public class LinqToSqlDataContextProvider : IDataContextProvider
    {
        public static DataContext GetDataContext(IQueryable query)
        {
            if (query == null)
                return null;

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

        IDataContext IDataContextProvider.GetDataConext(IQueryable query)
        {
            return GetDataContext(query) as IDataContext;
        }

        /// <summary>
        /// Gets the Future Context.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFutureContext GetFutureContext(IQueryable query)
        {
            return GetDataContext(query) as IFutureContext;
        }
    }
}
