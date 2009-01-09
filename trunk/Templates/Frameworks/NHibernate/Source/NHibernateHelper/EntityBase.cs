using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public abstract class EntityBase
    {
        private string _genericName;

        public EntityBase(ColumnSchema column)
        {
            Column = column;
        }

        internal void AppendNameSuffix(int suffix)
        {
            GenericName = String.Concat(GenericName, suffix);
        }

        public string GenericName
        {
            get { return _genericName; }
            protected set
            {
                _genericName = value;
                PropertyName = NHibernateHelper.GetPropertyName(value);
                PrivateVariableName = NHibernateHelper.GetPrivateVariableName(value);
                VariableName = NHibernateHelper.GetVariableName(value);
            }
        }
        public string PropertyName { get; private set; }
        public string PrivateVariableName { get; private set; }
        public string VariableName { get; private set; }

        public ColumnSchema Column { get; private set; }
        public string ColumnName
        {
            get { return Column.Name; }
        }
        public bool IsPrimaryKeyMember
        {
            get { return Column.IsPrimaryKeyMember; }
        }

    }
}
