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
            SystemType = NHibernateHelper.GetSystemTypeName(column);
            IsVersion = NHibernateHelper.VersionRegex.IsMatch(column.Name);
        }

        public string SystemType { get; private set; }
        public bool IsVersion { get; private set; }
    }
}
