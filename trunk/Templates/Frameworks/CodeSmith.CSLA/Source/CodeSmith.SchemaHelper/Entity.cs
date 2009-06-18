using System;
using System.Collections.Generic;
using System.Linq;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class Entity
    {
        #region Private Member(s)

        private bool _initialized;
        private Dictionary<string, AssociationMember> _associationMap;
        private Dictionary<string, Member> _memberMap;
        private Dictionary<string, Member> _fkMemberMap;
        private Dictionary<string, AssociationMember> _fkRemoteMemberMap;
        private EntityKey _primaryKey;
        private List<SearchCriteria> _searchCriteria;

        #endregion

        #region Constructor(s)

        public Entity(TableSchema sourceTable)
        {
            Table = sourceTable;
            ClassName = sourceTable.ClassName();
        }

        #endregion

        #region Private Method(s)

        private void Init()
        {
            if (!_initialized)
            {
                // Create Dictionaries
                _associationMap = new Dictionary<string, AssociationMember>();
                _memberMap = new Dictionary<string, Member>();
                _fkMemberMap = new Dictionary<string, Member>();
                _fkRemoteMemberMap = new Dictionary<string, AssociationMember>();

                // Get Primary Key & Member Columns
                GetPrimaryKey();
                GetMembers(Table.NonKeyColumns);

                // Get all associations.
                GetManyToOne();
                GetToMany();

                // Update to prevent duplicate names.
                UpdateDuplicateProperties();

                // Get SearchCriteria
                GetSearchCriteria();

                _initialized = true;
            }
        }

        private void GetPrimaryKey()
        {
            _primaryKey = new EntityKey(Table, this);

            foreach (Member em in _primaryKey.KeyMembers)
                _memberMap.Add(em.ColumnName, em);
        }

        private void GetMembers(ColumnSchemaCollection columns)
        {
            foreach (ColumnSchema column in columns)
                if (!_memberMap.ContainsKey(column.Name))
                {
                    Member em = new Member(column, this);
                    _memberMap.Add(column.Name, em);
                }

            if (_memberMap.Values.Where(em => em.IsRowVersion).Count() > 1)
                throw new Exception(String.Format("More than one Version column in {0}", Table.FullName));
        }

        private void GetManyToOne()
        {
            foreach (TableKeySchema tks in Table.ForeignKeys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1)
                {
                    GetMembers(Table.ForeignKeyColumns);
                }
                else
                {
                    ColumnSchema column = tks.ForeignKeyMemberColumns[0];
                    ColumnSchema localColumn = tks.PrimaryKeyMemberColumns[0];

                    if (!Configuration.Instance.ExcludeTableRegexIsMatch(tks.PrimaryKeyTable.FullName)
                        && !column.IsPrimaryKeyMember
                        && !_associationMap.ContainsKey(column.Name))
                    {
                        AssociationMember association = new AssociationMember(AssociationType.ManyToOne, tks.PrimaryKeyTable, column, localColumn, this);
                        _associationMap.Add(column.Name, association);

                        if (!_fkMemberMap.ContainsKey(column.Name))
                            _fkMemberMap.Add(column.Name, association.LocalColumn);

                        if (!_fkRemoteMemberMap.ContainsKey(column.Name))
                            _fkRemoteMemberMap.Add(column.Name, association);
                    }
                }
            }
        }

        private void GetToMany()
        {
            foreach (TableKeySchema tks in Table.PrimaryKeys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1)
                {
                    GetMembers(Table.ForeignKeyColumns);
                }
                else
                {
                    ColumnSchema column = tks.ForeignKeyMemberColumns[0];

                    if (!column.IsPrimaryKeyMember && !_associationMap.ContainsKey(column.Name))
                    {
                        if(tks.PrimaryKeyMemberColumns.Count > 1)
                            throw new Exception("We do not currently support Composite Keys.");
                        ColumnSchema localColumn = tks.PrimaryKeyMemberColumns[0];

                        if (!column.Table.IsManyToMany())
                        {
                            if (!Configuration.Instance.ExcludeTableRegexIsMatch(column.Table.FullName))
                            {
                                AssociationMember association = new AssociationMember(AssociationType.OneToMany, column.Table, column, localColumn, this);
                                _associationMap.Add(column.Name, association);

                                if (!_fkMemberMap.ContainsKey(column.Name))
                                    _fkMemberMap.Add(column.Name, association.LocalColumn);

                                if (!_fkRemoteMemberMap.ContainsKey(column.Name))
                                    _fkRemoteMemberMap.Add(column.Name, association);
                            }
                        }
                        else
                        {
                            TableSchema foreignTable = GetToManyTable(column.Table, Table);
                            if (!Configuration.Instance.ExcludeTableRegexIsMatch(foreignTable.FullName))
                            {
                                AssociationMember association = new AssociationMember(AssociationType.ManyToMany, foreignTable, column, localColumn, this);
                                _associationMap.Add(column.Name, association);

                                if (!_fkMemberMap.ContainsKey(column.Name))
                                    _fkMemberMap.Add(column.Name, association.LocalColumn);

                                if (!_fkRemoteMemberMap.ContainsKey(column.Name))
                                    _fkRemoteMemberMap.Add(column.Name, association);
                            }
                        }
                    }
                    else if (GetToManyTable(column.Table, Table) == null)
                    {
                        if (!Configuration.Instance.ExcludeTableRegexIsMatch(column.Table.FullName))
                        {
                            AssociationMember association = new AssociationMember(AssociationType.OneToZeroOrOne,
                                                                                  column.Table, column, tks.PrimaryKeyMemberColumns[0],
                                                                                  this);
                            if (_fkMemberMap.ContainsKey(column.Name))
                                _associationMap.Add(column.Table + "-" + column.Name, association);
                            else
                                _associationMap.Add(column.Name, association);

                            if (!_fkMemberMap.ContainsKey(column.Name))
                                _fkMemberMap.Add(column.Name, association.LocalColumn);

                            if (!_fkRemoteMemberMap.ContainsKey(column.Name))
                                _fkRemoteMemberMap.Add(column.Name, association);
                        }

                    }
                }
            }
        }

        private void UpdateDuplicateProperties()
        {
            IEnumerable<MemberBase> entities = MemberMap.Values.Cast<MemberBase>().Union(AssociationMap.Values.Cast<MemberBase>());
            foreach (MemberBase entity in entities)
            {
                MemberBase entity1 = entity;
                List<MemberBase> duplicateMembers = entities.Where(e => e.Name == entity1.Name).ToList();
                if (duplicateMembers.Count > 1)
                    for (int x = 0; x < duplicateMembers.Count(); x++)
                        duplicateMembers[x].AppendNameSuffix(x + 1);
            }
        }

        private void GetSearchCriteria()
        {
            Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

            switch (Configuration.Instance.SearchCriteriaProperty.SearchCriteria)
            {
                case SearchCriteriaEnum.All:
                    AddForeignKeySearchCriteria(map);
                    AddIndexSearchCriteria(map);
                    AddPrimaryKeySearchCriteria(map);
                    break;
                case SearchCriteriaEnum.ForeignKeys:
                    AddForeignKeySearchCriteria(map);
                    break;
                case SearchCriteriaEnum.Index:
                    AddIndexSearchCriteria(map);
                    break;
                case SearchCriteriaEnum.PrimaryKey:
                    AddPrimaryKeySearchCriteria(map);
                    break;
            }

            _searchCriteria = map.Values.ToList();
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

        private void AddPrimaryKeySearchCriteria(IDictionary<string, SearchCriteria> map)
        {
            SearchCriteria searchCriteria = new SearchCriteria(true);

            foreach (MemberColumnSchema column in Table.PrimaryKey.MemberColumns)
                searchCriteria.Members.Add(GetFromColumn(column));

            AddToMap(map, searchCriteria);
        }

        private void AddForeignKeySearchCriteria(IDictionary<string, SearchCriteria> map)
        {
            foreach (TableKeySchema tks in Table.ForeignKeys)
            {
                SearchCriteria searchCriteria = new SearchCriteria(false);

                foreach (MemberColumnSchema column in tks.ForeignKeyMemberColumns)
                    if (column.Table.Equals(Table))
                        searchCriteria.Members.Add(GetFromColumn(column));

                AddToMap(map, searchCriteria);
            }
        }

        private void AddIndexSearchCriteria(IDictionary<string, SearchCriteria> map)
        {
            foreach (IndexSchema indexSchema in Table.Indexes)
            {
                SearchCriteria searchCriteria = new SearchCriteria(false);

                foreach (MemberColumnSchema column in indexSchema.MemberColumns)
                    if (column.Table.Equals(Table))
                        searchCriteria.Members.Add(GetFromColumn(column));

                AddToMap(map, searchCriteria);
            }
        }

        private static bool AddToMap(IDictionary<string, SearchCriteria> map, SearchCriteria searchCriteria)
        {
            string key = searchCriteria.Key;
            bool result = (searchCriteria.Members.Count > 0 && !map.ContainsKey(key));

            if (result)
                map.Add(key, searchCriteria);

            return result;
        }

        #endregion

        #endregion

        #region Public Methods

        public MemberBase GetFromColumn(ColumnSchema column)
        {
            if (MemberMap.ContainsKey(column.Name))
                return MemberMap[column.Name];

            if (AssociationMap.ContainsKey(column.Name))
                return AssociationMap[column.Name];

            return null;
        }

        public AssociationMember GetAssocitionFromColumn(ColumnSchema column)
        {
            return (AssociationMap.ContainsKey(column.Name))
                       ? AssociationMap[column.Name]
                       : null;
        }

        public Member GetMemberFromColumn(ColumnSchema column)
        {
            return (MemberMap.ContainsKey(column.Name))
                       ? MemberMap[column.Name]
                       : null;
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

        private Dictionary<string, AssociationMember> AssociationMap
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

        private Dictionary<string, Member> FKMemberMap
        {
            get
            {
                if (_fkMemberMap == null)
                    Init();
                return _fkMemberMap;
            }
        }

        private Dictionary<string, AssociationMember> FKRemoteMemberMap
        {
            get
            {
                if (_fkRemoteMemberMap == null)
                    Init();
                return _fkRemoteMemberMap;
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

        public bool HasOneToZeroOrZeroMember
        {
            get { return (OneToZeroOrZeroMember != null); }
        }

        public AssociationMember OneToZeroOrZeroMember
        {
            get
            {
                var associationMembers = new List<AssociationMember>();

                foreach (TableKeySchema tks in Table.ForeignKeys)
                {
                    if (tks.PrimaryKeyMemberColumns.Count == 1 && tks.ForeignKeyMemberColumns.Count == 1)
                    {
                        ColumnSchema column = tks.PrimaryKeyMemberColumns[0].Column;

                        if (column.IsPrimaryKeyMember) //column.Name.Equals(tks.ForeignKeyMemberColumns[0].Column.Name, StringComparison.InvariantCultureIgnoreCase)
                        {
                            if (!Configuration.Instance.ExcludeTableRegexIsMatch(column.Table.FullName))
                            {
                                associationMembers.Add(new AssociationMember(AssociationType.OneToZeroOrOne,
                                                                             column.Table, column,
                                                                             tks.ForeignKeyMemberColumns[0], this));
                            }
                        }
                    }
                }

                return associationMembers.FirstOrDefault();
            }
        }

        public List<Member> Members
        {
            get
            {
                var memberUnion = MemberMap.Values.Where(m => !m.IsPrimaryKey).ToList().Union(FKMemberMap.Values).ToList();

                List<Member> members = new List<Member>(memberUnion.Count);
                foreach (Member memberBase in memberUnion)
                {
                    string name = memberBase.ColumnName;
                    if (members.Count(m => m.ColumnName == name) == 0)
                        members.Add(memberBase);
                }

                return members;
            }
        }

        public List<Member> MembersNoRowVersion
        {
            get
            {
                return Members
                    .Where(m => !m.IsRowVersion)
                    .ToList();
            }
        }


        public List<AssociationMember> RemoteAssociations
        {
            get
            {
                return FKRemoteMemberMap.Values.Cast<AssociationMember>().ToList();
            }
        }

        //public List<MemberBase> MembersNoRowVersionIncludeRemoteAssociations
        //{
        //    get
        //    {
        //        var memberUnion = MemberMap.Values.Cast<MemberBase>().Where(m => !m.IsPrimaryKey && !m.IsRowVersion).ToList().Union(FKRemoteMemberMap.Values.Cast<MemberBase>()).ToList();

        //        List<MemberBase> members = new List<MemberBase>(memberUnion.Count);
        //        foreach (MemberBase memberBase in memberUnion)
        //        {
        //            string name = memberBase.ColumnName;
        //            if (members.Count(m => m.ColumnName == name) == 0)
        //                members.Add(memberBase);
        //        }

        //        return members;
        //    }
        //}

        public List<Member> MembersNoForeignKey
        {
            get
            {
                return Members
                    .Where(m => !m.IsForeignKey)
                    .ToList();
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

        public List<Member> MembersNoRowVersionIncludePrimaryKey
        {
            get
            {
                var membersNoRowVersion = MemberMap.Values.Where(m => !m.IsRowVersion).ToList();

                List<Member> members = new List<Member>(membersNoRowVersion.Count);
                foreach (Member memberBase in membersNoRowVersion)
                {
                    string name = memberBase.ColumnName;
                    if (members.Count(m => m.ColumnName == name) == 0)
                        members.Add(memberBase);
                }

                return members;
            }
        }

        public List<MemberBase> MembersNoRowVersionIncludePrimaryKeyForeignKey
        {
            get
            {
                var membersNoRowVersion = MemberMap.Values.Cast<MemberBase>().Where(m => !m.IsRowVersion).ToList().Union(FKRemoteMemberMap.Values.Cast<MemberBase>().Where(fk => fk.TableName == this.Table.Name).Cast<MemberBase>().ToList()).ToList();

                List<MemberBase> members = new List<MemberBase>(membersNoRowVersion.Count);
                foreach (var memberBase in membersNoRowVersion)
                {
                    string name = memberBase.ColumnName;
                    if (members.Count(m => m.ColumnName == name) == 0)
                    {
                        members.Add(memberBase);
                    }
                        
                }

                return members;
            }
        }

        public List<AssociationMember> MembersForeignKey
        {
            get
            {
                var foreignKeys = FKRemoteMemberMap.Values.Where(fk => fk.TableName == this.Table.Name).ToList();

                List<AssociationMember> members = new List<AssociationMember>(foreignKeys.Count);
                foreach (var associationMember in foreignKeys)
                {
                    string name = associationMember.ColumnName;
                    if (members.Count(m => m.ColumnName == name) == 0)
                    {
                        members.Add(associationMember);
                    }

                }

                return members;
            }
        }

        //public List<MemberBase> MembersNoRowVersionIncludePrimaryKeyIncludeRemoteAssociations
        //{
        //    get
        //    {
        //        var memberUnion = MemberMap.Values.Cast<MemberBase>().Where(m => !m.IsRowVersion).ToList().Union(FKRemoteMemberMap.Values.Cast<MemberBase>()).ToList();

        //        List<MemberBase> members = new List<MemberBase>(memberUnion.Count);
        //        foreach ( MemberBase memberBase in memberUnion )
        //        {
        //            string name = memberBase.ColumnName;
        //            if (members.Count(m => m.ColumnName == name) == 0)
        //                members.Add( memberBase );
        //        }

        //        return members;
        //    }
        //}

        public List<Member> MembersNoRowVersionNoForeignKeyIncludePrimaryKey
        {
            get
            {
                return MemberMap.Values.Where(m => !m.IsRowVersion && !m.IsForeignKey).ToList();
            }
        }

        public List<Member> MembersPrimaryKeyUnion
        {
            get { return MemberMap.Values.ToList(); }
        }

        public List<MemberBase> MembersManyUnion
        {
            get
            {
                return MemberMap.Values
                    .Cast<MemberBase>()
                    .Union(
                        AssociationMap.Values
                        .Cast<MemberBase>()
                    ).ToList();
            }
        }

        public List<AssociationMember> ManyToOne
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.ManyToOne)
                    .ToList();
            }
        }

        public List<AssociationMember> OneToZeroOrOne
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.OneToZeroOrOne)
                    .ToList();
            }
        }

        public List<AssociationMember> OneToMany
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.OneToMany)
                    .ToList();
            }
        }

        public List<AssociationMember> ManyToMany
        {
            get
            {
                return AssociationMap.Values
                    .Where(a => a.AssociationType == AssociationType.ManyToMany)
                    .ToList();
            }
        }

        public List<AssociationMember> ToManyUnion
        {
            get
            {
                return AssociationMap.Values
                    .Where(a =>
                           a.AssociationType == AssociationType.ManyToMany
                           || a.AssociationType == AssociationType.OneToMany)
                    .ToList();
            }
        }

        public List<SearchCriteria> SearchCriteria
        {
            get
            {
                if (_searchCriteria == null)
                    Init();
                return _searchCriteria;
            }
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