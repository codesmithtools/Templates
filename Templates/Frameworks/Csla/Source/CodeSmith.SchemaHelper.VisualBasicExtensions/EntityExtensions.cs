using System;
using System.Collections.Generic;
using System.Globalization;
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
        //public static string BuildEnumValues(this Entity entity)
        //{
        //    string enumValues = string.Empty;

        //    foreach (SearchCriteria sc in entity.SearchCriteria)
        //    {
        //        enumValues += string.Format("\n\t\t\t{0},", sc.MethodName);
        //    }

        //    return enumValues.TrimStart(new[] { '\t', '\n' }).TrimEnd(new[] { ',' });
        //}

        public static List<Member> GetUniqueSearchCriteriaMembers(this Entity entity)
        {
            List<Member> members = new List<Member>();

            foreach (SearchCriteria sc in entity.SearchCriteria)
            {
                members = members.Union(sc.Members).ToList();
            }

            foreach (Member member in entity.PrimaryKey.KeyMembers)
            {
                Member member1 = member;
                if (members.Exists(m => m.Name == member1.Name) == false)
                {
                    members.Add(member);
                }
            }

            return members;
        }

        //public static string FindOneToManyOrManyToManyListSearchCriteria(this Entity entity, string memberName)
        //{
        //    foreach (AssociationMember association in entity.ManyToOne)
        //    {
        //        if(association.ColumnName.EndsWith(memberName, true, CultureInfo.InvariantCulture))
        //            return string.Format("{0}{1}", Configuration.Instance.SearchCriteriaProperty.Prefix, NamingConventions.PropertyName(association.ColumnName));       

        //        foreach (SearchCriteria sc in association.ListSearchCriteria)
        //        {
        //            if (sc.MethodName.EndsWith(memberName))
        //                return sc.MethodName;
        //        }
        //    }

        //    return null;
        //}

        //public static string ResolveCriteriaPropertyName(this Entity entity, string columnName)
        //{
        //    return ResolveCriteriaPropertyName(entity, columnName, false);
        //}

        //public static string ResolveCriteriaPropertyName(this Entity entity, string columnName, string className)
        //{
        //    return ResolveCriteriaPropertyName(entity, columnName, false, className);
        //}

        //public static string ResolveCriteriaPropertyName(this Entity entity, string columnName, bool isCritiaClass)
        //{
        //    return ResolveCriteriaPropertyName(entity, columnName, isCritiaClass, string.Empty);
        //}

        //public static string ResolveCriteriaPropertyName(this Entity entity, string columnName, bool isCritiaClass, string className)
        //{
        //    string propertyName = NamingConventions.PropertyName(columnName);

        //    foreach (AssociationMember association in entity.RemoteAssociations)
        //    {
        //        if(association.TableName == className)
        //        {
        //            if (association.ColumnName.EndsWith(columnName, true, CultureInfo.InvariantCulture))
        //                return NamingConventions.PropertyName(association.ColumnName); 
        //        }
        //    }

        //    foreach (AssociationMember association in entity.ManyToOne)
        //    {
        //        if (association.PropertyName == propertyName)
        //        {
        //            foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
        //            {
        //                return sc.Members[0].PropertyName;
        //            }
        //        }

        //        if (!isCritiaClass)
        //        {
        //            foreach (Member member in association.AssociationEntity().Members)
        //            {
        //                if (columnName.EndsWith(member.ColumnName, true, CultureInfo.InvariantCulture))
        //                    return NamingConventions.PropertyName(member.ColumnName);
        //            }
        //        }
        //    }

        //    return propertyName;
        //}

        //public static string ResolveCriteriaVariableName(this Entity entity, string columnName)
        //{
        //    string variableName = NamingConventions.VariableName(columnName);

        //    foreach (AssociationMember association in entity.ManyToOne)
        //    {

        //        if (association.VariableName == variableName)
        //        {
        //            foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
        //            {
        //                return sc.Members[0].VariableName;
        //            }

        //            foreach (Member member in association.AssociationEntity().Members)
        //            {
        //                if (columnName.EndsWith(member.ColumnName, true, CultureInfo.InvariantCulture))
        //                    return NamingConventions.VariableName(member.ColumnName);
        //            }
        //        }
        //    }

        //    return variableName;
        //}

        //public static string ResolveCriteriaPrivateMemberVariableName(this Entity entity, string columnName)
        //{
        //    return ResolveCriteriaPrivateMemberVariableName(entity, columnName, null);
        //}

        //public static string ResolveCriteriaPrivateMemberVariableName(this Entity associationEntity, string columnName, Entity entity)
        //{
        //    // If the current entity contains the column then return..
        //    if (entity != null && entity.Table.Columns.Contains(columnName))
        //        return NamingConventions.PrivateMemberVariableName(columnName);

        //    string variableName = NamingConventions.PrivateMemberVariableName(columnName);

        //    foreach (AssociationMember association in associationEntity.ManyToOne)
        //    {
        //        if (association.PrivateMemberVariableName == variableName)
        //        {
        //            foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
        //            {
        //                return sc.Members[0].PrivateMemberVariableName;
        //            }

        //            foreach (Member member in association.AssociationEntity().Members)
        //            {
        //                if (columnName.EndsWith(member.ColumnName, true, CultureInfo.InvariantCulture))
        //                    return NamingConventions.PrivateMemberVariableName(member.ColumnName);
        //            }
        //        }
        //    }

        //    return variableName;
        //}

        //public static string ResolveCriteriaColumnName(this Entity entity, string columnName)
        //{
        //    string variableName = NamingConventions.PrivateMemberVariableName(columnName);

        //    foreach (AssociationMember association in entity.ManyToOne)
        //    {
        //        if (association.PrivateMemberVariableName == variableName)
        //        {
        //            foreach (SearchCriteria sc in association.AssociationEntity().SearchCriteria)
        //            {
        //                return sc.Members[0].ColumnName;
        //            }

        //            foreach (Member member in association.AssociationEntity().Members)
        //            {
        //                if (columnName.EndsWith(member.ColumnName, true, CultureInfo.InvariantCulture))
        //                    return member.ColumnName;
        //            }
        //        }
        //    }

        //    return variableName;
        //}

        //public static bool HasByteArrayColumn(this Entity entity)
        //{
        //    if (entity.Members.HasByteArrayColumn())
        //        return true;

        //    if (entity.ManyToOne.HasByteArrayColumn() || entity.ToManyUnion.HasByteArrayColumn())
        //        return true;


        //    return false;
        //}

        public static string BuildInsertSelectStatement(this Entity entity)
        {
            string query = string.Empty;

            if (entity.PrimaryKey.IsIdentity && entity.HasRowVersionMember)
            {
                query = string.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {4} = SCOPE_IDENTITY()",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.RowVersionMember.BuildDataBaseColumn(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      entity.PrimaryKey.KeyMember.ColumnName);
            }
            else if (entity.PrimaryKey.IsIdentity)
            {
                query = string.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = SCOPE_IDENTITY()",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      entity.PrimaryKey.KeyMember.ColumnName);
            }
            else if (entity.HasRowVersionMember)
            {
                query = string.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = {4}{5}",
                                      entity.RowVersionMember.BuildDataBaseColumn(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      entity.PrimaryKey.KeyMember.ColumnName,
                                      Configuration.Instance.ParameterPrefix,
                                      entity.PrimaryKey.KeyMember.ColumnName);
            }

            return query;
        }

        public static string BuildUpdateSelectStatement(this Entity entity)
        {
            if (entity.HasRowVersionMember)
            {
                return string.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = {4}{5}",
                                     entity.RowVersionMember.BuildDataBaseColumn(),
                                     entity.Table.Owner,
                                     entity.Table.Name,
                                     entity.PrimaryKey.KeyMember.ColumnName,
                                     Configuration.Instance.ParameterPrefix,
                                     entity.PrimaryKey.KeyMember.ColumnName);
            }

            return string.Empty;
        }
    }
}