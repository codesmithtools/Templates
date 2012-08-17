using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public class TableFunction : Node
    {
        private readonly TableFunctionParameterCollection arguments = new TableFunctionParameterCollection();

        public AccessModifier? AccessModifier { get; set; }

        public TableFunctionParameterCollection Arguments
        {
            get { return arguments; }
        }

        public Function MappedFunction { get; set; }

        public TableFunctionReturn Return { get; set; }
    }
}