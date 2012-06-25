using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    internal class TableSearchCriteria
    {
        #region Constructor

        public TableSearchCriteria(EntityManager entityManager, TableSchema sourceTable)
            : this(entityManager, sourceTable, String.Empty) { }
        public TableSearchCriteria(EntityManager entityManager, TableSchema sourceTable, string extendedProperty)
        {
            this.Table = sourceTable;
            this.EntityManager = entityManager;
            this.ExtendedProperty = (String.IsNullOrEmpty(extendedProperty))
                ? NHibernateHelper.ExtendedPropertyName
                : extendedProperty;
        }

        #endregion

        #region Methods

        public List<SearchCriteria> GetAllSearchCriteria()
        {
            Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

            GetPrimaryKeySearchCriteria(map);
            GetForeignKeySearchCriteria(map);
            GetIndexSearchCriteria(map);

            return GetResultsFromMap(map);
        }
        public List<SearchCriteria> GetPrimaryKeySearchCriteria()
        {
            Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

            GetPrimaryKeySearchCriteria(map);

            return GetResultsFromMap(map);
        }
        public List<SearchCriteria> GetForeignKeySearchCriteria()
        {
            Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

            GetForeignKeySearchCriteria(map);

            return GetResultsFromMap(map);
        }
        public List<SearchCriteria> GetIndexSearchCriteria()
        {
            Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

            GetIndexSearchCriteria(map);

            return GetResultsFromMap(map);
        }

        protected void GetPrimaryKeySearchCriteria(Dictionary<string, SearchCriteria> map)
        {
            List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>(Table.PrimaryKey.MemberColumns.ToArray());
            CleanColumnList(mcsList);
            SearchCriteria searchCriteria = new SearchCriteria(ExtendedProperty, mcsList, true);

            if (!String.IsNullOrEmpty(ExtendedProperty)
               && Table.PrimaryKey.ExtendedProperties.Contains(ExtendedProperty)
               && Table.PrimaryKey.ExtendedProperties[ExtendedProperty].Value != null
              )
                searchCriteria.SetMethodNameGeneration(Table.PrimaryKey.ExtendedProperties[ExtendedProperty].Value.ToString());

            AddToMap(map, searchCriteria);
        }
        protected void GetForeignKeySearchCriteria(Dictionary<string, SearchCriteria> map)
        {
            foreach (TableKeySchema tks in Table.ForeignKeys)
            {
                SearchCriteria searchCriteria = new SearchCriteria(ExtendedProperty);
                List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>(tks.ForeignKeyMemberColumns.ToArray());
                CleanColumnList(mcsList);
                foreach (MemberColumnSchema mcs in mcsList)
                    searchCriteria.Add(mcs);

                if (!String.IsNullOrEmpty(ExtendedProperty) && tks.ExtendedProperties.Contains(ExtendedProperty) && tks.ExtendedProperties[ExtendedProperty].Value != null)
                    searchCriteria.SetMethodNameGeneration(tks.ExtendedProperties[ExtendedProperty].Value.ToString());

                AddToMap(map, searchCriteria);
            }
        }
        protected void GetIndexSearchCriteria(Dictionary<string, SearchCriteria> map)
        {
            foreach (IndexSchema indexSchema in Table.Indexes)
            {
                SearchCriteria searchCriteria = new SearchCriteria(ExtendedProperty);
                List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>(indexSchema.MemberColumns.ToArray());
                CleanColumnList(mcsList);
                foreach (MemberColumnSchema mcs in mcsList)
                    searchCriteria.Add(mcs);

                if (!String.IsNullOrEmpty(ExtendedProperty) && indexSchema.ExtendedProperties.Contains(ExtendedProperty) && indexSchema.ExtendedProperties[ExtendedProperty].Value != null)
                    searchCriteria.SetMethodNameGeneration(indexSchema.ExtendedProperties[ExtendedProperty].Value.ToString());

                AddToMap(map, searchCriteria);
            }
        }

        protected bool AddToMap(Dictionary<string, SearchCriteria> map, SearchCriteria searchCriteria)
        {
            string key = searchCriteria.Key;
            bool result = (searchCriteria.Items.Count > 0 && !map.ContainsKey(key));

            if (result)
                map.Add(key, searchCriteria);

            return result;
        }
        protected List<SearchCriteria> GetResultsFromMap(Dictionary<string, SearchCriteria> map)
        {
            List<SearchCriteria> result = new List<SearchCriteria>();
            foreach (SearchCriteria sc in map.Values)
                result.Add(sc);
            return result;
        }

        protected void CleanColumnList(List<MemberColumnSchema> mcsList)
        {
            for (int i = 0; i < mcsList.Count; i++)
            {
                if (!mcsList[i].Table.Equals(Table) || EntityManager.GetEntityBaseFromColumn(mcsList[i]) == null)
                {
                    mcsList.Remove(mcsList[i]);
                    i--;
                }
            }
        }

        #endregion

        #region Properties

        public string ExtendedProperty { get; protected set; }
        public TableSchema Table { get; protected set; }
        public EntityManager EntityManager { get; protected set; }

        #endregion
    }
}
