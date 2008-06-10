using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;
using NHibernate.Criterion;

namespace NHibernate.Base
{
    public interface IManagerBase<T, IdT>
    {
        // Get Methods
        T GetById(IdT Id);
        IList<T> GetAll();
        IList<T> GetByCriteria(params ICriterion[] criterionList);
        IList<T> GetByExample(T exampleObject, params string[] excludePropertyList);

        // CRUD Methods
        object Save(T entity);
        void SaveOrUpdate(T entity);
        void Delete(T entity);
        void Update(T entity);
        void Refresh(T entity);

        // Transaction Methods
        bool BeginTransaction();
        bool CommitTransaction();
        bool RollbackTransaction();

        // Commit/Flush
        void CommitChanges();

        // Properties
        System.Type Type { get; }
    }

    public abstract class ManagerBase<T, IdT> : IManagerBase<T, IdT>
    {
        #region Get Methods

        public virtual T GetById(IdT id)
        {
            return (T)Session.Get(typeof(T), id);
        }
        public IList<T> GetAll()
        {
            return GetByCriteria();
        }
        public IList<T> GetByCriteria(params ICriterion[] criterionList)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(T));

            foreach (ICriterion criterion in criterionList)
                criteria.Add(criterion);

            return criteria.List<T>();
        }
        public IList<T> GetByExample(T exampleObject, params string[] excludePropertyList)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(T));
            Example example = Example.Create(exampleObject);

            foreach (string excludeProperty in excludePropertyList)
                example.ExcludeProperty(excludeProperty);

            criteria.Add(example);

            return criteria.List<T>();
        }
        
        #endregion

        #region CRUD Methods

        public object Save(T entity)
        {
            return Session.Save(entity);
        }
        public void SaveOrUpdate(T entity)
        {
            Session.SaveOrUpdate(entity);
        }
        public void Delete(T entity)
        {
            Session.Delete(entity);
        }
        public void Update(T entity)
        {
            Session.Update(entity);
        }
        public void Refresh(T entity)
        {
            Session.Refresh(entity);
        }
        
        #endregion

        #region Transaction Methods

        public bool BeginTransaction()
        {
            return NHibernateSessionManager.Instance.BeginTransaction();
        }
        public bool CommitTransaction()
        {
            return NHibernateSessionManager.Instance.CommitTransaction();
        }
        public bool RollbackTransaction()
        {
            return NHibernateSessionManager.Instance.RollbackTransaction();
        }

        #endregion

        // Commit/Flush
        public void CommitChanges()
        {
            if (HasOpenTransaction)
                CommitTransaction();
            else
                Session.Flush();
        }

        #region Properties

        public System.Type Type
        {
            get { return typeof(T); }
        }
        public bool HasOpenTransaction
        {
            get { return NHibernateSessionManager.Instance.HasOpenTransaction; }
        }

        /// <summary>
        /// The NHibernate Session object is exposed only to the Manager class.
        /// It is recommended that you...
        /// ...use the the NHibernateSessionManager methods to control Transactions (unless you specifically want nested transactions).
        /// ...do not directly expose the Flush method (to prevent open transactions from locking your DB).
        /// </summary>
        protected ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        #endregion
    }
}
