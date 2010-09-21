using System;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for SearchCriteriaExtensions
    /// </summary>
    public static class SearchCriteriaExtensions
    {
        #region BuildObjectInitializer

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
                            if (member.AssociatedColumn.IsNullable && member.AssociatedColumn.SystemType != "System.String") continue;

                            resolvedPropertyName = member.MemberPropertyName;
                            propertyName = string.Format("item.{0}", member.AssociatedMemberPropertyName);
                        }
                        else
                        {
                            if (member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") continue;

                            resolvedPropertyName = member.AssociatedMemberPropertyName;
                            propertyName = string.Format("item.{0}", member.MemberPropertyName);
                        }
                    }

                    parameters += string.Format(", {0} = {1}", resolvedPropertyName, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (Member member in sc.Members)
                {
                    if(member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") continue;

                    var propertyName = isObjectFactory ? string.Format("item.{0}", member.PropertyName) : member.VariableName;
                    parameters += string.Format(", {0} = {1}", member.PropertyName, propertyName);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        #endregion

        #region BuildNullableObjectInitializer

        public static string BuildNullableObjectInitializer(this SearchCriteria sc)
        {
            return sc.BuildNullableObjectInitializer(false);
        }

        public static string BuildNullableObjectInitializer(this SearchCriteria sc, bool isObjectFactory)
        {
            string parameters = string.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaEnum.ForeignKey)
            {
                foreach (AssociationMember member in sc.AssociationMembers)
                {
                    var propertyName = isObjectFactory ? string.Format("item.{0}", member.PropertyName) : member.VariableName;
                    var resolvedPropertyName = member.PropertyName;
                    if (isObjectFactory)
                    {
                        if (sc.IsChild)
                        {
                            if ((member.AssociatedColumn.IsNullable && member.AssociatedColumn.SystemType != "System.String") == false) continue;

                            resolvedPropertyName = member.MemberPropertyName;
                            var nullable = member.AssociatedColumn.IsNullable && member.AssociatedColumn.SystemType != "System.String" ? ".Value" : string.Empty;
                            propertyName = string.Format("item.{0}{1}", member.AssociatedMemberPropertyName, nullable);
                        }
                        else
                        {
                            if ((member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") == false) continue;

                            resolvedPropertyName = member.AssociatedMemberPropertyName;
                            propertyName = string.Format("item.{0}.Value", member.MemberPropertyName);
                        }
                    }

                    parameters += string.Format("\r\n\t\t\t\tif({1}.HasValue) criteria.{0} = {1};", resolvedPropertyName, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (Member member in sc.Members)
                {
                    if((member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") == false) continue;

                    var propertyName = isObjectFactory ? string.Format("item.{0}", member.PropertyName) : member.VariableName;
                    parameters += string.Format("\r\n\t\t\t\tif({1}.HasValue) criteria.{0} = {1}.Value;", member.PropertyName, propertyName);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t' });
        }

        #endregion

        #region BuildUpdateStatements

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
                        var nullable = member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]" ? ".Value" : string.Empty;
                        resolvedPropertyName = member.AssociatedMemberPropertyName;
                        propertyName = string.Format("{0}{1}", member.MemberPropertyName, nullable);
                    }

                    parameters += string.Format("\r\n\t\t\t\t{0}.{1} = {2}{3};", associationPropertyName, resolvedPropertyName, prefix, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (Member member in sc.Members)
                {
                    parameters += string.Format("\r\n\t\t\t\t{0}.{1} = {1}{2};", associationPropertyName, member.PropertyName, prefix);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n', });
        }

        #endregion
    }
}
