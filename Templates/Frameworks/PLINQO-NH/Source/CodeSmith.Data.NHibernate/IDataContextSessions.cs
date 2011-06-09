using System;
using NHibernate;

namespace CodeSmith.Data.NHibernate
{
    public interface IDataContextAdvanced : IDisposable
    {
        bool HasSession { get; }

        ISession Session { get; }

        bool HasStatelessSession { get; }

        IStatelessSession StatelessSession { get; }

        bool HasDefaultSession { get; }

        IStateSession DefaultSession { get; }
    }
}