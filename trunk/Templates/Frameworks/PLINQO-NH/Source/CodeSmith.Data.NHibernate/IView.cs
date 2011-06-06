using System.Linq;

namespace Plinqo.NHibernate
{
    public interface IView<T> : IQueryable<T>
        where T : class
    {
        DataContext DataContext { get; }
    }
}