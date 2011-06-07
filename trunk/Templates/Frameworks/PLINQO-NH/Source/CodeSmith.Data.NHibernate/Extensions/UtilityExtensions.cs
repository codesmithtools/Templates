using System.Linq;
using System.Reflection;
using CodeSmith.Data.Caching;
using CodeSmith.Data.NHibernate;
using NHibernate.Linq;

namespace CodeSmith.Data.Linq
{
    public static class UtilityExtensions
    {
        private static FieldInfo _sessionField = null;

        private static FieldInfo SessionField
        {
            get
            {
                if (_sessionField == null)
                {
                    var type = typeof(NhQueryProvider);
                    _sessionField = type.GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance);
                }

                return _sessionField;
            }
        }

        public static DataContext GetDataContext(this IQueryable query)
        {
            var provider = query.Provider as NhQueryProvider;
            if (provider == null)
                return null;

            var session = SessionField.GetValue(provider);
            if (session == null)
                return null;

            return DataContext.GetBySession(session);
        }

        public static string GetHashKey(this IQueryable query)
        {
            return QueryResultCache.GetHashKey(query);
        }
    }
}