using System;
using NHibernate;

namespace CodeSmith.Data.NHibernate
{
    public interface INHibernateDataContext : IDataContext
    {
        bool IsOpen { get; }

        INHibernateDataContextAdvanced Advanced { get; }
    }
}
