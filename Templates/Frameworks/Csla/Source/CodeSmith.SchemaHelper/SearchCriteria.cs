using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CodeSmith.SchemaHelper.Util;

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

        #endregion

        #region Public Overridden Method(s)

        public override string ToString()
        {
            //Todo: eventually add suppport for preappending list...
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            if (SearchCriteriaType == SearchCriteriaEnum.ForeignKeysOneToMany || SearchCriteriaType == SearchCriteriaEnum.ForeignKeysManyToOne)
            {
                foreach (AssociationMember member in AssociationMembers)
                {
                    if (isFirst)
                    {
                        isFirst = false;

                        if (SearchCriteriaType == SearchCriteriaEnum.ForeignKeysManyToOne)
                            sb.AppendFormat(Configuration.Instance.SearchCriteriaProperty.Prefix, member.AssociatedColumn.Entity.ClassName);
                        else
                            sb.AppendFormat(Configuration.Instance.SearchCriteriaProperty.Prefix, member.ClassName);
                    }
                    else
                        sb.Append(Configuration.Instance.SearchCriteriaProperty.Delimeter);

                    if (SearchCriteriaType == SearchCriteriaEnum.ForeignKeysManyToOne)
                        sb.Append(Util.NamingConventions.PropertyName(member.AssociatedColumn.ColumnName));
                    else
                        sb.Append(Util.NamingConventions.PropertyName(member.ColumnName));
                }
            }
            else
            {

            #region Handle anything not a OneToMany.

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

        internal List<AssociationMember> AssociationMembers { get; set; }
        public List<Member> Members { get; private set; }

        public bool IsUniqueResult
        {
            get
            {
                bool result = false;
                foreach (Member member in Members)
                    if (member.IsUnique)
                    {
                        result = true;
                        break;
                    }
                return result;
            }
        }

        public string MethodName
        {
            get { return this.ToString(); }
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
                    case SearchCriteriaEnum.ForeignKeysManyToOne: //Parent
                        typeDesc = "Many To One Foreign Key";
                        break;
                    case SearchCriteriaEnum.ForeignKeysOneToMany: //Child
                        typeDesc = "One To Many Foreign Key";
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