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

            LocalColumn = new Member(localColumn, new Entity(localColumn.Table));
            Cascade = (associationType == SchemaHelper.AssociationType.OneToMany && !column.AllowDBNull);
            ClassName = table.ClassName();
            AssociationType = associationType;
            Name = column.GetName(table, associationType);
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

        public Member LocalColumn { get; private set; }
        internal TableSchema Table { get; private set; }

        public AssociationType AssociationType { get; private set; }
        public string ClassName { get; private set; }
        public string ToManyTableKeyName { get; private set; }
        public bool Cascade { get; private set; }

        public new List<SearchCriteria> ListSearchCriteria
        {
            get
            {
                return this.AssociationEntity().SearchCriteria
                    .Where(sc =>
                        sc.MethodName.EndsWith(LocalColumn.Name) ||
                        sc.MethodName.EndsWith(LocalColumn.ColumnName) ||
                        sc.MethodName.EndsWith(Name) ||
                        sc.MethodName.EndsWith(ColumnName) && 
                        !sc.IsUniqueResult)
                    .ToList();
            }
        }

        #endregion
    }
}