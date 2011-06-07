using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace CodeSmith.Data.NHibernate
{
    public class View<T> : IView<T>
        where T : class
    {
        public View(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public DataContext DataContext { get; private set; }

        #region IQueryable Methods

        private IQueryable<T> GetQueryable()
        {
            return DataContext.Advanced.StatelessSession.Query<T>();
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
            get { return GetQueryable().Provider; ; }
        }

        #endregion
    }
}