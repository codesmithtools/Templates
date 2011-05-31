using System;
using NHibernate;

namespace Plinqo.NHibernate
{
    public abstract class StateSession<T> : IStateSession<T>
        where T : IDisposable
    {
        #region Abstract Members

        public abstract void Save(object o);

        public abstract void Refresh(object o);

        public abstract void Delete(object o);

        public abstract void Close();

        public abstract IQuery GetNamedQuery(string queryName);

        public abstract ITransaction BeginTransaction();

        public abstract bool IsOpen { get; }

        #endregion

        #region Destructor

        ~StateSession()
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
            if (_isDisposed)
                return;

            if (HasOpenTransaction)
                RollbackTransaction();

            if (Session != null)
            {
                if (IsOpen)
                    Close();

                Session.Dispose();
            }

            if (!finalizing)
                GC.SuppressFinalize(this);

            _isDisposed = true;
        }

        #endregion

        #region Public Methods
        
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

        public T Session { get; protected set; }

        public ITransaction Transaction { get; protected set; }

        public bool HasOpenTransaction
        {
            get { return Transaction != null; }
        }

        #endregion
    }
}