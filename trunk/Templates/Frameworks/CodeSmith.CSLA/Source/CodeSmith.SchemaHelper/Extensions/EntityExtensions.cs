using System;
using System.Collections.Generic;
using System.Linq;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for EntityExtensions
    /// </summary>
    public static class EntityExtensions
    {
        public static string BuildEnumValues(this Entity entity)
        {
            string enumValues = string.Empty;

            foreach (SearchCriteria sc in entity.SearchCriteria)
            {
                enumValues += string.Format("\n\t\t\t{0},", sc.MethodName);
            }

            return enumValues.TrimStart(new[] { '\t', '\n' }).TrimEnd(new[] { ',' });
        }

        public static List<MemberBase> GetUniqueSearchCriteriaMembers(this Entity entity)
        {
            List<MemberBase> member = new List<MemberBase>();

            foreach (SearchCriteria sc in entity.SearchCriteria)
            {
                member = member.Union(sc.Members).ToList();
            }

            return member;
        }

        public static SearchCriteria FindOneToManyOrManyToManyListSearchCriteria(this Entity entity, string memberName)
        {
            foreach (AssociationMember association in entity.ManyToOne)
            {
                foreach (SearchCriteria sc in association.ListSearchCriteria)
                {
                    if (sc.MethodName.EndsWith(memberName))
                        return sc;
                }
            }

            return null;
        }

        public static string ResolveCriteriaPropertyName(this Entity entity, string columnName)
        {
            string propertyName = NamingConventions.PropertyName(columnName);

            foreach (AssociationMember association in entity.ManyToOne)
            {

                if (association.PropertyName == propertyName)
                {
                    foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
                    {
                        return sc.Members[0].PropertyName;

                    }
                }
            }

            return propertyName;
        }

        public static string ResolveCriteriaVariableName(this Entity entity, string columnName)
        {
            string variableName = NamingConventions.VariableName(columnName);

            foreach (AssociationMember association in entity.ManyToOne)
            {

                if (association.VariableName == variableName)
                {
                    foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
                    {
                        return sc.Members[0].VariableName;

                    }
                }
            }

            return variableName;
        }

        public static string ResolveCriteriaPrivateMemberVariableName(this Entity entity, string columnName)
        {
            string variableName = NamingConventions.PrivateMemberVariableName(columnName);

            foreach (AssociationMember association in entity.ManyToOne)
            {
                if (association.PrivateMemberVariableName == variableName)
                {
                    foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
                    {
                        return sc.Members[0].PrivateMemberVariableName;

                    }
                }
            }

            return variableName;
        }
    }
}