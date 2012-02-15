using System;

using CodeSmith.Engine;

using System.ComponentModel;

namespace CodeSmith.SchemaHelper
{
    [Serializable]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NamingProperty
    {
        #region Private Member(s)

        private TableNaming _tableNaming = SchemaHelper.TableNaming.Singular;
        private ColumnNaming _columnNaming = SchemaHelper.ColumnNaming.RemoveTablePrefix;
        private EntityNaming _entityNaming = SchemaHelper.EntityNaming.Singular;
        private AssociationNaming _associationNaming = SchemaHelper.AssociationNaming.Table;
        private AssociationSuffix _associationSuffix = SchemaHelper.AssociationSuffix.Plural;

        #endregion

        #region Constructor(s)

        public NamingProperty()
        {
        }

        #endregion

        #region Public Overridden Method(s)

        public override string ToString()
        {
            return "(Expand to edit...)";
        }

        #endregion

        #region Public Properties

        [NotifyParentProperty(true)]
        [Description("Table naming convention used in the database.")]
        public TableNaming TableNaming
        {
            get { return _tableNaming; }
            set { _tableNaming = value; }
        }

        [NotifyParentProperty(true)]
        [Description("Desired column naming convention to be used by generator.")]
        public ColumnNaming ColumnNaming
        {
            get { return _columnNaming; }
            set { _columnNaming = value; }
        }

        [NotifyParentProperty(true)]
        [Description("Desired naming convention to be used by generator.")]
        public EntityNaming EntityNaming
        {
            get { return _entityNaming; }
            set { _entityNaming = value; }
        }

        [NotifyParentProperty(true)]
        [Description("Desired association naming convention to be used by generator.")]
        public AssociationNaming AssociationNaming
        {
            get { return _associationNaming; }
            set { _associationNaming = value; }
        }

        [NotifyParentProperty(true)]
        [Description("Desired association suffix naming convention to be used by generator.")]
        public AssociationSuffix AssociationSuffix
        {
            get { return _associationSuffix; }
            set { _associationSuffix = value; }
        }

        #endregion
    }
}