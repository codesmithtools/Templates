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
        public Table(IDataContext dataContext)
        {
            DataContext = dataContext;
        }

        public IDataContext DataContext { get; private set; }

        #region IQueryable Methods

        private IQueryable<T> GetQueryable()
        {
            return DataContext.ObjectTrackingEnabled
                ? DataContext.StatefulSession.Session.Query<T>()
                : DataContext.StatelessSession.Session.Query<T>();
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
            get { return GetQueryable().Provider; }
        }

        #endregion

        #region Table Methods

        public void InsertOnSubmit(T entity)
        {
            DataContext.GetDefaultStateSession()
                .Save(entity);
        }

        public void InsertAllOnSubmit(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                InsertOnSubmit(entity);
        }

        public void DeleteOnSubmit(T entity)
        {
            DataContext.GetDefaultStateSession()
                .Delete(entity);
        }

        public void DeleteAllOnSubmit(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                DeleteOnSubmit(entity);
        }

        public void Attach(T entity)
        {
            if (!DataContext.ObjectTrackingEnabled)
                throw new Exception("Can not attach to DataContext when ObjectTrackingEnabled is false.");

            DataContext.StatefulSession.Session
                .Update(entity);
        }

        public void AttachAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Attach(entity);
        }

        public void Detach(T entity)
        {
            if (DataContext.ObjectTrackingEnabled)
                DataContext.StatefulSession.Session
                    .Evict(entity);
        }

        public void DetachAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Detach(entity);
        }

        #endregion
    }
}
