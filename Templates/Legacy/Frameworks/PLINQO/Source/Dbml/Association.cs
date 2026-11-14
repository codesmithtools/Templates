namespace LinqToSqlShared.DbmlObjectModel
{
    public class Association : Node
    {
        private string name;
        public string OtherKey { get; set; }
        public string ThisKey { get; set; }

        public Association(string name)
        {
            if (name == null)
                throw Error.SchemaRequirementViolation("Association", "Name");
            this.name = name;
        }

        public AccessModifier? AccessModifier { get; set; }

        public Cardinality? Cardinality { get; set; }

        public bool? DeleteOnNull { get; set; }

        public string DeleteRule { get; set; }

        public bool? IsForeignKey { get; set; }

        public string Member { get; set; }

        public MemberModifier? Modifier { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw Error.SchemaRequirementViolation("Association", "Name");
                name = value;
            }
        }

        public string Storage { get; set; }

        public string Type { get; set; }

        public string[] GetOtherKey()
        {
            return Dbml.ParseKeyField(OtherKey);
        }

        public string[] GetThisKey()
        {
            return Dbml.ParseKeyField(ThisKey);
        }

        public void SetOtherKey(string[] columnNames)
        {
            OtherKey = Dbml.BuildKeyField(columnNames);
        }

        public void SetThisKey(string[] columnNames)
        {
            ThisKey = Dbml.BuildKeyField(columnNames);
        }

        public override string ToString()
        {
            return (KeyValue("Name", Name) + KeyValue("Type", Type) + KeyValue("Member", Member) +
                    KeyValue("ThisKey", ThisKey) + KeyValue("OtherKey", OtherKey) +
                    KeyValue("IsForeignKey", IsForeignKey) + KeyValue("Cardinality", Cardinality) +
                    KeyValue("DeleteOnNull", DeleteOnNull) + KeyValue("DeleteRule", DeleteRule));
        }

        public AssociationKey ToKey()
        {
            return new AssociationKey(Name, IsForeignKey.HasValue ? IsForeignKey.Value : false);
        }

        public AssociationKey ToOtherKey()
        {
            return new AssociationKey(Name, IsForeignKey.HasValue ? !IsForeignKey.Value : false);
        }
    }
}