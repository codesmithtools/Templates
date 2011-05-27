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

        #endregion

        #region Declarations

        private bool _isDisposed = false;

        protected ITransaction Transaction;

        #endregion

        #region Constructors & Destructors

        protected DataContext()
        {
            Session = CreateSession();
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

            if (HasOpenTransaction)
                RollbackTransaction();

            if (Session != null)
            {
                if (Session.IsOpen)
                    Session.Close();

                Session.Dispose();
            }

            if (!finalizing)
                GC.SuppressFinalize(this);

            _isDisposed = true;
        }

        #endregion

        #region Public Methods

        public void SubmitChanges()
        {
            Session.Flush();
        }

        public void Refresh(object entity)
        {
            Session.Refresh(entity);
        }

        public void RefreshAll(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
                Refresh(entity);
        }

        public ITransaction BeginTransaction()
        {
            if (Transaction == null)
                Transaction = Session.BeginTransaction();

            return Transaction;
        }

        public void CommitTransaction()
        {
            if (Transaction == null)
                return;

            try
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
        }

        public void RollbackTransaction()
        {
            if (Transaction == null)
                return;

            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
        }

        #endregion

        #region Properties

        public ISession Session { get; private set; }

        public bool HasOpenTransaction
        {
            get { return (Transaction != null); }
        }

        public bool IsOpen
        {
            get { return (Session != null && Session.IsOpen); }
        }

        #endregion
    }
}
