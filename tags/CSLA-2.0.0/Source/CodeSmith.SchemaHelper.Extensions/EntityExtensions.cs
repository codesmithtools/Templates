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
        public static List<Member> GetUniqueSearchCriteriaMembers(this Entity entity)
        {
            List<Member> members = new List<Member>();

            foreach (SearchCriteria sc in entity.SearchCriteria(SearchCriteriaEnum.All))
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