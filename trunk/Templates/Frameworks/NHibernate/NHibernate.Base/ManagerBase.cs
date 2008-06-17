using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;
using NHibernate.Expression;    // For NHibernate v1.2
//using NHibernate.Criterion;   // For NHibernate v2.0

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

        // Properties
        System.Type Type { get; }
        INHibernateSession Session { get; }
    }

    public abstract partial class ManagerBase<T, IdT> : IManagerBase<T, IdT>
    {
        #region Declarations

        protected INHibernateSession session;

        #endregion

        #region Constructors

        public ManagerBase()
        {
            session = NHibernateSessionManager.Instance.GetContextSession();
        }
        public ManagerBase(INHibernateSession session)
        {
            this.session = session;
        }

        #endregion

        #region Get Methods

        public virtual T GetById(IdT id)
        {
            return (T)Session.ISession.Get(typeof(T), id);
        }
        public IList<T> GetAll()
        {
            return GetByCriteria();
        }
        public IList<T> GetByCriteria(params ICriterion[] criterionList)
        {
            ICriteria criteria = Session.ISession.CreateCriteria(typeof(T));

            foreach (ICriterion criterion in criterionList)
                criteria.Add(criterion);

            return criteria.List<T>();
        }
        public IList<T> GetByExample(T exampleObject, params string[] excludePropertyList)
        {
            ICriteria criteria = Session.ISession.CreateCriteria(typeof(T));
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
            return Session.ISession.Save(entity);
        }
        public void SaveOrUpdate(T entity)
        {
            Session.ISession.SaveOrUpdate(entity);
        }
        public void Delete(T entity)
        {
            Session.ISession.Delete(entity);
        }
        public void Update(T entity)
        {
            Session.ISession.Update(entity);
        }
        public void Refresh(T entity)
        {
            Session.ISession.Refresh(entity);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// The NHibernate Session object is exposed only to the Manager class.
        /// It is recommended that you...
        /// ...use the the NHibernateSession methods to control Transactions (unless you specifically want nested transactions).
        /// ...do not directly expose the Flush method (to prevent open transactions from locking your DB).
        /// </summary>
        public System.Type Type
        {
            get { return typeof(T); }
        }
        public INHibernateSession Session
        {
            get { return session; }
        }

        #endregion
    }
}
