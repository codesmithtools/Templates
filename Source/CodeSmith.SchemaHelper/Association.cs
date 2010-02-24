using System;
using System.Collections.ObjectModel;
using CodeSmith.SchemaHelper.Util;
using System.Text;


namespace CodeSmith.SchemaHelper
{
    public class Association : Collection<AssociationMember>
    {
        /// <summary>
        /// Returns the appended Key for all AssociatedMembers
        /// </summary>
        public string Key
        {
            get
            {
                string key = string.Empty;

                foreach (var item in this)
                {
                    key += item.Key + "|";  
                }

                return key.TrimEnd('|');
            }
        }

        /// <summary>
        /// Returns the association type of the First associated member.
        /// 
        /// TODO: Handle different association types between members
        /// </summary>
        public AssociationType AssociationType
        {
            get
            {
                if(this.Count > 0)
                    return this[0].AssociationType;

                throw new Exception("There are no Associated members to get the AssociationType.");
            }
        }

        internal bool ContainsKey(string key)
        {
            foreach (var item in this)
            {
                if (key == item.Key)
                    return true;
            }

            return false;
        }

        private string _propertyName = string.Empty;

        /// <summary>
        /// Returns the concatenated PropertyName of all associated members.
        /// </summary>
        public string PropertyName
        {
            get 
            {
                if (!string.IsNullOrEmpty(_propertyName))
                    return _propertyName;

                string propertyName = string.Empty;
                foreach (var item in this)
                {
                    propertyName += NamingConventions.PropertyName(item.Name); 
                }

                return propertyName;
            }
            internal set
            {
                _propertyName = value;
            }
        }

        public string MembersToString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool isFirst = true;

                foreach (AssociationMember member in this)
                {
                    if (isFirst)
                    {
                        sb.Append(Configuration.Instance.SearchCriteriaProperty.Delimeter);
                        isFirst = false;
                    }
                    if(this.AssociationType == AssociationType.ManyToOne)
                        sb.Append(Util.NamingConventions.PropertyName(member.AssociatedColumn.ColumnName));
                    else
                        sb.Append(Util.NamingConventions.PropertyName(member.ColumnName));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns the concatenated PrivateMemberVariableName of all associated members.
        /// </summary>
        public string PrivateMemberVariableName
        {
            get
            {
                return NamingConventions.PrivateMemberVariableName(PropertyName);
            }
        }

        /// <summary>
        /// Returns the concatenated VariableName of all associated members.
        /// </summary>
        public string VariableName
        {
            get
            {
                return NamingConventions.VariableName(PropertyName);
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description) && this.Count > 0)
                    _description = this[0].Table.Description;

                return _description;
            }
        }

        public bool HasDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(Description))
                    return Description.Trim().Length > 0;

                return false;
            }
        }

        /// <summary>
        /// Returns the ClassName for the first associated member
        /// </summary>
        public string ClassName
        {
            get
            {
                if (this.Count > 0)
                    return this[0].Table.ClassName();

                throw new Exception("There are no Associated members to get the ClassName.");
            }
        }

        private string _tableName = string.Empty;
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName) && this.Count > 0)
                    _tableName = this.AssociationType == AssociationType.ManyToOne ? this[0].AssociatedColumn.TableName : this[0].TableName;

                return _tableName;
            }
        }

        public SearchCriteria SearchCriteria
        {
            get; internal set;
        }
    }
}