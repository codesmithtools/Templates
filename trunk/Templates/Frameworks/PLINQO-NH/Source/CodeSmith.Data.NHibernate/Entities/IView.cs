using System.Linq;

namespace CodeSmith.Data.NHibernate
{
    public interface IView<T> : IQueryable<T>
        where T : class
    {
        NHibernateDataContext DataContext { get; }
    }
}