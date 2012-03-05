using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeSmith.SchemaHelper;
using SchemaExplorer;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA
{
    // TODO: This needs to be profiled. We probably should be caching the table entities so they are not retreived repeatidly...
    class CSLASchemaExplorerEntityProvider : SchemaExplorerEntityProvider
    {
        private static readonly Dictionary<string, SortedDictionary<string, IEntity>> _cache = new Dictionary<string, SortedDictionary<string, IEntity>>(); 
        private readonly string _extendedPropertyName;
        private readonly List<TableSchema> _tablesToKeep = new List<TableSchema>(); 

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database) : base(database)
        {
        }

        public CSLASchemaExplorerEntityProvider(TableSchema table) : base(table.Database)
        {
            _tablesToKeep.Add(table);
        }

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database, TableSchemaCollection tables) : base(database)
        {
            _tablesToKeep.AddRange(tables);
        }

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database, TableSchemaCollection tables, string extendedPropertyName) : this(database, tables)
        {
            _tablesToKeep.AddRange(tables);
            _extendedPropertyName = extendedPropertyName;
        }

        protected override void Initialize(TableSchemaCollection tables, ViewSchemaCollection views, CommandSchemaCollection commands)
        {
            var key = _database != null ? _database.FullName + Configuration.Instance.IncludeAssociations : null;
            if (String.IsNullOrEmpty(key) || !_cache.ContainsKey(key))
            {
                base.Initialize(tables, views, commands);
                if (key != null)
                    _cache[key] = EntityStore.Instance.EntityCollection;
            }

            var entities = key != null && _cache.ContainsKey(key) ? _cache[key] : EntityStore.Instance.EntityCollection;
            var itemsToRemove = entities.Where(e => e.Value is TableEntity && !_tablesToKeep.Contains(((TableEntity)e.Value).EntitySource)).ToList();
            foreach (var entity in itemsToRemove)
            {
                EntityStore.Instance.EntityCollection.Remove(entity.Key);
            }
        }

        protected override void LoadViews(ViewSchemaCollection views)
        {
            if (String.IsNullOrEmpty(_extendedPropertyName) || !Configuration.Instance.IncludeViews || _database == null)
            {
                base.LoadViews(views);
                return;
            }

            foreach (var view in _database.Views)
            {
                if (_extendedPropertyName.Equals(Constants.Criteria) && !IsBusinessObject(view.ExtendedProperties))
                    continue;

                if (!_extendedPropertyName.Equals(Constants.Criteria) && !view.ExtendedProperties.Contains(_extendedPropertyName))
                    continue;

                var viewEntity = new ViewEntity(view);

                if (!Configuration.Instance.IncludeRegexIsMatch(view.FullName) ||
                    Configuration.Instance.ExcludeRegexIsMatch(view.FullName))
                {
                    Trace.WriteLine(String.Format("Skipping view: '{0}'", view.FullName));
                    Debug.WriteLine(String.Format("Skipping view: '{0}'", view.FullName));

                    EntityStore.Instance.ExcludedEntityCollection.Add(view.FullName, viewEntity);
                    continue;
                }

                CodeSmith.Engine.Logger.Log.Info(String.Format("Adding '{0}' Entity of type {1}", view.FullName, _extendedPropertyName));
                EntityStore.Instance.EntityCollection.Add(view.FullName, viewEntity);
            }
        }

        protected override void LoadCommands(CommandSchemaCollection commands)
        {
            if (String.IsNullOrEmpty(_extendedPropertyName) || !Configuration.Instance.IncludeFunctions || _database == null)
            {
                base.LoadCommands(commands);
                return;
            }

            foreach (var command in _database.Commands)
            {
                if(_extendedPropertyName.Equals(Constants.Criteria) && !IsBusinessObject(command.ExtendedProperties))
                    continue;

                if (!_extendedPropertyName.Equals(Constants.Criteria) && !command.ExtendedProperties.Contains(_extendedPropertyName))
                    continue;

                var commandEntity = new CommandEntity(command);

                if (!Configuration.Instance.IncludeRegexIsMatch(command.FullName) ||
                    Configuration.Instance.ExcludeRegexIsMatch(command.FullName))
                {
                    Trace.WriteLine(String.Format("Skipping command: '{0}'", command.FullName));
                    Debug.WriteLine(String.Format("Skipping command: '{0}'", command.FullName));

                    EntityStore.Instance.ExcludedEntityCollection.Add(command.FullName, commandEntity);
                    continue;
                }

                EntityStore.Instance.CommandCollection.Add(command.FullName, commandEntity);
                EntityStore.Instance.EntityCollection.Add(command.FullName, commandEntity);
            }
        }

        private bool IsBusinessObject(ExtendedPropertyCollection collection)
        {
            if (collection.Contains(Constants.DynamicListBase)
                || collection.Contains(Constants.DynamicRoot)
                || collection.Contains(Constants.DynamicRootList)
                || collection.Contains(Constants.EditableChild)
                || collection.Contains(Constants.EditableChildList)
                || collection.Contains(Constants.EditableRoot)
                || collection.Contains(Constants.EditableRootList)
                || collection.Contains(Constants.NameValueList)
                || collection.Contains(Constants.ReadOnlyChild)
                || collection.Contains(Constants.ReadOnlyChildList)
                || collection.Contains(Constants.ReadOnlyList)
                || collection.Contains(Constants.ReadOnlyRoot)
                || collection.Contains(Constants.SwitchableObject))
                return true;

            return false;
        }
    }
}
