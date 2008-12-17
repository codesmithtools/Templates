using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class EntityAssociation
    {
        #region Declarations

        private TableSchema _table;
        private MemberColumnSchema _mcs;
        private bool _usePluralNames;

        #endregion

        #region Constructor

        public EntityAssociation(AssociationTypeEnum associationType, TableSchema table, MemberColumnSchema mcs, bool usePluralNames)
        {
            AssociationType = associationType;

            _table = table;
            _mcs = mcs;
            _usePluralNames = usePluralNames;

            SetVariableNames(true);

            ColumnName = mcs.Name;
            TableName = mcs.Table.FullName;
            ClassName = NHibernateHelper.GetClassName(table);
        }

        #endregion

        #region Methods

        public void SetVariableNames(bool useTable)
        {
            if (_usePluralNames)
            {
                if (useTable)
                {
                    PropertyName = NHibernateHelper.GetPropertyNamePlural(_table, _mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableNamePlural(_table, _mcs);
                    VariableName = NHibernateHelper.GetVariableNamePlural(_table, _mcs);
                }
                else
                {
                    PropertyName = NHibernateHelper.GetPropertyNamePlural(_mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableNamePlural(_mcs);
                    VariableName = NHibernateHelper.GetVariableNamePlural(_mcs);
                }
            }
            else
            {
                if (useTable)
                {
                    PropertyName = NHibernateHelper.GetPropertyName(_table, _mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableName(_table, _mcs);
                    VariableName = NHibernateHelper.GetVariableName(_table, _mcs);
                }
                else
                {
                    PropertyName = NHibernateHelper.GetPropertyName(_mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableName(_mcs);
                    VariableName = NHibernateHelper.GetVariableName(_mcs);
                }
            }
        }

        #endregion

        #region Properties

        public AssociationTypeEnum AssociationType { get; private set; }
        public string PropertyName { get; private set; }
        public string PrivateVariableName { get; private set; }
        public string VariableName { get; private set; }
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public string ClassName { get; private set; }

        public string ToManyTableKeyName { get; set; }
        public string Cascade { get; set; }

        #endregion
    }
}
