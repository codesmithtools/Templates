using System;
using System.Collections.Generic;
using System.Linq;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class AssociationMember : Member
    {
        #region Constructor(s)

        public AssociationMember(AssociationType associationType, TableSchema table, ColumnSchema column, ColumnSchema localColumn, Entity entity) : base(column, entity)
        {
            ToManyTableKeyName = (associationType == SchemaHelper.AssociationType.ManyToMany)
                                     ? GetToManyTableKey(column.Table, table).Name
                                     : String.Empty;

            AssociatedColumn = new Member(localColumn, new Entity(localColumn.Table));
            Cascade = (associationType == SchemaHelper.AssociationType.OneToMany && !column.AllowDBNull);
            ClassName = table.ClassName();
            AssociationType = associationType;
            Name = column.GetName(table, associationType);
            GenericProperty = table.ResolveIsGenericExtendedProperty();
            Table = table;
        }

        #endregion

        #region Private Method(s)

        private static MemberColumnSchema GetToManyTableKey(TableSchema manyToTable, TableSchema foreignTable)
        {
            foreach (TableKeySchema key in manyToTable.ForeignKeys)
                if (key.PrimaryKeyTable.Equals(foreignTable))
                    return key.ForeignKeyMemberColumns[0];

            return null;
        }

        #endregion

        #region Public Read-Only Methods

        public Member AssociatedColumn { get; private set; }
        internal TableSchema Table { get; private set; }

        public AssociationType AssociationType { get; private set; }
        public string ClassName { get; private set; }
        public string ToManyTableKeyName { get; private set; }
        public bool Cascade { get; private set; }

        /// <summary>
        /// Returns the unique key for this AssociationMember
        /// </summary>
        public string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", TableName, ColumnName, AssociatedColumn.ColumnName);
            }
        }

        /// <summary>
        /// Returns a generic parameter if the table has an extended property named CS_IsGeneric and the value of CS_IsGeneric is True.
        /// </summary>
        public string GenericProperty { get; private set; }

        #endregion
    }
}