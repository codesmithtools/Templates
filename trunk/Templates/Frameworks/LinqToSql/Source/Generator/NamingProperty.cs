using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CodeSmith.Engine;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace LinqToSqlShared.Generator
{
    [Serializable, PropertySerializer(typeof(XmlPropertySerializer))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NamingProperty
    {
        public NamingProperty() { }

        private bool _disableRenaming = false;
        [NotifyParentProperty(true), Description("Disable the automatic renaming.of class, property and function names.")]
        public bool DisableRenaming
        {
            get { return _disableRenaming; }
            set { _disableRenaming = value; }
        }

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

        private AssociationNamingEnum _associationNaming = AssociationNamingEnum.ListSuffix;
        [NotifyParentProperty(true), Description("Desired association naming convention to be used by generator.")]
        public AssociationNamingEnum AssociationNaming
        {
            get { return _associationNaming; }
            set { _associationNaming = value; }
        }

        public override string ToString()
        {
            return "(Expand to edit...)";
        }
    }
}
