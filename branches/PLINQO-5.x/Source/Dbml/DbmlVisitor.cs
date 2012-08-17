namespace LinqToSqlShared.DbmlObjectModel
{
    internal abstract class DbmlVisitor
    {
        public virtual Association VisitAssociation(Association association)
        {
            return association;
        }

        public virtual Column VisitColumn(Column column)
        {
            return column;
        }

        public virtual Connection VisitConnection(Connection connection)
        {
            return connection;
        }

        public virtual Database VisitDatabase(Database db)
        {
            if (db == null)
                return null;
            VisitConnection(db.Connection);
            foreach (Table table in db.Tables)
            {
                VisitTable(table);
            }
            foreach (Function function in db.Functions)
            {
                VisitFunction(function);
            }
            return db;
        }

        public virtual Function VisitFunction(Function f)
        {
            if (f == null)
                return null;
            foreach (Parameter parameter in f.Parameters)
            {
                VisitParameter(parameter);
            }
            foreach (Type type in f.Types)
            {
                VisitType(type);
            }
            VisitReturn(f.Return);
            return f;
        }

        public virtual Parameter VisitParameter(Parameter parameter)
        {
            return parameter;
        }

        public virtual Return VisitReturn(Return r)
        {
            return r;
        }

        public virtual Table VisitTable(Table table)
        {
            if (table == null)
                return null;
            VisitType(table.Type);
            VisitTableFunction(table.InsertFunction);
            VisitTableFunction(table.UpdateFunction);
            VisitTableFunction(table.DeleteFunction);
            return table;
        }

        public virtual TableFunction VisitTableFunction(TableFunction tf)
        {
            if (tf == null)
                return null;
            foreach (TableFunctionParameter parameter in tf.Arguments)
            {
                VisitTableFunctionParameter(parameter);
            }
            VisitTableFunctionReturn(tf.Return);
            return tf;
        }

        public virtual TableFunctionParameter VisitTableFunctionParameter(TableFunctionParameter parameter)
        {
            return parameter;
        }

        public virtual TableFunctionReturn VisitTableFunctionReturn(TableFunctionReturn tfr)
        {
            return tfr;
        }

        public virtual Type VisitType(Type type)
        {
            if (type == null)
                return null;
            foreach (Column column in type.Columns)
            {
                VisitColumn(column);
            }
            foreach (Association association in type.Associations)
            {
                VisitAssociation(association);
            }
            foreach (Type type2 in type.SubTypes)
            {
                VisitType(type2);
            }
            return type;
        }
    }
}