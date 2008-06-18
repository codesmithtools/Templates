using System;
using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public class Type : Node
    {
        private readonly AssociationCollection associations;
        private readonly ColumnCollection columns;
        private readonly TypeCollection subTypes;
        private string name;

        public Type(string name)
        {
            if (name == null)
            {
                throw Error.SchemaRequirementViolation("Type", "Name");
            }
            this.name = name;
            columns = new ColumnCollection();
            associations = new AssociationCollection();
            subTypes = new TypeCollection();
        }

        public AccessModifier? AccessModifier { get; set; }

        public AssociationCollection Associations
        {
            get { return associations; }
        }

        public ColumnCollection Columns
        {
            get { return columns; }
        }

        public string InheritanceCode { get; set; }

        public bool? IsInheritanceDefault { get; set; }

        public ClassModifier? Modifier { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                {
                    throw Error.SchemaRequirementViolation("Type", "Name");
                }
                name = value;
            }
        }

        public TypeCollection SubTypes
        {
            get { return subTypes; }
        }

        public override string ToString()
        {
            return (KeyValue("Name", Name) + KeyValue("AccessModifier", AccessModifier) + KeyValue("Modifier", Modifier) +
                    KeyValue("Columns", Columns.Count) + KeyValue("Associations", Associations.Count) +
                    KeyValue("SubTypes", SubTypes.Count) + KeyValue("IsInheritanceDefault", IsInheritanceDefault) +
                    KeyValue("InheritanceCode", InheritanceCode));
        }

        public IEnumerable<Association> EntityRefAssociations
        {
            get
            {
                foreach (Association a in Associations)
                {
                    if (a.IsForeignKey == true || a.Cardinality == Cardinality.One)
                        yield return a;
                }
            }
        }

        public IEnumerable<Association> EntitySetAssociations
        {
            get
            {
                foreach (Association a in Associations)
                {
                    if ((!a.IsForeignKey.HasValue || a.IsForeignKey == false) && a.Cardinality == Cardinality.Many)
                        yield return a;
                }
            }
        }

        public IEnumerable<Column> PrimaryKeyColumns
        {
            get
            {
                foreach (Column c in Columns)
                {
                    if (c.IsPrimaryKey == true)
                        yield return c;
                }
            }
        }

        public Column GetColumnByMember(string member)
        {
            if (string.IsNullOrEmpty(member))
                return null;

            foreach (Column c in Columns)
                if (c.Member.Equals(member))
                    return c;

            return null;
        }

        public List<Column> GetColumnsByMembers(string[] members)
        {
            List<Column> cols = new List<Column>();

            if (members == null || members.Length == 0)
                return cols;

            foreach (Column c in Columns)
                if (Array.Exists(members, delegate(string s) { return c.Member.Equals(s); }))
                    cols.Add(c);

            return cols;
        }

        public Association GetForeignKeyAssociation(Column c)
        {
            foreach (Association a in Associations)
                if (a.IsForeignKey == true && a.ThisKey.Equals(c.Member))
                    return a;

            return null;
        }
    }
}