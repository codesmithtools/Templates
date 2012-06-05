using System.Linq;
using System.Reflection;
using CodeSmith.Data.Future;
using CodeSmith.Data.NHibernate;
using NHibernate.Linq;

namespace CodeSmith.Data.NHibernate2
{
    public class NHibernateDataContextProvider : IDataContextProvider
    {
        private static readonly object _sessionFieldLock = new object();

        private static PropertyInfo _sessionField = null;

        private static PropertyInfo SessionField
        {
            get
            {
                lock (_sessionFieldLock)
                    if (_sessionField == null)
                    {
                        var type = typeof(DefaultQueryProvider);
                        _sessionField = type.GetProperty("Session", BindingFlags.NonPublic | BindingFlags.Instance);
                    }

                return _sessionField;
            }
        }

        public static NHibernateDataContext GetDataContext(IQueryable query)
        {
            var provider = query.Provider as DefaultQueryProvider;
            if (provider == null)
                return null;

            var session = SessionField.GetValue(provider, null);
            if (session == null)
                return null;

            return NHibernateDataContext.GetBySession(session);
        }

        public static IFutureContext GetFutureContext(IQueryable query)
        {
            return GetDataContext(query);
        }

        IDataContext IDataContextProvider.GetDataConext(IQueryable query)
        {
            return GetDataContext(query);
        }

        IFutureContext IDataContextProvider.GetFutureContext(IQueryable query)
        {
            return GetFutureContext(query);
        }
    }
}