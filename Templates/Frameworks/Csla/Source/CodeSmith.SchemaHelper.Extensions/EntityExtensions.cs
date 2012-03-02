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
            var keyProperties = entity.GetProperties(PropertyType.Key);
            IProperty guidColumn = null;
            foreach (ISchemaProperty primaryKey in keyProperties)
            {
                if (primaryKey.DataType == DbType.Guid)
                {
                    guidColumn = primaryKey;
                    break;
                }
            }

            if (entity.IdentityProperty != null && entity.HasConcurrencyProperty)
            {
                return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {4} = SCOPE_IDENTITY()",
                                      keyProperties.BuildDataBaseColumns(),
                                      entity.ConcurrencyProperty.BuildDataBaseColumn(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      entity.IdentityProperty.KeyName);
            }
            if (entity.IdentityProperty != null)
            {
                return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {3} = SCOPE_IDENTITY()",
                                      keyProperties.BuildDataBaseColumns(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      entity.IdentityProperty.KeyName);
            }
            if (guidColumn != null && entity.HasConcurrencyProperty)
            {
                return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {5} = {4}{5}",
                                      keyProperties.BuildDataBaseColumns(),
                                      entity.ConcurrencyProperty.BuildDataBaseColumn(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.KeyName);
            }
            if (guidColumn != null && entity.Key.Properties.Count > 1)
            {
                return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {4} = {3}{4}",
                                      keyProperties.BuildDataBaseColumns(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.KeyName);
            }
            if (entity.HasConcurrencyProperty)
            {
                return String.Format("; SELECT {0} FROM [{1}].[{2}] {3}",
                                      entity.ConcurrencyProperty.BuildDataBaseColumn(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      entity.GetProperties(PropertyType.Key).BuildWhereStatements());
            }

            return String.Empty;
        }

        /// <summary>
        /// Build the select for the update parameterized sql.
        /// </summary>
        /// <param name="entity">The Entity.</param>
        /// <returns>Returns the select for the update parameterized sql.</returns>
        public static string BuildUpdateSelectStatement(this IEntity entity)
        {
            var keyProperties = entity.GetProperties(PropertyType.Key);
            IProperty guidColumn = null;
            foreach (ISchemaProperty primaryKey in keyProperties)
            {
                if (primaryKey.DataType == DbType.Guid)
                {
                    guidColumn = primaryKey;
                    break;
                }
            }

            if ((keyProperties.Count(p => !p.IsType(PropertyType.Identity)) > 0) && entity.HasConcurrencyProperty)
            {
                return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] {4}",
                                      keyProperties.Where(p => !p.IsType(PropertyType.Identity)).ToList().BuildDataBaseColumns(),
                                      entity.ConcurrencyProperty.BuildDataBaseColumn(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      keyProperties.BuildWhereStatements(true));
            }
            if (entity.Properties.Count(p => !p.IsType(PropertyType.Identity)) > 0)
            {
                return String.Format("; SELECT {0} FROM [{1}].[{2}] {3}",
                    keyProperties.Where(p => !p.IsType(PropertyType.Identity)).ToList().BuildDataBaseColumns(),
                    entity.SchemaName,
                    entity.EntityKeyName,
                    entity.GetProperties(PropertyType.Key).BuildWhereStatements(true));
            }
            if (guidColumn != null && entity.HasConcurrencyProperty)
            {
                return String.Format("; SELECT {0}, {1} FROM [{2}].[{3}] WHERE {5} = {4}{5}",
                                      keyProperties.BuildDataBaseColumns(),
                                      entity.ConcurrencyProperty.BuildDataBaseColumn(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.KeyName);
            }
            if (guidColumn != null && keyProperties.Count > 0)
            {
                return String.Format("; SELECT {0} FROM [{1}].[{2}] WHERE {4} = {3}{4}",
                                      keyProperties.BuildDataBaseColumns(),
                                      entity.SchemaName,
                                      entity.EntityKeyName,
                                      Configuration.Instance.ParameterPrefix,
                                      guidColumn.KeyName);
            }
            if (entity.HasConcurrencyProperty)
            {
                return String.Format("; SELECT {0} FROM [{1}].[{2}] {3}",
                    entity.ConcurrencyProperty.BuildDataBaseColumn(),
                    entity.SchemaName,
                    entity.EntityKeyName,
                    keyProperties.BuildWhereStatements()); ;
            }

            return String.Empty;
        }
    }
}