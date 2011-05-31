using System;
using NHibernate;

namespace Plinqo.NHibernate
{
    public interface IStateSession<T> : IStateSession
    {
        T Session { get; }
    }

    public interface IStateSession : IDisposable
    {
        void Save(object o);

        void Delete(object o);

        IQuery GetNamedQuery(string queryName);

        ITransaction BeginTransaction();
        
        void CommitTransaction();

        void RollbackTransaction();

        ITransaction Transaction { get; }

        bool HasOpenTransaction { get; }

        bool IsOpen { get; }
    }
}