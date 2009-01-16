using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class EntityKey
    {
        private const string Identity = "CS_IsIdentity";

        public EntityKey(TableSchema sourceTable)
        {
            if(!sourceTable.HasPrimaryKey)
                throw new Exception("NHibernate requires all tables have a primary key.");

            KeyColumns = new List<EntityMember>();
            foreach (MemberColumnSchema mcs in sourceTable.PrimaryKey.MemberColumns)
                KeyColumns.Add(new EntityMember(mcs));

            if (!IsCompositeKey)
            {
                IsIdentity = (KeyColumn.Column.ExtendedProperties.Contains(Identity)
                    && ((bool)KeyColumn.Column.ExtendedProperties[Identity].Value) == true);

                Generator = (IsIdentity)
                    ? "<generator class=\"native\" />"
                    : "<generator class=\"assigned\" />";
            }
            else
            {
                IsIdentity = false;
                Generator = String.Empty;
            }
        }

        public bool IsCompositeKey
        {
            get { return ( KeyColumns.Count > 1); }
        }
        public EntityMember KeyColumn
        {
            get
            {
                return (IsCompositeKey || KeyColumns.Count == 0)
                    ? null
                    : KeyColumns[0];
            }
        }

        public List<EntityMember> KeyColumns { get; private set; }
        public string Generator { get; private set; }
        public bool IsIdentity { get; private set; }
    }
}
