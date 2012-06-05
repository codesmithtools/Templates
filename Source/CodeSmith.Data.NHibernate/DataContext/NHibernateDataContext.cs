using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using CodeSmith.Data.NHibernate2;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace CodeSmith.Data.NHibernate
{
    public abstract class NHibernateDataContext : INHibernateDataContext, IFutureContext
    {
        #region Session Repository

        private static Dictionary<ISession, NHibernateDataContext> _sessionMap;

        private static Dictionary<ISession, NHibernateDataContext> SessionMap
        {
            get
            {
                if (_sessionMap == null)
                    _sessionMap = new Dictionary<ISession, NHibernateDataContext>();

                return _sessionMap;
            }
        }

        private static Dictionary<IStatelessSession, NHibernateDataContext> _statelessSessionMap;

        private static Dictionary<IStatelessSession, NHibernateDataContext> StatelessSessionMap
        {
            get
            {
                if (_statelessSessionMap == null)
                    _statelessSessionMap = new Dictionary<IStatelessSession, NHibernateDataContext>();

                return _statelessSessionMap;
            }
        }

        internal static NHibernateDataContext GetBySession(object session)
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
            return names.Any(name => connection.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()));
        }

        protected ISessionFactory CreateSessionFactory(string databaseName, string assemblyName, string dialect, string connectionDriver)
        {
            _connectionString = GetConnectionString(databaseName);

            var config = new Configuration();
            ConfigureSessionFactory(config, databaseName, assemblyName, dialect, connectionDriver, _connectionString);
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

        private string _connectionString;

        #endregion

        #region Constructor & Destructor

        static NHibernateDataContext()
        {
            var provider = new NHibernateDataContextProvider();
            DataContextProvider.Register(provider);
        }

        protected NHibernateDataContext()
        {
            Advanced = new NHibernateDataContextAdvanced(this);
        }

        ~NHibernateDataContext()
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

        #region IDataContext

        string IDataContext.ConnectionString
        {
            get { return _connectionString; }
        }

        void IDataContext.Detach(params object[] enities)
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

        IDisposable IDataContext.BeginTransaction()
        {
            return BeginTransaction();
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

        public bool HasOpenTransaction
        {
            get { return Advanced.HasDefaultSession && Advanced.DefaultSession.HasOpenTransaction; }
        }

        #endregion

        #region IFutureContext

        void IFutureContext.ExecuteFutureQueries()
        {
            Advanced.ExecuteFutureQueries();
        }

        IEnumerable<T> IFutureContext.Future<T>(IQueryable<T> query, CacheSettings cacheSettings)
        {
            var action = new Action(Advanced.ExecuteFutureQueries);
            var future = new NHibernateFutureQuery<T>(query, cacheSettings, action);
            Advanced.FutureQueries.Add(future);
            return future;
        }

        Future.IFutureValue<int> IFutureContext.FutureCount<T>(IQueryable<T> query, CacheSettings cacheSettings)
        {
            throw new NotImplementedException();
        }

        Future.IFutureValue<T> IFutureContext.FutureFirstOrDefault<T>(IQueryable<T> query, CacheSettings cacheSettings)
        {
            var action = new Action(Advanced.ExecuteFutureQueries);
            var future = new NHibernateFutureValue<T>(query, cacheSettings, action);
            Advanced.FutureQueries.Add(future);
            return future;
        }

        IEnumerable<IFutureQuery> IFutureContext.FutureQueries
        {
            get { return Advanced.FutureQueries.Cast<IFutureQuery>(); }
        }

        #endregion

        #region INHibernateDataContext

        public bool IsOpen
        {
            get { return Advanced.HasDefaultSession && Advanced.DefaultSession.IsOpen; }
        }

        public INHibernateDataContextAdvanced Advanced { get; private set; }

        #endregion

        public class NHibernateDataContextAdvanced : INHibernateDataContextAdvanced
        {
            #region Declarations

            private readonly NHibernateDataContext _dataContext;

            private bool _isDisposed = false;

            private IStateSession<ISession> _stateSession = null;

            private IStateSession<IStatelessSession> _statelessStateSession = null;

            #endregion

            #region Constructor & Destructor

            public NHibernateDataContextAdvanced(NHibernateDataContext dataContext)
            {
                _dataContext = dataContext;
                FutureQueries = new List<INHibernateFutureQuery>();
            }

            ~NHibernateDataContextAdvanced()
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

            #region Future

            public void ExecuteFutureQueries()
            {
                if (FutureQueries.Count == 0)
                    return;

                FutureQueries
                    .FirstOrDefault()
                    .Load();

                foreach (var future in FutureQueries)
                    future.IsLoaded = true;

                FutureQueries.Clear();
            }

            #endregion

            #region Properties

            public IList<INHibernateFutureQuery> FutureQueries { get; private set; }

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

        