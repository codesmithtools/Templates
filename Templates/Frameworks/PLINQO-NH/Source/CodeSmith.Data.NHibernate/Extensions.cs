using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using CodeSmith.Data.NHibernate;
using NHibernate;
using NHibernate.Linq;

namespace Plinqo.NHibernate
{
    public class NHibernateQueryResultCacheHelper : IQueryResultCacheHelper
    {
        public string GetDbName(IQueryable query)
        {
            var db = query.GetDataContext();

            return db == null
                ? String.Empty
                : db.DatabaseName;
        }

        public void Detach<T>(ICollection<T> results, IQueryable query)
        {
            var db = query.GetDataContext();

            if (db == null)
                return;

            if (!db.ObjectTrackingEnabled || !db.Advanced.HasSession)
                return;

            try
            {
                foreach (var result in results)
                    db.Advanced.Session.Evict(result);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }

    internal class FutureValue<T> : IFutureValue<T>
    {
        private readonly T _value;

        public FutureValue(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }
}
