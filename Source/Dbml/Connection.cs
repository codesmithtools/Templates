namespace LinqToSqlShared.DbmlObjectModel
{
    public class Connection : Node
    {
        private string provider;

        public Connection(string provider)
        {
            if (provider == null)
            {
                throw Error.SchemaRequirementViolation("Connection", "Provider");
            }
            this.provider = provider;
        }

        public string ConnectionString { get; set; }

        public ConnectionMode? Mode { get; set; }

        public string Provider
        {
            get { return provider; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Connection", "Provider");
                }
                provider = value;
            }
        }

        public string SettingsObjectName { get; set; }

        public string SettingsPropertyName { get; set; }
    }
}