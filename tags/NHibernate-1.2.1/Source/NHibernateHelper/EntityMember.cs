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
            GenericName = GetGenericName(column);
            SystemType = GetSystemTypeName(column);
            IsRowVersion = (!column.IsPrimaryKeyMember && NHibernateHelper.VersionRegex.IsMatch(column.Name));
        }

        private static string GetGenericName(ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetNameFromColumn(column);

            string genericName = GetNameFromColumn(column);
            return NHibernateHelper.ValidateName(column, genericName);
        }
        private static string GetSystemTypeName(ColumnSchema column)
        {
            string result = (NHibernateHelper.SystemCSharpAliasMap.ContainsKey(column.SystemType.FullName))
                ? NHibernateHelper.SystemCSharpAliasMap[column.SystemType.FullName]
                : column.SystemType.FullName;

            return (column.AllowDBNull && column.SystemType.IsValueType)
                ? String.Concat(result, "?")
                : result;
        }

        public string SystemType { get; private set; }
        public bool IsRowVersion { get; private set; }
    }
}
