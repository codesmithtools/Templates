using System;
using System.Diagnostics;
using System.Linq;
using CodeSmith.SchemaHelper;
using SchemaExplorer;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA
{
    class CSLASchemaExplorerEntityProvider : SchemaExplorerEntityProvider
    {
        private readonly string _extendedPropertyName;
        public CSLASchemaExplorerEntityProvider(DatabaseSchema database) : base(database)
        {
        }

        public CSLASchemaExplorerEntityProvider(TableSchema table) : base(table.Database)
        {
            _tables = table.Database.Tables;
        }

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database, TableSchemaCollection tables) : base(database)
        {
            _tables = tables;

            if (_tables != null && _tables.Count > 0)
                _database = _tables[0].Database;
        }

        public CSLASchemaExplorerEntityProvider(DatabaseSchema database, TableSchemaCollection tables, string extendedPropertyName) : this(database, tables)
        {
            _tables = tables;
            _extendedPropertyName = extendedPropertyName;
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
