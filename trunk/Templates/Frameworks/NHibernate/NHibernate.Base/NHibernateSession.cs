using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Base
{
    public interface INHibernateSession : IDisposable
    {
        // Methods
        void CommitChanges();
        void Close();
        bool BeginTransaction();
        bool CommitTransaction();
        bool RollbackTransaction();

        // Properties
        bool HasOpenTransaction { get; }
        bool IsOpen { get; }
        ISession ISession { get; }
    }

    class NHibernateSession : INHibernateSession
    {
        #region Declarations

        protected ITransaction transaction = null;
        protected ISession session;

        #endregion

        #region Constructor & Destructor

        internal NHibernateSession(ISession session)
        {
            this.session = session;
        }
        ~NHibernateSession()
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
                // Close Session
                Close();

                // Flag as disposed.
                _isDisposed = true;
                if (!finalizing)
                    GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Methods

        public void CommitChanges()
        {
            if (HasOpenTransaction)
                CommitTransaction();
            else
                session.Flush();
        }

        public void Close()
        {
            if (session.IsOpen)
            {
                session.Flush();
                session.Close();
            }
        }

        public bool BeginTransaction()
        {
            bool result = !HasOpenTransaction;
            if (result)
                transaction = session.BeginTransaction();
            return result;
        }
        public bool CommitTransaction()
        {
            bool result = HasOpenTransaction;
            if (result)
            {
                try
                {
                    transaction.Commit();
                    transaction = null;
                }
                catch (HibernateException)
                {
                    transaction.Rollback();
                    transaction = null;
                    throw;
                }
            }
            return result;
        }
        public bool RollbackTransaction()
        {
            bool result = HasOpenTransaction;
            if (result)
            {
                transaction.Rollback();
                transaction = null;
            }
            return result;
        }

        #endregion

        #region Properties

        public bool HasOpenTransaction
        {
            get
            {
                return (transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack);
            }
        }
        public bool IsOpen
        {
            get { return session.IsOpen; }
        }
        public ISession ISession
        {
            get { return session; }
        }

        #endregion
    }
}
