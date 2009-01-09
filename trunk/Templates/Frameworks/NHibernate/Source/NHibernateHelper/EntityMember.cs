using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class EntityMember : EntityBase
    {
        public EntityMember(ColumnSchema column)
            : base(column)
        {
            GenericName = NHibernateHelper.GetGenericName(column);

            SystemType = column.SystemType;
        }

        public Type SystemType { get; private set; }
    }
}
