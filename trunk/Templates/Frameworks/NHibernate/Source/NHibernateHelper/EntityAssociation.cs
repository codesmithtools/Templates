using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class EntityAssociation : EntityBase
    {
        public EntityAssociation(AssociationTypeEnum associationType, TableSchema table, ColumnSchema column)
            : base(column)
        {
            ToManyTableKeyName = (associationType == AssociationTypeEnum.ManyToMany)
                ? NHibernateHelper.GetToManyTableKey(column.Table, table).Name
                : String.Empty;
            Cascade = (associationType == AssociationTypeEnum.OneToMany)
                ? NHibernateHelper.GetCascade(column)
                : String.Empty;
            ClassName = NHibernateHelper.GetClassName(table);
            AssociationType = associationType;
            GenericName = NHibernateHelper.GetGenericName(table, Column, AssociationType);
        }

        public AssociationTypeEnum AssociationType { get; private set; }
        public string ClassName { get; private set; }
        public string ToManyTableKeyName { get; private set; }
        public string Cascade { get; private set; }
        public string TableName
        {
            get { return Column.Table.FullName; }
        }
    }
}
