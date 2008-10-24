using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class AssociationManager
    {
        #region Declarations

        protected TableSchema sourceTable;
        protected TableSchemaCollection excludedTables;
        protected Dictionary<MemberColumnSchema, Association> columnMap;
        private bool initialized = false;
        
        #endregion

        #region Constructor

        public AssociationManager(TableSchema sourceTable, TableSchemaCollection excludedTables)
        {
            this.sourceTable = sourceTable;
            this.excludedTables = excludedTables;
        }

        #endregion

        #region Init Methods

        protected void Init()
        {
            if (!initialized)
            {
                columnMap = new Dictionary<MemberColumnSchema, Association>();

                GetManyToOne();
                GetToMany();
                GetToManyUnion();

                UpdateDuplicateProperties();
            }
        }

        protected void UpdateDuplicateProperties()
        {
            Dictionary<string, List<Association>> names = CreateNamesDictionary();
            foreach (List<Association> associationList in names.Values)
                if (associationList.Count > 1)
                    foreach (Association association in associationList)
                        association.SetVariableNames(false);
        }
        protected Dictionary<string, List<Association>> CreateNamesDictionary()
        {
            Dictionary<string, List<Association>> names = new Dictionary<string, List<Association>>();

            AddToDictionary(names, _manyToOne);
            AddToDictionary(names, _oneToMany);
            AddToDictionary(names, _manyToMany);

            return names;
        }
        protected void AddToDictionary(Dictionary<string, List<Association>> names, List<Association> associations)
        {
            foreach (Association association in associations)
            {
                if (!names.ContainsKey(association.PropertyName))
                    names.Add(association.PropertyName, new List<Association>());

                names[association.PropertyName].Add(association);
            }
        }

        #endregion

        #region Association Methods

        protected void GetManyToOne()
        {
            _manyToOne = new List<Association>();

            foreach (TableKeySchema tks in sourceTable.ForeignKeys)
            {
                MemberColumnSchema mcs = tks.ForeignKeyMemberColumns[0];

                if (!excludedTables.Contains(tks.PrimaryKeyTable) && !mcs.IsPrimaryKeyMember && !columnMap.ContainsKey(mcs))
                {
                    Association association = new Association(AssociationTypeEnum.ManyToOne, tks.PrimaryKeyTable, mcs, false);
                    _manyToOne.Add(association);
                    columnMap.Add(mcs, association);
                }
            }
        }
        protected void GetToMany()
        {
            _oneToMany = new List<Association>();
            _manyToMany = new List<Association>();

            foreach (TableKeySchema tks in sourceTable.PrimaryKeys)
            {
                MemberColumnSchema mcs = tks.ForeignKeyMemberColumns[0];

                if (!mcs.IsPrimaryKeyMember && !columnMap.ContainsKey(mcs))
                {
                    if (!NHibernateHelper.IsManyToMany(mcs.Table))
                    {
                        if (!excludedTables.Contains(mcs.Table))
                        {
                            Association association = new Association(AssociationTypeEnum.OneToMany, mcs.Table, mcs, true)
                            {
                                Cascade = NHibernateHelper.GetCascade(mcs)
                            };
                            _oneToMany.Add(association);
                            columnMap.Add(mcs, association);
                        }
                    }
                    else
                    {
                        TableSchema foreignTable = NHibernateHelper.GetToManyTable(mcs.Table, sourceTable);
                        if (!excludedTables.Contains(foreignTable))
                        {
                            Association association = new Association(AssociationTypeEnum.ManyToMany, foreignTable, mcs, true)
                            {
                                ToManyTableKeyName = NHibernateHelper.GetToManyTableKey(mcs.Table, foreignTable).Name
                            };
                            _manyToMany.Add(association);
                            columnMap.Add(mcs, association);
                        }
                    }
                }
            }
        }
        protected void GetToManyUnion()
        {
            _toManyUnion = new List<Association>();
            _toManyUnion.AddRange(OneToMany);
            _toManyUnion.AddRange(ManyToMany);
        }

        #endregion

        #region Public Methods

        public Association GetAssocitionFromColumn(MemberColumnSchema column)
        {
            if (columnMap == null)
                Init();

            return (columnMap.ContainsKey(column))
                ? columnMap[column]
                : null;
        }

        #endregion

        #region Properties

        private List<Association> _manyToOne = null;
        public List<Association> ManyToOne
        {
            get
            {
                if (_manyToOne == null)
                    Init();
                return _manyToOne;
            }
        }

        private List<Association> _oneToMany = null;
        public List<Association> OneToMany
        {
            get
            {
                if (_oneToMany == null)
                    Init();
                return _oneToMany;
            }
        }

        private List<Association> _manyToMany = null;
        public List<Association> ManyToMany
        {
            get
            {
                if (_manyToMany == null)
                    Init();
                return _manyToMany;
            }
        }

        private List<Association> _toManyUnion = null;
        public List<Association> ToManyUnion
        {
            get
            {
                if (_toManyUnion == null)
                    Init();
                return _toManyUnion;
            }
        }

        #endregion
    }
}
