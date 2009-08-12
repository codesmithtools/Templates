using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace Sample.Data.Generated.Base
{
    public interface INHibernateSession : IDisposable
    {
        // Methods
        void CommitChanges();
        void Close();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void IncrementRefCount();
        void DecrementRefCount();
        ISession GetISession();
        
        // Properties
        bool HasOpenTransaction { get; }
        bool IsOpen { get; }
    }

    public class NHibernateSession : INHibernateSession
    {
        #region Declarations

        protected ITransaction transaction = null;
        protected ISession iSession = null;
        
        private bool _isDisposed = false;
        private int _refCount = 0;

        #endregion

        #region Constructor & Destructor

        public NHibernateSession() { }
        ~NHibernateSession()
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
            if (!_isDisposed)
            {
                Close();

                if (!finalizing)
                    GC.SuppressFinalize(this);
                    
                _isDisposed = true;
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
            if (iSession == null)
                return;

            if (HasOpenTransaction)
                RollbackTransaction();

            if (iSession.IsOpen)
                iSession.Close();
            iSession.Dispose();
            iSession = null;
        }

        public void BeginTransaction()
        {
            transaction = GetISession().BeginTransaction();
        }
        public void CommitTransaction()
        {
            if (transaction == null)
                return;

            try
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
        }
        public void RollbackTransaction()
        {
            if (transaction == null)
                return;

            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
        }

        public void IncrementRefCount()
        {
            _refCount++;
        }
        public void DecrementRefCount()
        {
            _refCount--;
            if (_refCount == 0)
                Close();
        }

        public ISession GetISession()
        {
            if (iSession == null)
                iSession = NHibernateSessionManager.Instance.CreateISession();
            return iSession;
        }

        #endregion

        #region Properties

        public bool HasOpenTransaction
        {
            get { return (transaction != null); }
        }
        public bool IsOpen
        {
            get { return (iSession != null && iSession.IsOpen); }
        }

        #endregion
    }
}
