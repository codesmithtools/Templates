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
    }
}
