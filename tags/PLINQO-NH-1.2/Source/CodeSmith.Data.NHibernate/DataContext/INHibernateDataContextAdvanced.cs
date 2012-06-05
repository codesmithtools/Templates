using System;
using System.Collections.Generic;
using CodeSmith.Data.Future;
using NHibernate;

namespace CodeSmith.Data.NHibernate
{
    public interface INHibernateDataContextAdvanced : IDisposable
    {
        bool HasSession { get; }

        ISession Session { get; }

        bool HasStatelessSession { get; }

        IStatelessSession StatelessSession { get; }

        bool HasDefaultSession { get; }

        IStateSession DefaultSession { get; }

        IList<INHibernateFutureQuery> FutureQueries { get; }

        void ExecuteFutureQueries();
    }
}