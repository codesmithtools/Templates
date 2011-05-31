using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace Plinqo.NHibernate
{
    public abstract class DataContext : IDataContext
    {
        #region Session Implementation

        protected virtual string GetConnectionString(string databaseName)
        {
            foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
                if (CompareConnectionName(connection, "hibernate", databaseName))
                    return connection.ConnectionString;

            throw new ApplicationException("Connection String Not Found");
        }

        protected bool CompareConnectionName(ConnectionStringSettings connection, params string[] names)
        {
            return names.Any(name => connection.Name.Contains(name.ToLower()));
        }

        protected virtual ISessionFactory CreateSessionFactory(string databaseName, string assemblyName, string dialect, string connectionDriver)
        {
            var config = new Configuration();

            var connectionString = GetConnectionString(databaseName);

            config.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            config.SetProperty(Environment.Dialect, dialect);
            config.SetProperty(Environment.ConnectionDriver, connectionDriver);
            config.SetProperty(Environment.ConnectionString, connectionString);
            config.SetProperty(Environment.ProxyFactoryFactoryClass, "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            config.AddAssembly(assemblyName);

            return config.BuildSessionFactory();
        }

        protected abstract ISession CreateSession();

        protected abstract IStatelessSession CreateStatelessSession();

        #endregion

        #region Declarations

        private bool _isDisposed = false;

        private bool _objectTrackingEnabled = true;

        private IStateSession<ISession> _statefulSession = null;

        private IStateSession<IStatelessSession> _statelessSession = null;

        #endregion

        #region Destructor

        ~DataContext()
        {
            Dispose(true);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(false);
        }

        private void Dispose(bool finalizing)
        {
            if (_isDisposed)
                return;

            if (_statefulSession != null)
                _statefulSession.Dispose();

            if (_statelessSession != null)
                _statelessSession.Dispose();

            if (!finalizing)
                GC.SuppressFinalize(this);

            _isDisposed = true;
        }

        #endregion

        #region Methods

        public void SubmitChanges()
        {
            if (_statefulSession != null)
                _statefulSession.Session.Flush();
            else if (!ObjectTrackingEnabled)
                throw new Exception("Can not SubmitChanges when ObjectTrackingEnabled is false.");
        }

        public bool ObjectTrackingEnabled
        {
            get { return _objectTrackingEnabled; }
            set
            {
                if (GetDefaultStateSession(false) != null)
                    throw new Exception("Can not change ObjectTrackingEnabled after a session has been instantiated.");

                _objectTrackingEnabled = value;
            }
        }

        public ITransaction BeginTransaction()
        {
            return GetDefaultStateSession()
                .BeginTransaction();
        }

        public void CommitTransaction()
        {
            var session = GetDefaultStateSession(false);
            if (session != null)
                session.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            var session = GetDefaultStateSession(false);
            if (session != null)
                session.RollbackTransaction();
        }

        public IStateSession GetDefaultStateSession()
        {
            return GetDefaultStateSession(true);
        }

        private IStateSession GetDefaultStateSession(bool create)
        {
            if (create)
                return ObjectTrackingEnabled
                    ? (IStateSession)StatefulSession
                    : (IStateSession)StatelessSession;

            return ObjectTrackingEnabled
                ? (IStateSession)_statefulSession
                : (IStateSession)_statelessSession;
        }

        #endregion

        #region Properties

        public IStateSession<ISession> StatefulSession
        {
            get
            {
                if (_statefulSession == null)
                {
                    var session = CreateSession();
                    _statefulSession = new StatefulSession(session);
                }

                return _statefulSession;
            }
        }

        public IStateSession<IStatelessSession> StatelessSession
        {
            get
            {
                if (_statelessSession == null)
                {
                    var session = CreateStatelessSession();
                    _statelessSession = new StatelessSession(session);
                }

                return _statelessSession;
            }
        }

        public ITransaction Transaction
        {
            get
            {
                var session = GetDefaultStateSession(false);
                return session == null ? null : session.Transaction;
            }
        }

        public bool HasOpenTransaction
        {
            get
            {
                var session = GetDefaultStateSession(false);
                return session != null && session.HasOpenTransaction;
            }
        }

        public bool IsOpen
        {
            get
            {
                var session = GetDefaultStateSession(false);
                return session != null && session.IsOpen;
            }
        }

        #endregion
    }
}
