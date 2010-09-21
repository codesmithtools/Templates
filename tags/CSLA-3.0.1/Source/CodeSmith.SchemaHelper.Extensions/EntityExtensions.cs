using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        /// <summary>
        /// Builds the insert select statement for parameterized sql.
        /// </summary>
        /// <param name="entity">The Entity.</param>
        /// <returns>Returns the select for the insert parameterized sql.</returns>
        public static string BuildInsertSelectStatement(this Entity entity)
        {
            Member guidColumn = entity.PrimaryKey.KeyMember;
            if (entity.HasGuidPrimaryKeyMember)
            {
                foreach (Member primaryKey in entity.PrimaryKey.KeyMembers)
                {
                    if (primaryKey.DataType == DbType.Guid.ToString())
                    {
                        guidColumn = primaryKey;
                        break;
                    }
                }
            }

            if (entity.HasIdentityMember && entity.HasRowVersionMember)
            {
                return string.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {4} = SCOPE_IDENTITY()",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.RowVersionMember.BuildDataBaseColumn(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      entity.IdentityMember.ColumnName);
            }
            if (entity.HasIdentityMember)
            {
                return string.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = SCOPE_IDENTITY()",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      entity.IdentityMember.ColumnName);
            }
            if (entity.HasGuidPrimaryKeyMember && entity.HasRowVersionMember)
            {
                return string.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {5} = {4}{5}",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.RowVersionMember.BuildDataBaseColumn(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.ColumnName);
            }
            if (entity.HasGuidPrimaryKeyMember && entity.PrimaryKey.KeyMembers.Count > 1)
            {
                return string.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {4} = {3}{4}",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.ColumnName);
            }
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

        /// <summary>
        /// Build the select for the update parameterized sql.
        /// </summary>
        /// <param name="entity">The Entity.</param>
        /// <returns>Returns the select for the update parameterized sql.</returns>
        public static string BuildUpdateSelectStatement(this Entity entity)
        {
            Member guidColumn = entity.PrimaryKey.KeyMember;
            if (entity.HasGuidPrimaryKeyMember)
            {
                foreach (Member primaryKey in entity.PrimaryKey.KeyMembers)
                {
                    if (primaryKey.DataType == DbType.Guid.ToString())
                    {
                        guidColumn = primaryKey;
                        break;
                    }
                }
            }

            if ((entity.MembersNonIdentityPrimaryKeys.Count > 0) && entity.HasRowVersionMember)
            {
                return string.Format("; SELECT {0}, {1} FROM [{2}].[{3}] {4}",
                                      entity.MembersNonIdentityPrimaryKeys.BuildDataBaseColumns(),
                                      entity.RowVersionMember.BuildDataBaseColumn(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      entity.PrimaryKey.KeyMembers.BuildWhereStatements(true));
            }
            if (entity.MembersNonIdentityPrimaryKeys.Count > 0)
            {
                return string.Format("; SELECT {0} FROM [{1}].[{2}] {3}",
                    entity.MembersNonIdentityPrimaryKeys.BuildDataBaseColumns(),
                    entity.Table.Owner,
                    entity.Table.Name,
                    entity.PrimaryKey.KeyMembers.BuildWhereStatements(true));
            }
            if (entity.HasGuidPrimaryKeyMember && entity.HasRowVersionMember)
            {
                return string.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {5} = {4}{5}",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.RowVersionMember.BuildDataBaseColumn(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.ColumnName);
            }
            if (entity.HasGuidPrimaryKeyMember && entity.PrimaryKey.KeyMembers.Count > 0)
            {
                return string.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {4} = {3}{4}",
                                      entity.PrimaryKey.KeyMembers.BuildDataBaseColumns(),
                                      entity.Table.Owner,
                                      entity.Table.Name,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.ColumnName);
            }
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