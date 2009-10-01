using System;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class Member : MemberBase
    {
        #region Constructor(s)

        public Member(ColumnSchema column) : this(column, null)
        {
            IsForeignKey = false;
        }
        
        public Member(ColumnSchema column, Entity entity) : base(column, entity)
        {
            IsForeignKey = false;
        }

        public Member(ColumnSchema column, Entity entity, bool isForeignKey) : base(column, entity)
        {
            IsForeignKey = isForeignKey;
        }

        #endregion

        #region Public Read-Only Variables

        public bool IsForeignKey { get; private set; }

        public bool IsReadOnly
        {
            get { return (IsIdentity || IsRowVersion || IsComputed); }
        }

        #endregion
    }
}