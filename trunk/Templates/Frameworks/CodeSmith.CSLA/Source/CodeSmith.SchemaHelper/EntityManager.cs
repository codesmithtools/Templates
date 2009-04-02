using System;
using System.Collections.Generic;
using System.Diagnostics;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class EntityManager
    {
        #region Constructor(s)

        public EntityManager(DatabaseSchema database) : this(database.Tables) { }
        public EntityManager(TableSchemaCollection tables)
        {
            if (tables.Count > 0)
            {
                Database = tables[0].Database;
                ExcludedTables = new List< TableSchema >();

                Entities = new List< Entity >();
                foreach ( TableSchema table in tables )
                {
                    if ( Configuration.Instance.ExcludeTableRegexIsMatch( table.FullName ) || table.IsManyToMany() )
                        ExcludedTables.Add( table );
                    else if (!table.HasPrimaryKey)
                        Trace.WriteLine(string.Format("Skipping table: '{0}', no Primary Key was found!", table.Name));
                    else if (table.ContainsCompositeKeys())
                        Trace.WriteLine(string.Format("Skipping table: '{0}', contains composite keys!", table.Name));
                    else
                        Entities.Add(new Entity(table));
                }
            }
        }

        #endregion

        #region Public Read-Only Properties

        public List<Entity> Entities { get; private set; }
        public List<TableSchema> ExcludedTables { get; private set; }
        public DatabaseSchema Database { get; private set; }

        #endregion
    }
}