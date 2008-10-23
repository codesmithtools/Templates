using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class Association
    {
        #region Declarations

        private TableSchema _table;
        private MemberColumnSchema _mcs;
        private bool _usePluralNames;

        #endregion

        #region Constructor

        public Association(TableSchema table, MemberColumnSchema mcs, bool usePluralNames)
        {
            _table = table;
            _mcs = mcs;
            _usePluralNames = usePluralNames;

            SetVariableNames(true);

            ColumnName = mcs.Name;
            TableName = mcs.Table.Name;
            ClassName = table.Name;
        }

        #endregion

        #region Methods

        public void SetVariableNames(bool useTable)
        {
            if (_usePluralNames)
            {
                if (useTable)
                {
                    PropertyName = NHibernateHelper.GetPropertyName(_table, _mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableName(_table, _mcs);
                }
                else
                {
                    PropertyName = NHibernateHelper.GetPropertyName(_mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableName(_mcs);
                }
            }
            else
            {
                if (useTable)
                {
                    PropertyName = NHibernateHelper.GetPropertyNamePlural(_table, _mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableNamePlural(_table, _mcs);
                }
                else
                {
                    PropertyName = NHibernateHelper.GetPropertyNamePlural(_mcs);
                    PrivateVariableName = NHibernateHelper.GetPrivateVariableNamePlural(_mcs);
                }
            }
        }

        #endregion

        #region Properties

        public string PropertyName { get; private set; }
        public string PrivateVariableName { get; private set; }
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public string ClassName { get; private set; }

        public string ToManyTableKeyName { get; set; }
        public string Cascade { get; set; }

        #endregion
    }
}
