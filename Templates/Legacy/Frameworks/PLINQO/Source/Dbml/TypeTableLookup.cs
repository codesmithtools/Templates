using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: TypeTableLookup

        private class TypeTableLookup : DbmlVisitor
        {
            private readonly Dictionary<string, Table> typeToTable = new Dictionary<string, Table>();
            private Table currentTable;

            public static Dictionary<string, Table> CreateLookup(Database db)
            {
                var lookup = new TypeTableLookup();
                lookup.VisitDatabase(db);
                return lookup.typeToTable;
            }

            public override Table VisitTable(Table table)
            {
                Table table2;
                currentTable = table;
                try
                {
                    table2 = base.VisitTable(table);
                }
                finally
                {
                    currentTable = null;
                }
                return table2;
            }

            public override Type VisitType(Type type)
            {
                if (currentTable != null)
                {
                    typeToTable[type.Name] = currentTable;
                }
                return base.VisitType(type);
            }
        }

        #endregion
    }
}