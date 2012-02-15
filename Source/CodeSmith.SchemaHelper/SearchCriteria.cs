using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSmith.SchemaHelper
{
    public class SearchCriteria
    {
        #region Constructor(s)

        public SearchCriteria(SearchCriteriaEnum type)
        {
            AssociationMembers = new List<AssociationMember>();
            Members = new List<Member>();
            SearchCriteriaType = type;
        }

        public SearchCriteria(SearchCriteriaEnum type, bool treatAsChild) : this(type)
        {
            IsChild = treatAsChild;
        }

        #endregion

        #region Public Overridden Method(s)

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            if (SearchCriteriaType == SearchCriteriaEnum.ForeignKey)
            {
                foreach (AssociationMember member in AssociationMembers)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        sb.AppendFormat(Configuration.Instance.SearchCriteriaProperty.Prefix, member.AssociatedColumn.Entity.ClassName);
                    }
                    else
                        sb.Append(Configuration.Instance.SearchCriteriaProperty.Delimeter);

                    if (IsChild)
                        sb.Append(member.MemberPropertyName);
                    else
                        sb.Append(member.AssociatedMemberPropertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (Member member in Members)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        sb.AppendFormat(Configuration.Instance.SearchCriteriaProperty.Prefix, member.Entity.ClassName);
                    }
                    else
                        sb.Append(Configuration.Instance.SearchCriteriaProperty.Delimeter);

                    sb.Append(member.PropertyName);
                }

                #endregion
            }

            sb.Append(Configuration.Instance.SearchCriteriaProperty.Suffix);

            return sb.ToString();
        }

        #endregion

        #region Public Read-Only Properties

        /// <summary>
        /// This is the FK associated members.
        /// </summary>
        public List<AssociationMember> AssociationMembers { get; set; }
        
        /// <summary>
        /// This is the main list that you should use...
        /// </summary>
        public List<Member> Members { get; private set; }

        public bool IsUniqueResult
        {
            get;
            set;
        }

        public bool IsChild { get; internal set; }

        public string MethodName
        {
            get { return ToString(); }
        }

        /// <summary>
        /// Return the string description of this Search Criteria
        /// </summary>
        public string SearchCriteriaDescription
        {
            get
            {
                string typeDesc = string.Empty;
                switch (SearchCriteriaType)
                {
                    case SearchCriteriaEnum.PrimaryKey:
                        typeDesc = "Primary Key";
                        break;
                    case SearchCriteriaEnum.ForeignKey: //Parent
                        typeDesc = "Foreign Key";
                        break;
                    case SearchCriteriaEnum.Index:
                        typeDesc = "Index";
                        break;
                }

                return typeDesc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SearchCriteriaEnum SearchCriteriaType { get; internal set; }

        #endregion

        #region Internal Read-Only Properties

        internal string Key
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (Member member in Members)
                    sb.Append(member.PropertyName);

                return sb.ToString();
            }
        }

        #endregion
    }
}