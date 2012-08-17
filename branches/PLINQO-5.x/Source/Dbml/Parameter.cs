namespace LinqToSqlShared.DbmlObjectModel
{
    public class Parameter : Node
    {
        private string name;
        private string type;

        public Parameter(string name, string type)
        {
            if (name == null)
            {
                throw Error.SchemaRequirementViolation("Parameter", "Name");
            }
            this.name = name;
            if (type == null)
            {
                throw Error.SchemaRequirementViolation("Parameter", "Type");
            }
            this.type = type;
        }

        public string DbType { get; set; }

        public ParameterDirection? Direction { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Parameter", "Name");
                }
                name = value;
            }
        }

        public string ParameterName { get; set; }

        public string Type
        {
            get { return type; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Parameter", "Type");
                }
                type = value;
            }
        }
    }
}