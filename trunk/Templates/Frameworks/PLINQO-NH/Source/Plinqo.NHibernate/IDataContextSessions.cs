using System;
using NHibernate;

namespace Plinqo.NHibernate
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