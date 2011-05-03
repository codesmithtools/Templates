using System.Collections.Generic;
using System.Linq;

namespace Plinqo.NHibernate
{
    public interface ITable<T>: IQueryable<T>
        where T : class
    {
        DataContext DataContext { get; }

        void InsertOnSubmit(T entity);

        void InsertAllOnSubmit(IEnumerable<T> entities);

        void DeleteOnSubmit(T entity);

        void DeleteAllOnSubmit(IEnumerable<T> entities);

        void Attach(T entity);

        void AttachAll(IEnumerable<T> entities);

        void InsertOrUpdateOnSubmit(T entity);

        void InsertOrUpdateAllOnSubmit(IEnumerable<T> entities);

        void Detach(T entity);

        void DetachAll(IEnumerable<T> entities);
    }
}
