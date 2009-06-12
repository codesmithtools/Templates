using System;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    public static class SearchCriteriaExtensions
    {
        #region Public Method(s)

        public static string GetListMethodName(this SearchCriteria searchCriteria, AssociationMember member)
        {
            return string.Format("{0}{1}", Configuration.Instance.SearchCriteriaProperty.Prefix, NamingConventions.PropertyName(member.ColumnName));       
        }


        #endregion
    }
}
