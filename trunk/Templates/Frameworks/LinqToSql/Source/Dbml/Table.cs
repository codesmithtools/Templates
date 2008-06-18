namespace LinqToSqlShared.DbmlObjectModel
{
    public class Table : Node
    {
        private string name;
        private Type type;

        public Table(string name, Type type)
        {
            if (name == null)
            {
                throw Error.SchemaRequirementViolation("Table", "Name");
            }
            this.name = name;
            if (type == null)
            {
                throw Error.SchemaRequirementViolation("Table", "Type");
            }
            this.type = type;
        }

        public AccessModifier? AccessModifier { get; set; }

        public TableFunction DeleteFunction { get; set; }

        public TableFunction InsertFunction { get; set; }

        public string Member { get; set; }

        public MemberModifier? Modifier { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Table", "Name");
                }
                name = value;
            }
        }

        public Type Type
        {
            get { return type; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Table", "Type");
                }
                type = value;
            }
        }

        public TableFunction UpdateFunction { get; set; }
    }
}