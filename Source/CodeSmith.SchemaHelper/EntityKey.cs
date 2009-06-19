using System;
using System.Collections.Generic;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class EntityKey
    {
        #region Constructor(s)

        // TODO: Make this private; do construction in static get method and return null if no columns.
        public EntityKey(TableSchema sourceTable)
            : this(sourceTable, null) { }
        public EntityKey(TableSchema sourceTable, Entity entity)
        {
            if (!sourceTable.HasPrimaryKey)
                throw new Exception("Table must have a primary key.");

            KeyMembers = new List<Member>();
            bool isAllIdentityColumns = true;
            foreach (MemberColumnSchema mcs in sourceTable.PrimaryKey.MemberColumns)
            {
                if (!mcs.IsIdentity())
                    isAllIdentityColumns = false;
                KeyMembers.Add(new Member(mcs, entity));
            }

            IsIdentity = (!IsCompositeKey && isAllIdentityColumns);
        }

        #endregion

        #region Public Read-Only Properties

        public bool IsCompositeKey
        {
            get { return (KeyMembers.Count > 1); }
        }

        public Member KeyMember
        {
            get
            {
                if (KeyMembers.Count == 0)
                    return null;
                
                //return (IsCompositeKey || KeyMembers.Count == 0) ? null : KeyMembers[0];
                return KeyMembers[0];
            }
        }

        public List<Member> KeyMembers { get; private set; }
        public bool IsIdentity { get; private set; }

        #endregion
    }
}