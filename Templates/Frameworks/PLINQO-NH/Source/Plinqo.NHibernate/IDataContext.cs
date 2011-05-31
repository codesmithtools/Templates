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

        IStateSession GetDefaultStateSession(); 

        bool ObjectTrackingEnabled { get; set; }

        IStateSession<ISession> StatefulSession { get; }

        IStateSession<IStatelessSession> StatelessSession { get; }

        ITransaction BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        ITransaction Transaction { get; }

        bool HasOpenTransaction { get; }

        bool IsOpen { get; }
    }
}
