using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CodeSmith.Data.Caching;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace CodeSmith.Data.NHibernate
{
    public abstract class DataContext : IDataContext
    {
        #region Session Repository

        private static Dictionary<ISession, DataContext> _sessionMap;

        private static Dictionary<ISession, DataContext> SessionMap
        {
            get
            {
                if (_sessionMap == null)
                    _sessionMap = new Dictionary<ISession, DataContext>();

                return _sessionMap;
            }
        }

        private static Dictionary<IStatelessSession, DataContext> _statelessSessionMap;

        private static Dictionary<IStatelessSession, DataContext> StatelessSessionMap
        {
            get
            {
                if (_statelessSessionMap == null)
                    _statelessSessionMap = new Dictionary<IStatelessSession, DataContext>();

                return _statelessSessionMap;
            }
        }

        internal static DataContext GetBySession(object session)
        {
            var iSession = session as ISession;
            if (iSession != null)
                return SessionMap[iSession];

            var iStatelessSession = session as IStatelessSession;
            if (iStatelessSession != null)
                return StatelessSessionMap[iStatelessSession];

            return null;
        }

        #endregion

        #region Session Implementation

        protected bool CompareConnectionName(ConnectionStringSettings connection, params string[] names)
        {
            return names.Any(name => connection.Name.Contains(name.ToLower()));
        }

        protected ISessionFactory CreateSessionFactory(string databaseName, string assemblyName, string dialect, string connectionDriver)
        {
            ConnectionString = GetConnectionString(databaseName);

            var config = new Configuration();
            ConfigureSessionFactory(config, databaseName, assemblyName, dialect, connectionDriver, ConnectionString);
            return config.BuildSessionFactory();
        }

        protected virtual void ConfigureSessionFactory(Configuration config, string databaseName, string assemblyName, string dialect, string connectionDriver, string connectionString)
        {
            config.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            config.SetProperty(Environment.Dialect, dialect);
            config.SetProperty(Environment.ConnectionDriver, connectionDriver);
            config.SetProperty(Environment.ConnectionString, connectionString);
            config.SetProperty(Environment.ProxyFactoryFactoryClass, "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            config.AddAssembly(assemblyName);
        }

        protected virtual string GetConnectionString(string databaseName)
        {
            foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
                if (CompareConnectionName(connection, "hibernate", databaseName))
                    return connection.ConnectionString;

            throw new ApplicationException("Connection String Not Found");
        }

        protected abstract ISession CreateSession();

        protected abstract IStatelessSession CreateStatelessSession();

        #endregion

        #region Declarations

        private bool _isDisposed = false;

        private bool _objectTrackingEnabled = true;

        #endregion

        #region Constructor & Destructor
        
        static DataContext()
        {
            var provider = new NHibernateDataContextProvider();
            DataContextProvider.Register(provider);
        }

        protected DataContext()
        {
            Advanced = new DataContextAdvanced(this);
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

            Advanced.Dispose();

            if (!finalizing)
                GC.SuppressFinalize(this);

            _isDisposed = true;
        }

        #endregion

        #region IDataContext Members

        public string ConnectionString { get; private set; }

        public void Detach(params object[] enities)
        {
            if (!ObjectTrackingEnabled || !Advanced.HasSession)
                return;

            try
            {
                foreach (var entity in enities)
                    Advanced.Session.Evict(entity);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        #endregion

        #region Methods

        public void SubmitChanges()
        {
            if (Advanced.HasSession)
                Advanced.Session.Flush();
            else if (!ObjectTrackingEnabled)
                throw new InvalidOperationException("Can not SubmitChanges when ObjectTrackingEnabled is false.");
        }

        public bool ObjectTrackingEnabled
        {
            get { return _objectTrackingEnabled; }
            set
            {
                if (Advanced.HasDefaultSession)
                    throw new InvalidOperationException("Can not change ObjectTrackingEnabled after a session has been instantiated.");

                _objectTrackingEnabled = value;
            }
        }

        public ITransaction BeginTransaction()
        {
            return Advanced.DefaultSession.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (Advanced.HasDefaultSession)
                Advanced.DefaultSession.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            if (Advanced.HasDefaultSession)
                Advanced.DefaultSession.RollbackTransaction();
        }

        #endregion

        #region Properties

        public bool HasOpenTransaction
        {
            get { return Advanced.HasDefaultSession && Advanced.DefaultSession.HasOpenTransaction; }
        }

        public bool IsOpen
        {
            get { return Advanced.HasDefaultSession && Advanced.DefaultSession.IsOpen; }
        }

        public IDataContextAdvanced Advanced { get; private set; }

        #endregion

        public class DataContextAdvanced : IDataContextAdvanced
        {
            #region Declarations

            private readonly DataContext _dataContext;

            private bool _isDisposed = false;

            private IStateSession<ISession> _stateSession = null;

            private IStateSession<IStatelessSession> _statelessStateSession = null;

            #endregion

            #region Constructor & Destructor

            public DataContextAdvanced(DataContext dataContext)
            {
                _dataContext = dataContext;
            }

            ~DataContextAdvanced()
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
                {
                    SessionMap.Remove(_stateSession.Session);
                    _stateSession.Dispose();
                }

                if (HasStatelessSession)
                {
                    StatelessSessionMap.Remove(_statelessStateSession.Session);
                    _statelessStateSession.Dispose();
                }

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
                        SessionMap.Add(session, _dataContext);
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
                        StatelessSessionMap.Add(session, _dataContext);
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
