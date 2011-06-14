using System;

namespace CodeSmith.Data
{
    public interface IDataContext : IDisposable
    {
        string ConnectionString { get; }

        void Detach(params object[] enities);

        void SubmitChanges();

        bool ObjectTrackingEnabled { get; set; }

        IDisposable BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        bool HasOpenTransaction { get; }
    }
}