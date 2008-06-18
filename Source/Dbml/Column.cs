namespace LinqToSqlShared.DbmlObjectModel
{
    public class Column : Node
    {
        private string type;

        public Column(string clrType)
        {
            if (clrType == null)
            {
                throw Error.SchemaRequirementViolation("Column", "Type");
            }
            type = clrType;
        }

        public AccessModifier? AccessModifier { get; set; }

        public AutoSync? AutoSync { get; set; }

        public bool? CanBeNull { get; set; }

        public string DbType { get; set; }

        public string Expression { get; set; }

        public bool? IsDbGenerated { get; set; }

        public bool? IsDelayLoaded { get; set; }

        public bool? IsDiscriminator { get; set; }

        public bool? IsPrimaryKey { get; set; }

        public bool? IsReadOnly { get; set; }

        public bool? IsVersion { get; set; }

        public string Member { get; set; }

        public MemberModifier? Modifier { get; set; }

        public string Name { get; set; }

        public string Storage { get; set; }

        public string Type
        {
            get { return type; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Column", "Type");
                }
                type = value;
            }
        }

        public UpdateCheck? UpdateCheck { get; set; }

        public override string ToString()
        {
            return (SingleValue(AccessModifier) + SingleValue(Modifier) + SingleValue(Type) + SingleValue(Member) +
                    KeyValue("Name", Name) + KeyValue("DbType", DbType) + KeyValue("Storage", Storage) +
                    KeyValue("AutoSync", AutoSync) + KeyValue("CanBeNull", CanBeNull) +
                    KeyValue("Expression", Expression) + KeyValue("IsDbGenerated", IsDbGenerated) +
                    KeyValue("IsDelayLoaded", IsDelayLoaded) + KeyValue("IsDiscriminator", IsDiscriminator) +
                    KeyValue("IsPrimaryKey", IsPrimaryKey) + KeyValue("IsReadOnly", IsReadOnly) +
                    KeyValue("IsVersion", IsVersion) + KeyValue("UpdateCheck", UpdateCheck));
        }
    }
}