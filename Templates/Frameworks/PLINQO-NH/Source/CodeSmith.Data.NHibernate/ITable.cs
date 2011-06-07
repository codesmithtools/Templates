using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.Data.NHibernate
{
    public interface ITable<T> : IQueryable<T>
        where T : class
    {
        IDataContext DataContext { get; }

        void InsertOnSubmit(T entity);

        void InsertAllOnSubmit(IEnumerable<T> entities);

        void DeleteOnSubmit(T entity);

        void DeleteAllOnSubmit(IEnumerable<T> entities);

        void Attach(T entity);

        void AttachAll(IEnumerable<T> entities);

        void Detach(T entity);

        void DetachAll(IEnumerable<T> entities);
    }
}
