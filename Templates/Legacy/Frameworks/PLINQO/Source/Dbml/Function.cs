using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public class Function : Node
    {
        private readonly ParameterCollection parameters;
        private readonly TypeCollection types;
        private string name;

        public Function(string name)
        {
            if (name == null)
            {
                throw Error.SchemaRequirementViolation("Function", "Name");
            }
            this.name = name;
            parameters = new ParameterCollection();
            types = new TypeCollection();
        }

        public AccessModifier? AccessModifier { get; set; }

        public bool? HasMultipleResults { get; set; }

        public bool? IsComposable { get; set; }

        public string Method { get; set; }

        public MemberModifier? Modifier { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Function", "Name");
                }
                name = value;
            }
        }

        public ParameterCollection Parameters
        {
            get { return parameters; }
        }

        public Return Return { get; set; }

        public TypeCollection Types
        {
            get { return types; }
        }
    }
}