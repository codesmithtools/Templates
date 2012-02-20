using System;
using CodeSmith.SchemaHelper;
using SchemaExplorer;

namespace Generator.CSLA
{
    class CSLASchemaExplorerEntityProvider : SchemaExplorerEntityProvider
    {
        private string _extendedPropertyName;

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database) : base(database)
        {
        }

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database, TableSchemaCollection tables) : base(database)
        {
            _tables = tables;
        }

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database, TableSchemaCollection tables, string extendedPropertyName) : base(database)
        {
            _tables = tables;
            _extendedPropertyName = extendedPropertyName;
        }

    }
}
