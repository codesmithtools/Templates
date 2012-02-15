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
        public static List<IProperty> GetUniqueSearchCriteriaMembers(this IEntity entity)
        {
            var members = new List<IProperty>();

            foreach (SearchCriteria sc in entity.SearchCriteria)
            {
                members = members.Union(sc.Properties).ToList();
            }

            foreach (IProperty property in entity.Key.Properties)
            {
                IProperty member1 = property;
                if (members.Exists(m => m.Name == member1.Name) == false)
                {
                    members.Add(property);
                }
            }

            return members;
        }

        /// <summary>
        /// Builds the insert select statement for parameterized sql.
        /// </summary>
        /// <param name="entity">The Entity.</param>
        /// <returns>Returns the select for the insert parameterized sql.</returns>
        public static string BuildInsertSelectStatement(this IEntity entity)
        {
            //IProperty guidColumn = entity.PrimaryKey.KeyMember;
            //if (entity.HasGuidPrimaryKeyMember)
            //{
            //    foreach (IProperty primaryKey in entity.Key.Properties)
            //    {
            //        if (primaryKey.DataType == DbType.Guid.ToString())
            //        {
            //            guidColumn = primaryKey;
            //            break;
            //        }
            //    }
            //}

            //if (entity.HasIdentityMember && entity.HasRowVersionMember)
            //{
            //    return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {4} = SCOPE_IDENTITY()",
            //                          entity.Key.Properties.BuildDataBaseColumns(),
            //                          entity.RowVersionMember.BuildDataBaseColumn(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          entity.IdentityMember.KeyName);
            //}
            //if (entity.HasIdentityMember)
            //{
            //    return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = SCOPE_IDENTITY()",
            //                          entity.Key.Properties.BuildDataBaseColumns(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          entity.IdentityMember.KeyName);
            //}
            //if (entity.HasGuidPrimaryKeyMember && entity.HasRowVersionMember)
            //{
            //    return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {5} = {4}{5}",
            //                          entity.Key.Properties.BuildDataBaseColumns(),
            //                          entity.RowVersionMember.BuildDataBaseColumn(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          Configuration.Instance.ParameterPrefix,
            //                          guidColumn.KeyName);
            //}
            //if (entity.HasGuidPrimaryKeyMember && entity.Key.Properties.Count > 1)
            //{
            //    return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {4} = {3}{4}",
            //                          entity.Key.Properties.BuildDataBaseColumns(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          Configuration.Instance.ParameterPrefix,
            //                          guidColumn.KeyName);
            //}
            //if (entity.HasRowVersionMember)
            //{
            //    return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = {4}{5}",
            //                          entity.RowVersionMember.BuildDataBaseColumn(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          entity.PrimaryKey.KeyMember.KeyName,
            //                          Configuration.Instance.ParameterPrefix,
            //                          entity.PrimaryKey.KeyMember.KeyName);
            //}

            return string.Empty;
        }

        /// <summary>
        /// Build the select for the update parameterized sql.
        /// </summary>
        /// <param name="entity">The Entity.</param>
        /// <returns>Returns the select for the update parameterized sql.</returns>
        public static string BuildUpdateSelectStatement(this IEntity entity)
        {
            IProperty guidColumn = entity.Key.Properties.FirstOrDefault();
            //if (entity.HasGuidPrimaryKeyMember)
            //{
            //    foreach (IProperty primaryKey in entity.Key.Properties)
            //    {
            //        if (primaryKey.DataType == DbType.Guid.ToString())
            //        {
            //            guidColumn = primaryKey;
            //            break;
            //        }
            //    }
            //}

            //if ((entity.MembersNonIdentityPrimaryKeys.Count > 0) && entity.HasRowVersionMember)
            //{
            //    return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] {4}",
            //                          entity.MembersNonIdentityPrimaryKeys.BuildDataBaseColumns(),
            //                          entity.RowVersionMember.BuildDataBaseColumn(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          entity.Key.Properties.BuildWhereStatements(true));
            //}
            //if (entity.MembersNonIdentityPrimaryKeys.Count > 0)
            //{
            //    return String.Format("; SELECT {0} FROM [{1}].[{2}] {3}",
            //        entity.MembersNonIdentityPrimaryKeys.BuildDataBaseColumns(),
            //        entity.Table.Owner,
            //        entity.Table.Name,
            //        entity.Key.Properties.BuildWhereStatements(true));
            //}
            //if (entity.HasGuidPrimaryKeyMember && entity.HasRowVersionMember)
            //{
            //    return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {5} = {4}{5}",
            //                          entity.Key.Properties.BuildDataBaseColumns(),
            //                          entity.RowVersionMember.BuildDataBaseColumn(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          Configuration.Instance.ParameterPrefix,
            //                          guidColumn.KeyName);
            //}
            //if (entity.HasGuidPrimaryKeyMember && entity.Key.Properties.Count > 0)
            //{
            //    return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {4} = {3}{4}",
            //                          entity.Key.Properties.BuildDataBaseColumns(),
            //                          entity.Table.Owner,
            //                          entity.Table.Name,
            //                          Configuration.Instance.ParameterPrefix,
            //                          guidColumn.KeyName);
            //}
            //if (entity.HasRowVersionMember)
            //{
            //    return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = {4}{5}",
            //        entity.RowVersionMember.BuildDataBaseColumn(),
            //        entity.Table.Owner,
            //        entity.Table.Name,
            //        entity.PrimaryKey.KeyMember.KeyName,
            //        Configuration.Instance.ParameterPrefix,
            //        entity.PrimaryKey.KeyMember.KeyName);
            //}

            return string.Empty;
        }
    }
}