using System.Linq;
using System.Reflection;
using NHibernate.Linq;

namespace CodeSmith.Data.NHibernate
{
    public class NHibernateDataContextProvider : IDataContextProvider
    {
        private static readonly object _sessionFieldLock = new object();

        private static FieldInfo _sessionField = null;

        private static FieldInfo SessionField
        {
            get
            {
                lock (_sessionFieldLock)
                    if (_sessionField == null)
                    {
                        var type = typeof(NhQueryProvider);
                        _sessionField = type.GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance);
                    }

                return _sessionField;
            }
        }

        public static DataContext GetDataContext(IQueryable query)
        {
            var provider = query.Provider as NhQueryProvider;
            if (provider == null)
                return null;

            var session = SessionField.GetValue(provider);
            if (session == null)
                return null;

            return DataContext.GetBySession(session);
        }

        CodeSmith.Data.IDataContext IDataContextProvider.GetDataConext(IQueryable query)
        {
            return GetDataContext(query);
        }
    }
}