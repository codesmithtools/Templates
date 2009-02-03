using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LinqToSqlShared.DbmlObjectModel
{
    [DebuggerDisplay("Name = {Name}")]
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
                if (Array.Exists(members, s => c.Member.Equals(s)))
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

        public bool IsManyToMany()
        {
            // 1) Table must have Two ForeignKeys.
            // 2) All columns must be either...
            //    a) Member of the Primary Key.
            //    b) Member of a Foreign Key.
            //    c) A DateTime stamp (CreateDate, EditDate, etc).

            // has to be at least 2 columns
            if (Columns.Count < 2)
                return false;

            List<Column> fColumns = new List<Column>();
            
            // can only be 2 fkeys
            List<Association> fk = new List<Association>(EntityRefAssociations);
            if (fk.Count != 2)
                return false;

            // find all fkey columns
            foreach (Association a in fk)
                foreach (string key in a.GetThisKey())
                    if (Columns.Contains(key))
                        fColumns.Add(Columns[key]);

            // check all columns
            foreach (var c in Columns)
                if (c.IsPrimaryKey == false
                    && c.IsDbGenerated == false
                    && c.IsVersion == false
                    && c.IsReadOnly == false
                    && string.IsNullOrEmpty(c.Expression)
                    && !fColumns.Contains(c))
                    return false;

            return true;
        }

        public bool IsUniqueMember(string member)
        {
            foreach (var column in columns)
                if (column.Member == member)
                    return false;

            foreach (var association in Associations)
                if (association.Member == member)
                    return false;
            
            return true;
        }
    }
}