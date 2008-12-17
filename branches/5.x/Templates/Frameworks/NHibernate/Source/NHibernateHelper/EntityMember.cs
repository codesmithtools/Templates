using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class EntityMember
    {
        public EntityMember(ColumnSchema column)
        {
            Column = column;

            PropertyName = NHibernateHelper.GetPropertyName(column);
            VariableName = NHibernateHelper.GetVariableName(column);
            PrivateVariableName = NHibernateHelper.GetPrivateVariableName(column);
            ColumnName = column.Name;
            SystemType = column.SystemType;
        }

        public string PropertyName { get; private set; }
        public string VariableName { get; private set; }
        public string PrivateVariableName { get; private set; }
        public string ColumnName { get; private set; }
        public Type SystemType { get; private set; }
        public ColumnSchema Column { get; private set; }
    }
}
