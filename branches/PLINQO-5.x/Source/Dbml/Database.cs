using System.Collections.Generic;
using System.Diagnostics;

namespace LinqToSqlShared.DbmlObjectModel
{
    [DebuggerDisplay("Name = {Name}, Class = {Class}")]
    public class Database : Node
    {
        private readonly FunctionCollection functions = new FunctionCollection();
        private readonly TableCollection tables = new TableCollection();

        public AccessModifier? AccessModifier { get; set; }

        public string BaseType { get; set; }

        public string Class { get; set; }

        public Connection Connection { get; set; }

        public string ContextNamespace { get; set; }

        public string EntityBase { get; set; }

        public string EntityNamespace { get; set; }

        public bool? ExternalMapping { get; set; }

        public FunctionCollection Functions
        {
            get { return functions; }
        }

        public ClassModifier? Modifier { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }

        public SerializationMode? Serialization { get; set; }

        public TableCollection Tables
        {
            get { return tables; }
        }

        private Dictionary<string, Table> typeToTable;
            
        public Type GetTypeByName(string name)
        {
            if (typeToTable == null)
                typeToTable = Dbml.GetTablesByTypeName(this);

            Table table;
            if (typeToTable.TryGetValue(name, out table))
                return table.Type;

            return null;
        }

    }
}