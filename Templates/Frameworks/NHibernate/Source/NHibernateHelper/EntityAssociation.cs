using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;
using CodeSmith.Engine;

namespace NHibernateHelper
{
    public class EntityAssociation : EntityBase
    {
        public EntityAssociation(AssociationTypeEnum associationType, TableSchema table, ColumnSchema column)
            : base(column)
        {
            ToManyTableKeyName = (associationType == AssociationTypeEnum.ManyToMany)
                ? GetToManyTableKey(column.Table, table).Name
                : String.Empty;
            Cascade = NHibernateHelper.GetCascade(column, associationType);
            ClassName = NHibernateHelper.GetClassName(table);
            AssociationType = associationType;
            GenericName = GetGenericName(table, Column, AssociationType);
        }

        private string GetGenericName(TableSchema table, ColumnSchema column, AssociationTypeEnum associationType)
        {
            return GetGenericName(table, column, (associationType != AssociationTypeEnum.ManyToOne));
        }
        private string GetGenericName(TableSchema table, ColumnSchema column, bool plural)
        {
            if (ColumnHasAlias(column))
                return GetNameFromColumn(column);

            string genericName = GetAssociationName(table, column, plural);
            return NHibernateHelper.ValidateName(column, genericName);
        }
        private string GetAssociationName(TableSchema table, ColumnSchema column, bool plural)
        {
            string result = (NHibernateHelper.AssociationNaming == AssociationNamingEnum.Column)
                ? GetNameFromColumn(column)
                : NHibernateHelper.GetClassName(table);

            if (plural)
            {
                if (NHibernateHelper.AssociationSuffix == AssociationSuffixEnum.List)
                    result = String.Concat(result, NHibernateHelper.ListSuffix);
                else if (NHibernateHelper.AssociationSuffix == AssociationSuffixEnum.Plural)
                    result = StringUtil.ToPlural(result);
            }

            return result;
        }

        private MemberColumnSchema GetToManyTableKey(TableSchema manyToTable, TableSchema foreignTable)
        {
            MemberColumnSchema result = null;
            foreach (TableKeySchema key in manyToTable.ForeignKeys)
                if (key.PrimaryKeyTable.Equals(foreignTable))
                {
                    result = key.ForeignKeyMemberColumns[0];
                    break;
                }
            return result;
        }
        
        public AssociationTypeEnum AssociationType { get; private set; }
        public string ClassName { get; private set; }

        public string ToManyTableKeyName { get; private set; }
        public string ToManyTableKeySafeName
        {
            get { return NHibernateHelper.SafeName(ToManyTableKeyName); }
        }

        public string Cascade { get; private set; }
        public bool HasCascade
        {
            get { return !String.IsNullOrEmpty(Cascade); }
        }
        public string TableName
        {
            get { return Column.Table.FullName; }
        }
        public string TableSafeName
        {
            get { return NHibernateHelper.TableSafeName(Column.Table); }
        }
    }
}
