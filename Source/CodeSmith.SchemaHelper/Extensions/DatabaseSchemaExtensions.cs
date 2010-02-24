using System;

using CodeSmith.SchemaHelper.Util;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for DatabaseSchema
    /// </summary>
    public static class DatabaseSchemaExtensions
    {
        public static string Namespace(this DatabaseSchema database)
        {
            return NamingConventions.PropertyName(database.Name);
        }
    }
}
