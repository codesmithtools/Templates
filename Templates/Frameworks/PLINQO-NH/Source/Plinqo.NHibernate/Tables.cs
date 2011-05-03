using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Plinqo.NHibernate
{
    public class Table<T> : ITable<T>
        where T : class
    {
        public Table(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public DataContext DataContext { get; private set; }

        #region IQueryable Methods

        private IQueryable<T> GetQueryable()
        {
            return DataContext.Session.Query<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetQueryable().GetEnumerator();
        }

        public Type ElementType
        {
            get { return GetQueryable().ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return GetQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return GetQueryable().Provider;; }
        }

        #endregion

        #region Table Methods

        public void InsertOnSubmit(T entity)
        {
            DataContext.Session.Save(entity);
        }

        public void InsertAllOnSubmit(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                InsertOnSubmit(entity);
        }

        public void DeleteOnSubmit(T entity)
        {
            DataContext.Session.Delete(entity);
        }

        public void DeleteAllOnSubmit(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                DeleteOnSubmit(entity);
        }

        public void Attach(T entity)
        {
            DataContext.Session.Update(entity);
        }

        public void AttachAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Attach(entity);
        }

        public void InsertOrUpdateOnSubmit(T entity)
        {
            DataContext.Session.SaveOrUpdate(entity);
        }

        public void InsertOrUpdateAllOnSubmit(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                InsertOrUpdateOnSubmit(entity);
        }

        public void Detach(T entity)
        {
            DataContext.Session.Evict(entity);
        }

        public void DetachAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Detach(entity);
        }

        #endregion
    }
}
