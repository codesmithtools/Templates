using System;
using System.Collections.Generic;
using NHibernate;

namespace Plinqo.NHibernate
{
    public interface IDataContext : IDisposable
    {
        void SubmitChanges();

        void Refresh(object entity);

        void RefreshAll(IEnumerable<object> entities);

        ITransaction BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        ISession Session { get; }

        bool HasOpenTransaction { get; }

        bool IsOpen { get; }
    }
}
