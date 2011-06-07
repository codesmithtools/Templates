using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.LinqToSql
{
    public class LinqToSqlQueryResultCacheHelper : IQueryResultCacheHelper
    {
        public string GetDbName(IQueryable query)
        {
            var db = query.GetDataContext();

            return db != null && db.Connection != null
                ? db.Connection.ConnectionString
                : String.Empty;
        }

        public void Detach<T>(ICollection<T> results, IQueryable query)
        {
            foreach (var item in results)
            {
                var entity = item as ILinqEntity;
                if (entity != null)
                    entity.Detach();
            }
        }
    }
}
