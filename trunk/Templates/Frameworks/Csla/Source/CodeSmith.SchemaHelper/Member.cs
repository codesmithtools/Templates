using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.SchemaHelper.Util;
using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class Member
    {
        #region Constructor(s)

        public Member(ColumnSchema column, Entity entity)
        {
            Name = column.GetName();
            ColumnName = column.Name;
            TableName = column.Table.Name;
            TableOwner = column.Table.Owner;

            Size = column.Size;
            SystemType = column.ResolveSystemType();
            Description = column.ResolveDescription();
            DataType = column.DataType.ToString();
            NativeType = column.NativeType;
            IsUnique = column.IsUnique;
            IsNullable = column.AllowDBNull;
            IsPrimaryKey = column.IsPrimaryKeyMember;
            IsForeignKey = column.IsForeignKeyMember;
            IsIdentity = column.IsIdentity();
            IsComputed = column.IsComputed();
            IsRowVersion = column.IsRowVersion();

            Entity = entity != null &&
                     entity.Table.FullName.Equals(column.Table.FullName, StringComparison.InvariantCultureIgnoreCase)
                         ? entity
                         : new Entity(column.Table);

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

        public bool IsForeignKey { get; private set; }

        public bool IsReadOnly
        {
            get { return (IsIdentity || IsRowVersion || IsComputed); }
        }
        
        public string Description { get; private set; }
        public bool HasDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(Description))
                    return Description.Trim().Length > 0;

                return false;
            }
        }

        public bool IsRowVersion { get; private set; }

        public string Name { get; internal set; }

        public string DataType { get; private set; }

        public string NativeType { get; private set; }
        
        public string SystemType { get; private set; }

        public string SystemTypeWithSize
        {
            get
            {
                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    return string.Format("{0}({1})", SystemType.Replace("()", string.Empty), Size);

                return SystemType.Replace("[]", string.Format("[{0}]", Size));
            }
        }

        public string BaseSystemType
        {
            get
            {
                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    return SystemType.Replace("System.Nullable(Of ", string.Empty).Replace(")", string.Empty);

                return SystemType.Replace("?", string.Empty);
            }
        }

        private string _propertyName = string.Empty;
        public string PropertyName
        {
            get
            {
                if (string.IsNullOrEmpty(_propertyName))
                    _propertyName = NamingConventions.PropertyName(Name, false);

                return _propertyName;
            }
        }

        private string _privateMemberVariableName = string.Empty;
        public string PrivateMemberVariableName
        {
            get
            {
                if (string.IsNullOrEmpty(_privateMemberVariableName))
                    _privateMemberVariableName = NamingConventions.PrivateMemberVariableName(Name, false);

                return _privateMemberVariableName;
            }
        }

        private string _variableName = string.Empty;
        public string VariableName
        {
            get
            {
                if(string.IsNullOrEmpty(_variableName))
                    _variableName = NamingConventions.VariableName(Name, false);

                return _variableName;
            }
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

        #endregion    
    }
}