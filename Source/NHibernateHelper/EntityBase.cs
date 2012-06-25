using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;
using CodeSmith.Engine;

namespace NHibernateHelper
{
    public abstract class EntityBase
    {
        private string _genericName;

        public EntityBase(ColumnSchema column)
        {
            Column = column;
        }

        private static string GetPropertyName(string name)
        {
            return StringUtil.ToPascalCase(name);
        }
        private static string GetPrivateVariableName(string name)
        {
            return String.Concat("_", GetVariableName(name));
        }
        private static string GetVariableName(string name)
        {
            return StringUtil.ToCamelCase(name);
        }

        protected static bool ColumnHasAlias(ColumnSchema column)
        {
            return column.ExtendedProperties.Contains(NHibernateHelper.ExtendedPropertyName);
        }
        protected static string GetNameFromColumn(ColumnSchema column)
        {
            string name = (ColumnHasAlias(column))
                ? column.ExtendedProperties[NHibernateHelper.ExtendedPropertyName].Value.ToString()
                : column.Name;

            return NHibernateHelper.ValidateName(name);
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
                PropertyName = GetPropertyName(value);
                PrivateVariableName = GetPrivateVariableName(value);
                VariableName = GetVariableName(value);
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
        public string ColumnSafeName
        {
            get { return NHibernateHelper.ColumnSafeName(Column); }
        }

        public bool IsPrimaryKeyMember
        {
            get { return Column.IsPrimaryKeyMember; }
        }
    }
}
