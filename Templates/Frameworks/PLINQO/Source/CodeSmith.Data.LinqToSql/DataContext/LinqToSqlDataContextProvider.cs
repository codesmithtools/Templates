using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeSmith.Data.Future;

namespace CodeSmith.Data.LinqToSql
{
    public class LinqToSqlDataContextProvider : IDataContextProvider
    {
        public static DataContext GetDataContext(IQueryable query)
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

        IDataContext IDataContextProvider.GetDataConext(IQueryable query)
        {
            return GetDataContext(query) as IDataContext;
        }

        public IFutureContext GetFutureContext(IQueryable query)
        {
            return GetDataContext(query) as IFutureContext;
        }
    }
}
