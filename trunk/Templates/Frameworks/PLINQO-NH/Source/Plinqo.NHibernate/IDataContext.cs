using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace Plinqo.NHibernate
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

        IDataContextSessions Sessions { get; }
    }
}
