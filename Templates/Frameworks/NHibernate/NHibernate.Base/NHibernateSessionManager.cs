using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;
using NHibernate.Cfg;

namespace NHibernate.Base
{
    /// <summary>
    /// A Singleton that creates and persits a single SessionFactory for the to program to access globally.
    /// This uses the .Net CallContext to store a session for each thread.
    /// 
    /// This is heavely based on 'NHibernate Best Practices with ASP.NET' By Billy McCafferty.
    /// http://www.codeproject.com/KB/architecture/NHibernateBestPractices.aspx
    /// </summary>
    public sealed class NHibernateSessionManager : IDisposable
    {
        #region Static Content

        private static NHibernateSessionManager _nHibernateSessionManager = new NHibernateSessionManager();
        public static NHibernateSessionManager Instance
        {
            get { return _nHibernateSessionManager; }
        }

        #endregion

        #region Declarations

        private ISessionFactory _sessionFactory = null;

        private const string _sessionContextKey = "CsNHibernate-SessionContextKey";
        private const string _transactionContextKey = "CsNHibernate-TransactionContextKey";

        #endregion

        #region Constructors & Finalizers

        /// <summary>
        /// This will load the NHibernate settings from the App.config.
        /// Note: This can/should be expanded to support multiple databases.
        /// </summary>
        private NHibernateSessionManager()
        {
            _sessionFactory = new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory();
        }
        ~NHibernateSessionManager()
        {
            Dispose(true);
        }

        #endregion

        #region IDisposable

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(false);
        }
        private void Dispose(bool finalizing)
        {
            if (!_isDisposed)
            {
                // Close SessionFactory
                _sessionFactory.Close();
                _sessionFactory.Dispose();

                // Flag as disposed.
                _isDisposed = true;
                if (!finalizing)
                    GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Methods

        public ISession GetSession()
        {
            ISession session = ContextSession;

            // If the thread does not yet have a session, create one.
            if (session == null)
            {
                lock (_sessionFactory)
                {
                    session = _sessionFactory.OpenSession();
                }

                // Save to CallContext.
                ContextSession = session;
            }

            return session;
        }
        public void CloseSession()
        {
            ISession session = ContextSession;

            CloseSession(session);

            // Remove from CallContext.
            ContextSession = null;
        }
        private void CloseSession(ISession session)
        {
            if (session != null && session.IsOpen)
            {
                session.Flush();
                session.Close();
            }
        }

        public bool BeginTransaction()
        {
            bool noPreviousTransaction = (ContextTransaction == null);

            if (noPreviousTransaction)
                ContextTransaction = GetSession().BeginTransaction();

            return noPreviousTransaction;
            
        }
        public bool CommitTransaction()
        {
            ITransaction transaction = ContextTransaction;
            bool hasOpenTransaction = IsOpenTransaction(transaction);

            if (hasOpenTransaction)
            {
                try
                {
                    transaction.Commit();
                    ContextTransaction = null;
                }
                catch(HibernateException)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return hasOpenTransaction;
        }
        public bool RollbackTransaction()
        {
            ITransaction transaction = ContextTransaction;
            bool hasOpenTransaction = IsOpenTransaction(transaction);

            if (hasOpenTransaction)
            {
                transaction.Rollback();
                ContextTransaction = null;
            }

            return hasOpenTransaction;
        }
        private bool IsOpenTransaction(ITransaction transaction)
        {
            return (transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack);

        }

        private void SetContextObject(string key, object value)
        {
            if (IsWebContext)
                System.Web.HttpContext.Current.Items[key] = value;
            else
                System.Runtime.Remoting.Messaging.CallContext.SetData(key, value);
        }
        private object GetContextOject(string key)
        {
            if (IsWebContext)
                return System.Web.HttpContext.Current.Items[key];
            else
                return System.Runtime.Remoting.Messaging.CallContext.GetData(key);
        }

        #endregion

        #region Properties

        private ISession ContextSession
        {
            get { return (ISession)GetContextOject(_sessionContextKey); }
            set { SetContextObject(_sessionContextKey, value); }
        }
        private ITransaction ContextTransaction
        {
            get { return (ITransaction)GetContextOject(_transactionContextKey); }
            set { SetContextObject(_transactionContextKey, value); }
        }
        private bool IsWebContext
        {
            get { return (System.Web.HttpContext.Current != null); }
        }

        public bool HasOpenTransaction
        {
            get { return IsOpenTransaction(ContextTransaction); }
        }

        #endregion
    }
}
