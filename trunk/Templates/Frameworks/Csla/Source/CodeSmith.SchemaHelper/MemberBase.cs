using System;
using System.Linq;
using CodeSmith.SchemaHelper.Util;

using SchemaExplorer;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    public abstract class MemberBase
    {
        #region Public Constructor(s)

        protected MemberBase(ColumnSchema column, Entity entity)
        {
            Name = column.GetName();
            ColumnName = column.Name; 
            TableName = column.Table.Name;
            TableOwner = column.Table.Owner;

            Size = column.Size;
            SystemType = column.ResolveSystemType();
            IsUnique = column.IsUnique;
            IsNullable = column.AllowDBNull;
            IsPrimaryKey = column.IsPrimaryKeyMember;
            IsIdentity = column.IsIdentity();
            IsComputed = column.IsComputed();
            IsRowVersion = column.IsRowVersion();
            
            Entity = entity ?? new Entity(column.Table);
        }

        #endregion

        #region Internal Method(s)

        internal void AppendNameSuffix(int suffix)
        {
            Name = String.Concat(Name, suffix);
        }

        #endregion

        #region Public Overridden Method(s)

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Public Read-Only Properties

        public bool IsRowVersion { get; private set; }

        public string Name { get; protected set; }

        public string SystemType { get; private set; }

        public string BaseSystemType
        {
            get
            {
                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    return SystemType.Replace("System.Nullable(Of ", string.Empty).Replace(")", string.Empty);

                return SystemType.Replace("?", string.Empty);
            }
        }

        public string PropertyName
        {
            get { return NamingConventions.PropertyName(Name); }
        }

        public string PrivateMemberVariableName
        {
            get { return NamingConventions.PrivateMemberVariableName(Name); }
        }

        public string VariableName
        {
            get { return NamingConventions.VariableName(Name); }
        }

        public Entity Entity { get; private set; }
        
        public string TableName { get; private set; }
        public string TableOwner { get; private set; }
        public string ColumnName { get; private set; }
        
        public int Size { get; private set; }

        public bool IsPrimaryKey { get; private set; }
        public bool IsUnique { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsIdentity { get; private set; }
        public bool IsComputed { get; private set; }

        public List<SearchCriteria> SearchCriteria
        {
            get
            {
                return Entity.SearchCriteria
                    .Where(sc => sc.MethodName.Contains(Name))
                    .ToList();
            }
        }

        public List<SearchCriteria> ListSearchCriteria
        {
            get
            {
                return Entity.SearchCriteria
                    .Where(sc => sc.MethodName.EndsWith(Name) && !sc.IsUniqueResult)
                    .ToList();
            }
        }

        #endregion    
    }
}