using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for SearchCriteriaExtensions
    /// </summary>
    public static class SearchCriteriaExtensions
    {
        public static string BuildObjectInitializer(this SearchCriteria sc)
        {
            return sc.BuildObjectInitializer(false);
        }

        public static string BuildObjectInitializer(this SearchCriteria sc, bool isObjectFactory)
        {
            string parameters = string.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaEnum.ForeignKey)
            {
                foreach (AssociationMember member in sc.AssociationMembers)
                {
                    var propertyName = isObjectFactory ? string.Format("item.{0}", member.PropertyName) : member.VariableName;
                    var resolvedPropertyName = member.PropertyName;
                    if(isObjectFactory)
                    {
                        if (sc.IsChild)
                        {
                            resolvedPropertyName = member.MemberPropertyName;
                            var nullable = member.AssociatedColumn.IsNullable && member.AssociatedColumn.SystemType != "System.String" ? ".Value" : string.Empty;
                            propertyName = string.Format("item.{0}{1}", member.AssociatedMemberPropertyName, nullable);
                        }
                        else
                        {
                            resolvedPropertyName = member.AssociatedMemberPropertyName;
                            var nullable = member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()" ? ".Value" : string.Empty;
                            propertyName = string.Format("item.{0}{1}", member.MemberPropertyName, nullable);
                        }
                    }

                    parameters += string.Format("\r\n\t\t{0} = {1}", resolvedPropertyName, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (Member member in sc.Members)
                {
                    var propertyName = isObjectFactory ? string.Format("item.{0}", member.PropertyName) : member.VariableName;
                    parameters += string.Format("\r\n\t\t{0} = {1}{2}", member.PropertyName, propertyName, member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()" ? ".Value" : string.Empty);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n' });
        }

        public static string BuildUpdateStatements(this SearchCriteria sc, string associationPropertyName)
        {
            return sc.BuildUpdateStatements(associationPropertyName, "item.");
        }

        public static string BuildUpdateStatements(this SearchCriteria sc, string associationPropertyName, string prefix)
        {
            string parameters = string.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaEnum.ForeignKey)
            {
                foreach (AssociationMember member in sc.AssociationMembers)
                {
                    var propertyName = member.PropertyName;
                    var resolvedPropertyName = member.PropertyName;

                    if (sc.IsChild)
                    {
                        var nullable = member.AssociatedColumn.IsNullable && member.AssociatedColumn.SystemType != "System.String" ? ".Value" : string.Empty;
                        resolvedPropertyName = member.MemberPropertyName;
                        propertyName = string.Format("{0}{1}", member.AssociatedMemberPropertyName, nullable);
                    }
                    else
                    {
                        var nullable = member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()" ? ".Value" : string.Empty;
                        resolvedPropertyName = member.AssociatedMemberPropertyName;
                        propertyName = string.Format("{0}{1}", member.MemberPropertyName, nullable);
                    }

                    parameters += string.Format("\r\n\t\t{0}.{1} = {2}{3}", associationPropertyName, resolvedPropertyName, prefix, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (Member member in sc.Members)
                {
                    parameters += string.Format("\r\n\t\t{0}.{1} = {1}{2}", associationPropertyName, member.PropertyName, prefix);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n' });
        }
    }
}
