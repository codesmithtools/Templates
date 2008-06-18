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
        ISession GetISession();

        // Properties
        bool HasOpenTransaction { get; }
        bool IsOpen { get; }
    }

    public class NHibernateSession : INHibernateSession
    {
        #region Declarations

        protected ITransaction transaction = null;
        protected ISession iSession;

        #endregion

        #region Constructor & Destructor

        public NHibernateSession(ISession session)
        {
            this.iSession = session;
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
                iSession.Flush();
        }
        public void Close()
        {
            if (iSession.IsOpen)
            {
                iSession.Flush();
                iSession.Close();
            }
        }

        public bool BeginTransaction()
        {
            bool result = !HasOpenTransaction;
            if (result)
                transaction = iSession.BeginTransaction();
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

        public ISession GetISession()
        {
            return iSession;
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
            get { return iSession.IsOpen; }
        }

        #endregion
    }
}
