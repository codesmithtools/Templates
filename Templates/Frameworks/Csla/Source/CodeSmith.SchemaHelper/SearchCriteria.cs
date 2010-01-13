using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSmith.SchemaHelper
{
    public class SearchCriteria
    {
        #region Constructor(s)

        public SearchCriteria(bool isPrimaryKey)
        {
            Members = new List<Member>();
            IsPrimaryKey = isPrimaryKey;
        }

        #endregion

        #region Public Overridden Method(s)

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            sb.Append(Configuration.Instance.SearchCriteriaProperty.Prefix);
            foreach (Member member in Members)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(Configuration.Instance.SearchCriteriaProperty.Delimeter);

                sb.Append(member.PropertyName);
            }

            sb.Append(Configuration.Instance.SearchCriteriaProperty.Suffix);

            return sb.ToString();
        }

        #endregion

        #region Public Read-Only Properties

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

        public bool IsPrimaryKey { get; private set; }

        public string MethodName
        {
            get { return this.ToString(); }
        }

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