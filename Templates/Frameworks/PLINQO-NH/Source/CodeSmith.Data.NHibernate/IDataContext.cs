using System;
using NHibernate;
using Plinqo.NHibernate;

namespace CodeSmith.Data.NHibernate
{
    public interface IDataContext : IDisposable
    {
        void SubmitChanges();

        bool ObjectTrackingEnabled { get; set; }

        ITransaction BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        bool HasOpenTransaction { get; }

        bool IsOpen { get; }

        IDataContextAdvanced Advanced { get; }
    }
}
