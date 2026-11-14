namespace LinqToSqlShared.DbmlObjectModel
{
    public class Return : Node
    {
        private string type;

        public Return(string type)
        {
            if (type == null)
            {
                throw Error.SchemaRequirementViolation("Return", "Type");
            }
            this.type = type;
        }

        public string DbType { get; set; }

        public string Type
        {
            get { return type; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Return", "Type");
                }
                type = value;
            }
        }
    }
}