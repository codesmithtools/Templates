using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.SchemaHelper;
using SchemaExplorer;

namespace Generator.CSLA
{
    // TODO: This needs to be profiled. We probably should be caching the table entities so they are not retreived repeatidly...
    public class CSLAEntityProvider : IEntityProvider
    {
        private readonly string _extendedPropertyName;
        private readonly List<TableSchema> _tablesToKeep = new List<TableSchema>();
        private readonly List<IEntity> _entities;

        public CSLAEntityProvider(List<IEntity> entities, TableSchemaCollection tables)
        {
            _entities = entities;
            _tablesToKeep.AddRange(tables);
        }

        public CSLAEntityProvider(List<IEntity> entities, TableSchemaCollection tables, string extendedPropertyName)
        {
            _entities = entities;
            _tablesToKeep.AddRange(tables);
            _extendedPropertyName = extendedPropertyName;
        }

        #region Implementation of IEntityProvider

        public string Name
        {
            get { return "CSLAEntityProvider"; }
        }

        public string Description
        {
            get { return "CSLA Entity Provider"; }
        }

        public bool Validate()
        {
            return _entities != null;
        }

        public void Load()
        {
            foreach (var entity in _entities)
            {
                if (entity is TableEntity)
                {
                    if (_tablesToKeep.Contains(((TableEntity)entity).EntitySource))
                        EntityStore.Instance.EntityCollection.Add(entity.EntityKey(), entity);

                    continue;
                }

                if (String.IsNullOrEmpty(_extendedPropertyName))
                    continue;

                if (_extendedPropertyName.Equals(Constants.Criteria) && !IsBusinessObject(entity.ExtendedProperties))
                    continue;

                if (!_extendedPropertyName.Equals(Constants.Criteria) && !entity.ExtendedProperties.ContainsKey(_extendedPropertyName))
                    continue;

                EntityStore.Instance.EntityCollection.Add(entity.EntityKey(), entity);
            }
        }

        public void Save()
        {
        }

        #endregion

        private bool IsBusinessObject(Dictionary<string, object> collection)
        {
            if (collection.ContainsKey(Constants.DynamicListBase)
                || collection.ContainsKey(Constants.DynamicRoot)
                || collection.ContainsKey(Constants.DynamicRootList)
                || collection.ContainsKey(Constants.EditableChild)
                || collection.ContainsKey(Constants.EditableChildList)
                || collection.ContainsKey(Constants.EditableRoot)
                || collection.ContainsKey(Constants.EditableRootList)
                || collection.ContainsKey(Constants.NameValueList)
                || collection.ContainsKey(Constants.ReadOnlyChild)
                || collection.ContainsKey(Constants.ReadOnlyChildList)
                || collection.ContainsKey(Constants.ReadOnlyList)
                || collection.ContainsKey(Constants.ReadOnlyRoot)
                || collection.ContainsKey(Constants.SwitchableObject))
                return true;

            return false;
        }
    }
}
