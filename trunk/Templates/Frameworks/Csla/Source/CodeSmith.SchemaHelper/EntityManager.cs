using System;
using System.Collections.Generic;
using System.Diagnostics;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class EntityManager
    {
        #region Constructor(s)

        public EntityManager(DatabaseSchema database) : this(database.Tables) {}
        public EntityManager(TableSchemaCollection tables)
        {
            ExcludedTables = new List<TableSchema>();
            Entities = new List<Entity>();

            if (tables != null && tables.Count > 0 && tables[0].Database != null)
            {
                Database = tables[0].Database;
                if (!Database.DeepLoad)
                {
                    Database.DeepLoad = true;
                    Database.Refresh();
                }
               
                foreach ( TableSchema table in tables )
                {
                    if(table == null)
                        continue;
                   
                    bool includeManyToMany = table.IsManyToMany() && !Configuration.Instance.IncludeManyToManyEntity;
                    if (Configuration.Instance.ExcludeRegexIsMatch(table.FullName) || includeManyToMany)
                        ExcludedTables.Add( table );
                    else if (!table.HasPrimaryKey)
                        Trace.WriteLine(string.Format("Skipping table: '{0}', no Primary Key was found!", table.Name));
                    else
                    {
                        Entities.Add(new Entity(table));
                    }
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