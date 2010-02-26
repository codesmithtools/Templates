using System;
using System.Collections.Generic;
using System.Linq;

using SchemaExplorer;
using System.Diagnostics;

namespace CodeSmith.SchemaHelper
{
    public class Entity
    {
        #region Private Member(s)

        private bool _initialized;
        private Dictionary<string, Association> _associationMap;
        private Dictionary<string, Member> _memberMap;
        private Dictionary<string, Association> _fkRemoteMemberMap;
        private EntityKey _primaryKey;
        private List<SearchCriteria> _searchCriteria;

        #endregion

        #region Constructor(s)

        public Entity(TableSchema sourceTable)
        {
            Table = sourceTable;
            Description = sourceTable.ResolveDescription();
            ClassName = sourceTable.ClassName();
        }

        #endregion

        #region Private Method(s)

        private void Init()
        {
            if (!_initialized)
            {

                // Create Dictionaries
                _associationMap = new Dictionary<string, Association>();
                _memberMap = new Dictionary<string, Member>();
                _fkRemoteMemberMap = new Dictionary<string, Association>();

                // Get Primary Key & Member Columns
                GetPrimaryKey();
                GetAllMembers();

                // Get all associations.
                GetParentAssociations();
                GetChildAssociations();

                // Update to prevent duplicate names.
                UpdateDuplicateProperties();

                // Get SearchCriteria
                GetSearchCriteria();
                UpdateDuplicateAssociations();

                _initialized = true;
            }
        }

        /// <summary>
        /// Sets the MemberMap for all columns
        /// </summary>
        private void GetAllMembers()
        {
            foreach (ColumnSchema column in Table.Columns)
            {
                if (!_memberMap.ContainsKey(column.Name))
                {
                    Member em = new Member(column, this);
                    _memberMap.Add(column.Name, em);
                }
            }

            if (_memberMap.Values.Where(em => em.IsRowVersion).Count() > 1)
                throw new Exception(String.Format("More than one Version column in {0}", Table.FullName));
        }

        private void GetPrimaryKey()
        {
            _primaryKey = new EntityKey(Table, this);
        }

        private void GetParentAssociations()
        {
            foreach (TableKeySchema tks in Table.ForeignKeys)
            {
                Association association = new Association();

                for (int index = 0; index < tks.ForeignKeyMemberColumns.Count; index++)
                {
                    ColumnSchema column = tks.PrimaryKeyMemberColumns[index];
                    ColumnSchema localColumn = tks.ForeignKeyMemberColumns[index];
                    
                    if (!Configuration.Instance.ExcludeTableRegexIsMatch(tks.PrimaryKeyTable.FullName) && 
                        (!localColumn.IsPrimaryKeyMember || (localColumn.IsPrimaryKeyMember && localColumn.IsForeignKeyMember)))
                    {
                        AssociationMember member = new AssociationMember(AssociationType.ManyToOne, tks.PrimaryKeyTable, column, localColumn, this);
                        if(!association.ContainsKey(member.Key))
                        {
                            association.Add(member);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(association.Key) && !_associationMap.ContainsKey(association.Key))
                    _associationMap.Add(association.Key, association);

                if (!string.IsNullOrEmpty(association.Key) && !_fkRemoteMemberMap.ContainsKey(association.Key))
                    _fkRemoteMemberMap.Add(association.Key, association);
            }
        }

        private void GetChildAssociations()
        {
            foreach (TableKeySchema tks in Table.PrimaryKeys)
            {
                Association association = new Association();

                for (int index = 0; index < tks.ForeignKeyMemberColumns.Count; index++)
                {
                    ColumnSchema column = tks.ForeignKeyMemberColumns[index];
                    ColumnSchema localColumn = tks.PrimaryKeyMemberColumns[index];

                    //Added a check to see if the FK is also a Foreign composite key (http://community.codesmithtools.com/forums/t/10266.aspx).
                    bool isFKAlsoComposite = column.Table.PrimaryKey.MemberColumns.Count > 1 && column.IsPrimaryKeyMember && column.IsForeignKeyMember;
                    if (!Configuration.Instance.ExcludeTableRegexIsMatch(column.Table.FullName) && (!column.IsPrimaryKeyMember || isFKAlsoComposite))
                    {
                        if (!column.Table.IsManyToMany())
                        {
                            if (!Configuration.Instance.ExcludeTableRegexIsMatch(column.Table.FullName))
                            {
                                association.Add(new AssociationMember(AssociationType.OneToMany, column.Table, column, localColumn, this));
                            }
                        }
                        else
                        {
                            TableSchema foreignTable = GetToManyTable(column.Table, Table);
                            if (foreignTable != null && !Configuration.Instance.ExcludeTableRegexIsMatch(foreignTable.FullName))
                            {
                                association.Add(new AssociationMember(AssociationType.ManyToMany, foreignTable, column, localColumn, this));
                            }
                        }
                    }
                    else if (GetToManyTable(column.Table, Table) == null)
                    {
                        if (!Configuration.Instance.ExcludeTableRegexIsMatch(column.Table.FullName))
                        {
                            association.Add(new AssociationMember(AssociationType.OneToZeroOrOne, column.Table, column, localColumn, this));
                        }
                    }
                } //End For Index in ForeignKeyMemberColumns


                if (!string.IsNullOrEmpty(association.Key) && !_associationMap.ContainsKey(association.Key))
                    _associationMap.Add(association.Key, association);
                if (!string.IsNullOrEmpty(association.Key) && !_fkRemoteMemberMap.ContainsKey(association.Key))
                    _fkRemoteMemberMap.Add(association.Key, association);

            } //End Table.PrimaryKeys
        }

        private void UpdateDuplicateProperties()
        {
            IEnumerable<Member> entities = MemberMap.Values.ToList();
            foreach (Member entity in entities)
            {
                Member entity1 = entity;
                List<Member> duplicateMembers = entities.Where(e => e.Name == entity1.Name).ToList();
                if (duplicateMembers.Count > 1)
                    for (int x = 0; x < duplicateMembers.Count(); x++)
                        duplicateMembers[x].AppendNameSuffix(x + 1);
            }
        }

        private void UpdateDuplicateAssociations()
        {
            IEnumerable<Association> associations = _associationMap.Values.ToList();
            foreach (Association association in associations)
            {
                Association association1 = association;
                List<Association> duplicateAssociations = associations.Where(a => a.PropertyName == association1.PropertyName).ToList();
                if (duplicateAssociations.Count > 1)
                {
                    for (int index = 0; index < duplicateAssociations.Count(); index++)
                    {
                        if (duplicateAssociations[index].SearchCriteria != null)
                        {
                            duplicateAssociations[index].PropertyName += "By" + duplicateAssociations[index].MembersToString;
                        }
                    }
                }
            }
        }

        #region Many To Many Methods

        private static TableSchema GetToManyTable(TableSchema manyToTable, TableSchema sourceTable)
        {
            TableSchema result = null;
            foreach (TableKeySchema key in manyToTable.ForeignKeys)
                if (!key.PrimaryKeyTable.Equals(sourceTable))
                {
                    result = key.PrimaryKeyTable;
                    break;
                }
            return result;
        }

        #endregion

        #region Search Criteria Methods

        private void GetSearchCriteria()
        {
            Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

            switch (Configuration.Instance.SearchCriteriaProperty.SearchCriteria)
            {
                case SearchCriteriaEnum.All:
                    AddPrimaryKeySearchCriteria(map);
                    AddIndexSearchCriteria(map);
                    AddForeignKeySearchCriteria(map);
                    break;
                case SearchCriteriaEnum.ForeignKey:
                    AddForeignKeySearchCriteria(map);
                    break;
                case SearchCriteriaEnum.Index:
                    AddIndexSearchCriteria(map);
                    break;
                case SearchCriteriaEnum.PrimaryKey:
                    AddPrimaryKeySearchCriteria(map);
                    break;
                case SearchCriteriaEnum.NoForeignKeys:
                    AddIndexSearchCriteria(map);
                    AddPrimaryKeySearchCriteria(map);
                    break;
            }

            _searchCriteria = map.Values.ToList();
        }

        private void AddPrimaryKeySearchCriteria(IDictionary<string, SearchCriteria> map)
        {
            SearchCriteria searchCriteria = new SearchCriteria(SearchCriteriaEnum.PrimaryKey);

            foreach (MemberColumnSchema column in Table.PrimaryKey.MemberColumns)
            {
                if (column.Table.Equals(Table))
                {
                    Member member = GetFromColumn(column);
                    if (member != null)
                        searchCriteria.Members.Add(GetFromColumn(column));
                }
            }

            searchCriteria.IsUniqueResult = true;

            AddToSearchCriteria(map, searchCriteria);
        }

        private void AddForeignKeySearchCriteria(IDictionary<string, SearchCriteria> map)
        {
            foreach (Association association in AssociatedForeignKeys)
            {
                //Only adding Parent Associations
                SearchCriteria searchCriteria = new SearchCriteria(SearchCriteriaEnum.ForeignKey);
                SearchCriteria childSearchCriteria = new SearchCriteria(SearchCriteriaEnum.ForeignKey, true);
                foreach (AssociationMember member in association)
                {
                    //Validate that the tables are the same
                    //if (member.Table.Equals(Table))
                    //{
                    searchCriteria.AssociationMembers.Add(member);
                    searchCriteria.Members.Add(member.AssociatedColumn);
                    childSearchCriteria.AssociationMembers.Add(member);
                    childSearchCriteria.Members.Add(member.AssociatedColumn);
                    //}
                }

                if (association.AssociationType == AssociationType.ManyToOne)
                {
                    AddToSearchCriteria(map, searchCriteria);
                }

                association.SearchCriteria = childSearchCriteria;
            }

        }

        private void AddIndexSearchCriteria(IDictionary<string, SearchCriteria> map)
        {
            foreach (IndexSchema indexSchema in Table.Indexes)
            {
                SearchCriteria searchCriteria = new SearchCriteria(SearchCriteriaEnum.Index);

                foreach (MemberColumnSchema column in indexSchema.MemberColumns)
                {
                    if (column.Table.Equals(Table))
                    {
                        Member member = GetFromColumn(column.Column);
                        if (member != null)
                            searchCriteria.Members.Add(member);
                    }
                }

                if (indexSchema.IsUnique)
                    searchCriteria.IsUniqueResult = true;

                AddToSearchCriteria(map, searchCriteria);
            }
        }

        private static bool AddToSearchCriteria(IDictionary<string, SearchCriteria> map, SearchCriteria searchCriteria)
        {
            string key = searchCriteria.Key;
            bool result = (!string.IsNullOrEmpty(key) && searchCriteria.Members.Count > 0 && !map.ContainsKey(key));

            if (result)
                map.Add(key, searchCriteria);

            return result;
        }

        #endregion

        #endregion

        #region Public Methods

        internal Member GetFromColumn(ColumnSchema column)
        {
            //Member retVal = null;

            if (MemberMap.ContainsKey(column.Name))
                return MemberMap[column.Name];

            return null;
        }

        #endregion

        #region Public Overridden Methods

        public override string ToString()
        {
            return ClassName;
        }

        #endregion

        #region Public Read-Only Properties

        public TableSchema Table { get; private set; }
        public string ClassName { get; private set; }

        public string Description { get; private set; }
        public bool HasDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(Description))
                    return Description.Trim().Length > 0;

                return false;
            }
        }

        public EntityKey PrimaryKey
        {
            get
            {
                if (_primaryKey == null)
                    Init();
                return _primaryKey;
            }
        }

        public bool HasRowVersionMember
        {
            get { return (RowVersionMember != null); }
        }

        public Member RowVersionMember
        {
            get
            {
                return MemberMap.Values
                    .Where(m => m.IsRowVersion)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Does the Entity have an Identity column
        /// </summary>
        public bool HasIdentityMember
        {
            get { return (IdentityMember != null); }
        }

        /// <summary>
        /// The Identity Column
        /// </summary>
        public Member IdentityMember
        {
            get { return MemberMap.Values.Where(m => m.IsIdentity).FirstOrDefault(); }
        }

        #endregion

        #region Private Properties

        private Dictionary<string, Association> AssociationMap
        {
            get
            {
                if (_associationMap == null)
                    Init();
                return _associationMap;
            }
        }

        private Dictionary<string, Member> MemberMap
        {
            get
            {
                if (_memberMap == null)
                    Init();
                return _memberMap;
            }
        }

        private Dictionary<string, Association> FKRemoteMemberMap
        {
            get
            {
                if (_fkRemoteMemberMap == null)
                    Init();
                return _fkRemoteMemberMap;
            }
        }

        #endregion

        #region Members Logic.

        public List<Member> Members
        {
            get
            {               
                return MemberMap.Values.ToList();
            }
        }

        /// <summary>
        /// Returns a list of Members that are set during and update or insert.
        /// </summary>
        public List<Member> MembersUpdateInsert
        {
            get
            {
                return Members.Where(m => !m.IsIdentity && !m.IsComputed && !m.IsRowVersion).ToList();
            }
        }

        public List<Member> MembersNoRowVersion
        {
            get
            {
                return Members.Where(m => !m.IsRowVersion).ToList();
            }
        }

        public List<Member> MembersNoPrimaryKeys
        {
            get
            {
                return Members.Where(m => !m.IsPrimaryKey).ToList();
            }
        }
        
        public List<Member> MembersNoForeignKeys
        {
            get
            {
                return Members.Where(m => !m.IsForeignKey).ToList();
            }
        }

        public List<Member> MembersNoKeys
        {
            get
            {
                return Members.Where(m => !m.IsForeignKey && !m.IsPrimaryKey).ToList();
            }
        }

        public List<Member> MembersNoKeysOrRowVersion
        {
            get
            {
                return Members
                    .Where(m => !m.IsForeignKey && !m.IsPrimaryKey && !m.IsRowVersion)
                    .ToList();
            }
        }

        public List<Association> AssociatedForeignKeys
        {
            get
            {
                //NOTE: fk.IsPrimaryKey == false is a work around for pks that are also fk's.
                //return FKRemoteMemberMap.Values.Where(fk => fk.TableName == this.Table.Name && fk.IsPrimaryKey == false).ToList();
                return FKRemoteMemberMap.Values.ToList();
            }
        }

        public List<Association> AssociatedManyToOne
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.ManyToOne)
                    .ToList();
            }
        }

        public List<Association> AssociatedOneToZeroOrOne
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.OneToZeroOrOne)
                    .ToList();
            }
        }

        public List<Association> AssociatedOneToMany
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.OneToMany)
                    .ToList();
            }
        }

        public List<Association> AssociatedManyToMany
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.ManyToMany)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns ManyToMany and OneToMany Associations.
        /// </summary>
        public List<Association> AssociatedToManyUnion
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.ManyToMany || a.AssociationType == AssociationType.OneToMany)
                    .ToList();
            }
        }

        public List<SearchCriteria> SearchCriteria()
        {
             return SearchCriteria(SearchCriteriaEnum.All);
        }

        public List<SearchCriteria> SearchCriteria(SearchCriteriaEnum criteria)
        {
            if (_searchCriteria == null)
                Init();

            if(_searchCriteria == null)
                return null;

            if (criteria == SearchCriteriaEnum.All)
                return _searchCriteria;

            if (criteria == SearchCriteriaEnum.ForeignKey)
                return _searchCriteria.Where(sc => sc.SearchCriteriaType == SearchCriteriaEnum.ForeignKey).ToList();

            if (criteria == SearchCriteriaEnum.Index)
                return _searchCriteria.Where(sc => sc.SearchCriteriaType == SearchCriteriaEnum.Index).ToList();

            if (criteria == SearchCriteriaEnum.NoForeignKeys)
                return _searchCriteria.Where(sc => sc.SearchCriteriaType == SearchCriteriaEnum.Index || sc.SearchCriteriaType == SearchCriteriaEnum.PrimaryKey).ToList();

            return _searchCriteria;
        }

        // NOTE: Do we want this here or at all?
        /// <summary>
        /// Gives the entity access to all other entities in the database.
        /// </summary>
        public EntityManager EntityManager
        {
            get
            {
                return new EntityManager(Table.Database);
            }
        }

        #endregion
    }
}