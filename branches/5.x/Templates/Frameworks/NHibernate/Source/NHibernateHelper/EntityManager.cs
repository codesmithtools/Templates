using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class EntityManager
    {
        #region Declarations

        protected TableSchema sourceTable;
        protected TableSchemaCollection excludedTables;
        protected Dictionary<MemberColumnSchema, EntityAssociation> associationMap;
        protected Dictionary<ColumnSchema, EntityMember> memberMap = null;
        
        private bool initialized = false;
        
        #endregion

        #region Constructor

        public EntityManager(TableSchema sourceTable, TableSchemaCollection excludedTables)
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
                // Create Dictionaries
                associationMap = new Dictionary<MemberColumnSchema, EntityAssociation>();
                memberMap = new Dictionary<ColumnSchema, EntityMember>();

                // Get Primary Key & Member Columns
                GetPrimaryKey();
                GetMembers(sourceTable.NonKeyColumns);

                // Association 1) Get all associations.
                GetManyToOne();
                GetToMany();

                // Association 2) Update to try and prevent duplicate names.
                UpdateDuplicateProperties();
            }
        }

        protected void GetPrimaryKey()
        {
            _primaryKey = new EntityKey(sourceTable);

            foreach (EntityMember em in _primaryKey.KeyColumns)
                memberMap.Add(em.Column, em);
        }
        protected void GetMembers(ColumnSchemaCollection columns)
        {
            if (_members == null)
                _members = new List<EntityMember>();

            foreach (ColumnSchema column in columns)
                if (!memberMap.ContainsKey(column))
                {
                    EntityMember em = new EntityMember(column);
                    _members.Add(em);
                    memberMap.Add(column, em);
                }
        }

        protected void UpdateDuplicateProperties()
        {
            Dictionary<string, List<EntityAssociation>> names = CreateNamesDictionary();
            foreach (List<EntityAssociation> associationList in names.Values)
                if (associationList.Count > 1)
                    foreach (EntityAssociation association in associationList)
                        association.SetVariableNames(false);
        }
        protected Dictionary<string, List<EntityAssociation>> CreateNamesDictionary()
        {
            Dictionary<string, List<EntityAssociation>> names = new Dictionary<string, List<EntityAssociation>>();

            AddToDictionary(names, _manyToOne);
            AddToDictionary(names, _oneToMany);
            AddToDictionary(names, _manyToMany);

            return names;
        }
        protected void AddToDictionary(Dictionary<string, List<EntityAssociation>> names, List<EntityAssociation> associations)
        {
            foreach (EntityAssociation association in associations)
            {
                if (!names.ContainsKey(association.PropertyName))
                    names.Add(association.PropertyName, new List<EntityAssociation>());

                names[association.PropertyName].Add(association);
            }
        }

        #endregion

        #region Association Methods

        protected void GetManyToOne()
        {
            _manyToOne = new List<EntityAssociation>();

            foreach (TableKeySchema tks in sourceTable.ForeignKeys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1)
                {
                    GetMembers(sourceTable.ForeignKeyColumns);
                }
                else
                {
                    MemberColumnSchema mcs = tks.ForeignKeyMemberColumns[0];

                    if (!excludedTables.Contains(tks.PrimaryKeyTable)
                    && !mcs.IsPrimaryKeyMember
                    && !associationMap.ContainsKey(mcs))
                    {
                        EntityAssociation association = new EntityAssociation(AssociationTypeEnum.ManyToOne, tks.PrimaryKeyTable, mcs, false);
                        _manyToOne.Add(association);
                        associationMap.Add(mcs, association);
                    }
                }
            }
        }
        protected void GetToMany()
        {
            _oneToMany = new List<EntityAssociation>();
            _manyToMany = new List<EntityAssociation>();

            foreach (TableKeySchema tks in sourceTable.PrimaryKeys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1)
                {
                    GetMembers(sourceTable.ForeignKeyColumns);
                }
                else
                {
                    MemberColumnSchema mcs = tks.ForeignKeyMemberColumns[0];

                    if (!mcs.IsPrimaryKeyMember && !associationMap.ContainsKey(mcs))
                    {
                        if (!NHibernateHelper.IsManyToMany(mcs.Table))
                        {
                            if (!excludedTables.Contains(mcs.Table))
                            {
                                EntityAssociation association = new EntityAssociation(AssociationTypeEnum.OneToMany, mcs.Table, mcs, true)
                                {
                                    Cascade = NHibernateHelper.GetCascade(mcs)
                                };
                                _oneToMany.Add(association);
                                associationMap.Add(mcs, association);
                            }
                        }
                        else
                        {
                            TableSchema foreignTable = NHibernateHelper.GetToManyTable(mcs.Table, sourceTable);
                            if (!excludedTables.Contains(foreignTable))
                            {
                                EntityAssociation association = new EntityAssociation(AssociationTypeEnum.ManyToMany, foreignTable, mcs, true)
                                {
                                    ToManyTableKeyName = NHibernateHelper.GetToManyTableKey(mcs.Table, foreignTable).Name
                                };
                                _manyToMany.Add(association);
                                associationMap.Add(mcs, association);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public EntityAssociation GetAssocitionFromColumn(MemberColumnSchema column)
        {
            if (associationMap == null)
                Init();

            return (associationMap.ContainsKey(column))
                ? associationMap[column]
                : null;
        }
        public EntityMember GetMemberFromColumn(ColumnSchema column)
        {
            if (memberMap == null)
                Init();

            return (memberMap.ContainsKey(column))
                ? memberMap[column]
                : null;
        }

        #endregion

        #region Properties

        private EntityKey _primaryKey = null;
        public EntityKey PrimaryKey
        {
            get
            {
                if (_primaryKey == null)
                    Init();
                return _primaryKey;
            }
        }

        private List<EntityMember> _members = null;
        public List<EntityMember> Members
        {
            get
            {
                if (_members == null)
                    Init();
                return _members;
            }
        }

        private List<EntityAssociation> _manyToOne = null;
        public List<EntityAssociation> ManyToOne
        {
            get
            {
                if (_manyToOne == null)
                    Init();
                return _manyToOne;
            }
        }

        private List<EntityAssociation> _oneToMany = null;
        public List<EntityAssociation> OneToMany
        {
            get
            {
                if (_oneToMany == null)
                    Init();
                return _oneToMany;
            }
        }

        private List<EntityAssociation> _manyToMany = null;
        public List<EntityAssociation> ManyToMany
        {
            get
            {
                if (_manyToMany == null)
                    Init();
                return _manyToMany;
            }
        }

        public List<EntityMember> MembersPrimaryKeyUnion
        {
            get
            {
                List<EntityMember> membersPrimaryKeyUnion = new List<EntityMember>();
                membersPrimaryKeyUnion.AddRange(Members);
                if (PrimaryKey.IsCompositeKey)
                    membersPrimaryKeyUnion.AddRange(PrimaryKey.KeyColumns);
                return membersPrimaryKeyUnion;
            }
        }

        public List<EntityAssociation> ToManyUnion
        {
            get
            {
                List<EntityAssociation> toManyUnion = new List<EntityAssociation>();
                toManyUnion.AddRange(OneToMany);
                toManyUnion.AddRange(ManyToMany);
                return toManyUnion;
            }
        }

        #endregion
    }
}
