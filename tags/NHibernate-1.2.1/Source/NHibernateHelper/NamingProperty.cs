using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Engine;
using System.ComponentModel;

namespace NHibernateHelper
{
    [Serializable, PropertySerializer(typeof(XmlPropertySerializer))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NamingProperty
    {
        public NamingProperty() { }

        private TableNamingEnum _tableNaming = TableNamingEnum.Singular;
        [NotifyParentProperty(true), Description("Table naming convention used in the database.")]
        public TableNamingEnum TableNaming
        {
            get { return _tableNaming; }
            set { _tableNaming = value; }
        }

        private EntityNamingEnum _entityNaming = EntityNamingEnum.Singular;
        [NotifyParentProperty(true), Description("Desired naming naming convention to be used by generator.")]
        public EntityNamingEnum EntityNaming
        {
            get { return _entityNaming; }
            set { _entityNaming = value; }
        }

        private AssociationNamingEnum _associationNaming = AssociationNamingEnum.Table;
        [NotifyParentProperty(true), Description("Desired association naming convention to be used by generator.")]
        public AssociationNamingEnum AssociationNaming
        {
            get { return _associationNaming; }
            set { _associationNaming = value; }
        }

        private AssociationSuffixEnum _associationSuffix = AssociationSuffixEnum.Plural;
        [NotifyParentProperty(true), Description("Desired association suffix naming convention to be used by generator.")]
        public AssociationSuffixEnum AssociationSuffix
        {
            get { return _associationSuffix; }
            set { _associationSuffix = value; }
        }

        public override string ToString()
        {
            return "(Expand to edit...)";
        }
    }
}
