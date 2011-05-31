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

        #endregion

        #region Constructor & Destructor
        
        protected DataContext()
        {
            Sessions = new DataContextSessions(this);
        }

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

            Sessions.Dispose();

            if (!finalizing)
                GC.SuppressFinalize(this);

            _isDisposed = true;
        }

        #endregion

        #region Methods

        public void SubmitChanges()
        {
            if (Sessions.HasSession)
                Sessions.Session.Flush();
            else if (!ObjectTrackingEnabled)
                throw new InvalidOperationException("Can not SubmitChanges when ObjectTrackingEnabled is false.");
        }

        public bool ObjectTrackingEnabled
        {
            get { return _objectTrackingEnabled; }
            set
            {
                if (Sessions.HasDefaultSession)
                    throw new InvalidOperationException("Can not change ObjectTrackingEnabled after a session has been instantiated.");

                _objectTrackingEnabled = value;
            }
        }

        public ITransaction BeginTransaction()
        {
            return Sessions.DefaultSession.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (Sessions.HasDefaultSession)
                Sessions.DefaultSession.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            if (Sessions.HasDefaultSession)
                Sessions.DefaultSession.RollbackTransaction();
        }

        #endregion

        #region Properties

        public bool HasOpenTransaction
        {
            get { return Sessions.HasDefaultSession && Sessions.DefaultSession.HasOpenTransaction; }
        }

        public bool IsOpen
        {
            get { return Sessions.HasDefaultSession && Sessions.DefaultSession.IsOpen; }
        }

        public IDataContextSessions Sessions { get; private set; }

        #endregion

        public class DataContextSessions : IDataContextSessions
        {
            #region Declarations

            private readonly DataContext _dataContext;

            private bool _isDisposed = false;

            private IStateSession<ISession> _stateSession = null;

            private IStateSession<IStatelessSession> _statelessStateSession = null;

            #endregion

            #region Constructor & Destructor

            public DataContextSessions(DataContext dataContext)
            {
                _dataContext = dataContext;
            }

            ~DataContextSessions()
            {
                Dispose();
            }

            #endregion

            #region IDisposable

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool finalizing)
            {
                if (_isDisposed)
                    return;

                if (HasSession)
                    _stateSession.Dispose();

                if (HasStatelessSession)
                    _statelessStateSession.Dispose();

                if (!finalizing)
                    GC.SuppressFinalize(this);

                _isDisposed = true;
            }

            #endregion

            #region Properties

            public bool HasSession
            {
                get { return _stateSession != null; }
            }

            public ISession Session
            {
                get { return StateSession.Session; }
            }

            private IStateSession<ISession> StateSession
            {
                get
                {
                    if (!HasSession)
                    {
                        var session = _dataContext.CreateSession();
                        _stateSession = new StatefulSession(session);
                    }

                    return _stateSession;
                }
            }

            public bool HasStatelessSession
            {
                get { return _statelessStateSession != null; }
            }

            public IStatelessSession StatelessSession
            {
                get { return StatelessStateSession.Session; }
            }

            private IStateSession<IStatelessSession> StatelessStateSession
            {
                get
                {
                    if (!HasStatelessSession)
                    {
                        var session = _dataContext.CreateStatelessSession();
                        _statelessStateSession = new StatelessSession(session);
                    }

                    return _statelessStateSession;
                }
            }

            public bool HasDefaultSession
            {
                get { return _dataContext.ObjectTrackingEnabled ? HasSession : HasStatelessSession; }
            }

            public IStateSession DefaultSession
            {
                get
                {
                    return _dataContext.ObjectTrackingEnabled
                        ? (IStateSession)StateSession
                        : (IStateSession)StatelessStateSession;
                }
            }

            #endregion
        }
    }
}
