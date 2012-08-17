namespace LinqToSqlShared.DbmlObjectModel
{
    public class TableFunctionParameter : Node
    {
        private string member;
        private string parameterName;

        public TableFunctionParameter(string name, string member)
        {
            if (name == null)
            {
                throw Error.SchemaRequirementViolation("Parameter", "Parameter");
            }
            if (member == null)
            {
                throw Error.SchemaRequirementViolation("Parameter", "Member");
            }
            parameterName = name;
            this.member = member;
        }

        public string Member
        {
            get { return member; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Parameter", "Member");
                }
                member = value;
            }
        }

        public string ParameterName
        {
            get { return parameterName; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Parameter", "Parameter");
                }
                parameterName = value;
            }
        }

        public Version? Version { get; set; }
    }
}