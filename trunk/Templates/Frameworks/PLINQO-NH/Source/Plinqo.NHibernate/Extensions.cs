using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Plinqo.NHibernate
{
    public static class Extensions
    {
        #region Futures

        public static IEnumerable<T> Future<T>(this IQueryable<T> query)
        {
            return query.ToFuture();
        }

        public static IFutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> query)
        {
            return query.ToFutureValue();
        }

        #endregion

        #region Utilities

        public static DataContext GetDataContext(this IQueryable query)
        {
            var property = query
                .GetType()
                .GetProperty("Provider", BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return null;

            var provider = property.GetValue(query, null) as NhQueryProvider;
            if (provider == null)
                return null;

            var member = provider
                .GetType()
                .GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance);
            if (member == null)
                return null;

            var session = member.GetValue(provider);
            if (session == null)
                return null;

            return DataContext.GetBySession(session);
        }

        public static string GetHashKey(this IQueryable query)
        {
            return NHibernateQueryResultCache.Instance.GetHashKey(query);
        }

        #endregion
    }

    public class NHibernateQueryResultCache : QueryResultCache
    {
        public static QueryResultCache Instance { get; private set; }

        static NHibernateQueryResultCache()
        {
            Instance = new NHibernateQueryResultCache();
        }

        private NHibernateQueryResultCache()
        {
        }

        protected override string GetDbName(IQueryable query)
        {
            var db = query.GetDataContext();

            return db == null
                ? String.Empty
                : db.DatabaseName;
        }

        protected override void Detach<T>(ICollection<T> results, IQueryable query)
        {
            var db = query.GetDataContext();

            if (db == null)
                return;

            if (!db.ObjectTrackingEnabled || !db.Sessions.HasSession)
                return;

            try
            {
                foreach (var result in results)
                    db.Sessions.Session.Evict(result);
            }
            catch
            {
            }
        }
    }
}
