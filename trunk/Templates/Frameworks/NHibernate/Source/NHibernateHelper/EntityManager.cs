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

        private TableSchema _sourceTable;
        private TableSchemaCollection _excludedTables;
        
        private bool _initialized = false;
        
        #endregion

        #region Constructor

        public EntityManager(TableSchema sourceTable, TableSchemaCollection excludedTables)
        {
            this._sourceTable = sourceTable;
            this._excludedTables = excludedTables;
        }

        #endregion

        #region Init Methods

        protected void Init()
        {
            if (!_initialized)
            {
                _initialized = true;

                // Create Dictionaries
                _associationMap = new Dictionary<ColumnSchema, EntityAssociation>();
                _memberMap = new Dictionary<ColumnSchema, EntityMember>();

                // Get Primary Key & Member Columns
                GetPrimaryKey();
                GetMembers(_sourceTable.NonKeyColumns);

                // Association 1) Get all associations.
                GetManyToOne();
                GetToMany();

                // Association 2) Update to try and prevent duplicate names.
                UpdateDuplicateProperties();
            }
        }

        protected void GetPrimaryKey()
        {
            _primaryKey = new EntityKey(_sourceTable);

            foreach (EntityMember em in _primaryKey.KeyColumns)
                _memberMap.Add(em.Column, em);
        }
        protected void GetMembers(ColumnSchemaCollection columns)
        {
            foreach (ColumnSchema column in columns)
                if (!_memberMap.ContainsKey(column))
                {
                    EntityMember em = new EntityMember(column);
                    _memberMap.Add(column, em);
                }

            if (_memberMap.Values.Where(em => em.IsVersion).Count() > 1)
                throw new Exception(String.Format("More than one Version column in {0}", this._sourceTable.FullName));
        }

        protected void UpdateDuplicateProperties()
        {
            IEnumerable<EntityBase> entities = MemberMap.Values.Cast<EntityBase>().Union(AssociationMap.Values.Cast<EntityBase>());
            foreach (EntityBase entity in entities)
            {
                List<EntityBase> duplicateEntities = entities.Where(e => e.GenericName == entity.GenericName).ToList();
                if (duplicateEntities.Count > 1)
                    for (int x = 0; x < duplicateEntities.Count(); x++)
                        duplicateEntities[x].AppendNameSuffix(x + 1);
            }
        }

        #endregion

        #region Association Methods

        protected void GetManyToOne()
        {
            foreach (TableKeySchema tks in _sourceTable.ForeignKeys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1)
                {
                    GetMembers(_sourceTable.ForeignKeyColumns);
                }
                else
                {
                    ColumnSchema column = tks.ForeignKeyMemberColumns[0];

                    if (!_excludedTables.Contains(tks.PrimaryKeyTable)
                    && !column.IsPrimaryKeyMember
                    && !_associationMap.ContainsKey(column))
                    {
                        EntityAssociation association = new EntityAssociation(AssociationTypeEnum.ManyToOne, tks.PrimaryKeyTable, column);
                        _associationMap.Add(column, association);
                    }
                }
            }
        }
        protected void GetToMany()
        {
            foreach (TableKeySchema tks in _sourceTable.PrimaryKeys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1)
                {
                    GetMembers(_sourceTable.ForeignKeyColumns);
                }
                else
                {
                    ColumnSchema column = tks.ForeignKeyMemberColumns[0];

                    if (!column.IsPrimaryKeyMember && !_associationMap.ContainsKey(column))
                    {
                        if (!NHibernateHelper.IsManyToMany(column.Table))
                        {
                            if (!_excludedTables.Contains(column.Table))
                            {
                                EntityAssociation association = new EntityAssociation(AssociationTypeEnum.OneToMany, column.Table, column);
                                _associationMap.Add(column, association);
                            }
                        }
                        else
                        {
                            TableSchema foreignTable = NHibernateHelper.GetToManyTable(column.Table, _sourceTable);
                            if (!_excludedTables.Contains(foreignTable))
                            {
                                EntityAssociation association = new EntityAssociation(AssociationTypeEnum.ManyToMany, foreignTable, column);
                                _associationMap.Add(column, association);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public EntityBase GetEntityBaseFromColumn(ColumnSchema column)
        {
            if(MemberMap.ContainsKey(column))
                return MemberMap[column];
            else if(AssociationMap.ContainsKey(column))
                return AssociationMap[column];
            else
                return null;
        }

        public EntityAssociation GetAssocitionFromColumn(ColumnSchema column)
        {
            return (AssociationMap.ContainsKey(column))
                ? AssociationMap[column]
                : null;
        }
        public EntityMember GetMemberFromColumn(ColumnSchema column)
        {
            return (MemberMap.ContainsKey(column))
                ? MemberMap[column]
                : null;
        }

        #endregion

        #region Properties

        private Dictionary<ColumnSchema, EntityAssociation> _associationMap = null;
        private Dictionary<ColumnSchema, EntityAssociation> AssociationMap
        {
            get
            {
                if (_associationMap == null)
                    Init();
                return _associationMap;
            }
        }

        private Dictionary<ColumnSchema, EntityMember> _memberMap = null;
        private Dictionary<ColumnSchema, EntityMember> MemberMap
        {
            get
            {
                if (_memberMap == null)
                    Init();
                return _memberMap;
            }
        }

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

        public List<EntityMember> Members
        {
            get
            {
                return MemberMap.Values
                    .Where(m => !m.IsPrimaryKeyMember)
                    .ToList();
            }
        }

        public List<EntityAssociation> ManyToOne
        {
            get
            {
                return AssociationMap.Values
                   .Where(a => a.AssociationType == AssociationTypeEnum.ManyToOne)
                   .ToList();
            }
        }

        public List<EntityAssociation> OneToMany
        {
            get
            {
                return AssociationMap.Values
                   .Where(a => a.AssociationType == AssociationTypeEnum.OneToMany)
                   .ToList();
            }
        }

        public List<EntityAssociation> ManyToMany
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationTypeEnum.ManyToMany)
                    .ToList();
            }
        }

        public List<EntityAssociation> ToManyUnion
        {
            get
            {
                return AssociationMap.Values
                    .Where(a =>
                        a.AssociationType == AssociationTypeEnum.ManyToMany
                        || a.AssociationType == AssociationTypeEnum.OneToMany)
                    .ToList();
            }
        }

        public List<EntityMember> MembersPrimaryKeyUnion
        {
            get { return MemberMap.Values.ToList(); }
        }

        #endregion
    }
}
