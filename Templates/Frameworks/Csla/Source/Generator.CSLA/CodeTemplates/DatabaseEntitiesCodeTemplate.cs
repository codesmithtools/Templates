using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CodeSmith.CustomProperties;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using SchemaExplorer;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA.CodeTemplates
{
    public class DatabaseEntitiesCodeTemplate : EntitiesCodeTemplate
    {
        private List<IEntity> _entities = new List<IEntity>();
        private DatabaseSchema _database;

        public DatabaseEntitiesCodeTemplate()
        {
            CleanExpressions = new StringCollection();
            IgnoreExpressions = new StringCollection();
        }

        [Category("1. DataSource")]
        [Description("Source Database")]
        public DatabaseSchema SourceDatabase
        {
            get { return _database; }
            set
            {
                if (value != null && (_database == null || (_database != null && value.Name != _database.Name && value.ConnectionString != _database.ConnectionString)))
                {
                    _database = value;
                    if (!_database.DeepLoad)
                    {
                        _database.DeepLoad = true;
                        _database.Refresh();
                    }

                    OnDatabaseChanged();
                }
            }
        }

        [Category("1. DataSource")]
        [Description("List of regular expressions to clean table, view and column names.")]
        [Optional]
        [DefaultValue("^(sp|tbl|udf|vw)_")]
        public StringCollection CleanExpressions { get; set; }

        [Category("1. DataSource")]
        [Description("List of regular expressions to ignore tables when generating.")]
        [Optional]
        [DefaultValue("sysdiagrams$")]
        public StringCollection IgnoreExpressions { get; set; }

        [Optional]
        [Category("1. DataSource")]
        [Description("Include views that have an extended property declaring it's business type")]
        public bool IncludeViews
        {
            get { return Configuration.Instance.IncludeViews; }
            set { Configuration.Instance.IncludeViews = value; }
        }

        [Optional]
        [Category("1. DataSource")]
        [Description("Include stored procedures that have an extended property declaring it's business type")]
        public bool IncludeFunctions
        {
            get { return Configuration.Instance.IncludeFunctions; }
            set { Configuration.Instance.IncludeFunctions = value; }
        }

        #region Business Object Selections

        [Category("6a. Entities")]
        [Description("CommandObject")]
        [Optional]
        public TableSchemaCollection CommandObject
        {
            get { return CommandObjectEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    CommandObjectEntities = new EntityManager(new CSLAEntityProvider(_entities, value)).Entities;
                }
            }
        }

        [Category("6a. Entities")]
        [Description("DynamicRoot")]
        [Optional]
        public TableSchemaCollection DynamicRoot
        {
            get { return DynamicRootEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    DynamicRootEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.DynamicRoot)).Entities;
                }
            }
        }

        [Category("6a. Entities")]
        [Description("EditableChild")]
        [Optional]
        public TableSchemaCollection EditableChild
        {
            get { return EditableChildEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    EditableChildEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.EditableChild)).Entities;
                }
            }
        }

        [Category("6a. Entities")]
        [Description("EditableRoot")]
        [Optional]
        public TableSchemaCollection EditableRoot
        {
            get { return EditableRootEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    EditableRootEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.EditableRoot)).Entities;
                }
            }
        }

        [Category("6a. Entities")]
        [Description("ReadOnlyChild")]
        [Optional]
        public TableSchemaCollection ReadOnlyChild
        {
            get { return ReadOnlyChildEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    ReadOnlyChildEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.ReadOnlyChild)).Entities;
                }
            }
        }

        [Category("6a. Entities")]
        [Description("ReadOnlyRoot")]
        [Optional]
        public TableSchemaCollection ReadOnlyRoot
        {
            get { return ReadOnlyRootEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    ReadOnlyRootEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.ReadOnlyRoot)).Entities;
                }
            }
        }

        [Category("6a. Entities")]
        [Description("SwitchableObject")]
        [Optional]
        public TableSchemaCollection SwitchableObject
        {
            get { return SwitchableObjectEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    SwitchableObjectEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.SwitchableObject)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("DynamicListBase")]
        [Optional]
        public TableSchemaCollection DynamicListBase
        {
            get { return this.DynamicListBaseEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    this.DynamicListBaseEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.DynamicListBase)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("DynamicRootList")]
        [Optional]
        public TableSchemaCollection DynamicRootList
        {
            get { return DynamicRootListEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    DynamicRootListEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.DynamicRootList)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("EditableRootList")]
        [Optional]
        public TableSchemaCollection EditableRootList
        {
            get { return EditableRootListEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    EditableRootListEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.EditableRootList)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("EditableChildList")]
        [Optional]
        public TableSchemaCollection EditableChildList
        {
            get { return EditableChildListEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    EditableChildListEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.EditableChildList)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("ReadOnlyList")]
        [Optional]
        public TableSchemaCollection ReadOnlyList
        {
            get { return ReadOnlyListEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    ReadOnlyListEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.ReadOnlyList)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("ReadOnlyChildList")]
        [Optional]
        public TableSchemaCollection ReadOnlyChildList
        {
            get { return ReadOnlyChildListEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    ReadOnlyChildListEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.ReadOnlyChildList)).Entities;
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("NameValueList")]
        [Optional]
        public TableSchemaCollection NameValueList
        {
            get { return NameValueListEntities.ToCollection(); }
            set
            {
                if (value != null)
                {
                    EnsureEntitiesExists(value);
                    NameValueListEntities = new EntityManager(new CSLAEntityProvider(_entities, value, Constants.NameValueList)).Entities;
                }
            }
        }

        #endregion

        private bool _dontPopulateDefaultValues;
        private void EnsureEntitiesExists(TableSchemaCollection value)
        {
            // If entities exist then return.
            if (_entities != null && _entities.Count > 0)
                return;

            // The entities haven't been set yet, so lets set them if there is atleast one table passed in.
            if (value == null || value.Count <= 0)
                return;

            _dontPopulateDefaultValues = true;
            SourceDatabase = value[0].Database;
            _dontPopulateDefaultValues = false;
        }

        public virtual void OnDatabaseChanged()
        {
            using (TemplateContext.SetContext(this))
            {
                if (!UpdateEntities()) 
                    return;

                if (State == TemplateState.RestoringProperties)
                    return;

                if (_dontPopulateDefaultValues)
                    return;

                if (DynamicRoot.Count == 0 &&
                    EditableChild.Count == 0 &&
                    EditableRoot.Count == 0 &&
                    ReadOnlyChild.Count == 0 &&
                    ReadOnlyRoot.Count == 0 &&
                    SwitchableObject.Count == 0 &&
                    DynamicListBase.Count == 0 &&
                    DynamicRootList.Count == 0 &&
                    EditableRootList.Count == 0 &&
                    EditableChildList.Count == 0 &&
                    ReadOnlyList.Count == 0 &&
                    ReadOnlyChildList.Count == 0)
                {
                    PopulateDefaultEntities(_entities);
                }
                else
                {
                    Refresh();
                }
            }
        }

        protected override void OnPropertiesLoaded()
        {
            if (TemplateContext.Current == null && UpdateEntities())
                Refresh();

            base.OnPropertiesLoaded();
        }

        private bool UpdateEntities()
        {
            if (SourceDatabase == null)
                return false;

            if (CleanExpressions.Count == 0)
                CleanExpressions.Add("^(sp|tbl|udf|vw)_");

            Configuration.Instance.CleanExpressions.Clear();
            foreach (string clean in CleanExpressions)
            {
                if (!string.IsNullOrEmpty(clean))
                {
                    Configuration.Instance.CleanExpressions.Add(new Regex(clean, RegexOptions.IgnoreCase));
                }
            }

            if (IgnoreExpressions.Count == 0)
            {
                IgnoreExpressions.Add("sysdiagrams$");
                IgnoreExpressions.Add("^dbo.aspnet");
            }

            Configuration.Instance.IgnoreExpressions.Clear();
            foreach (string ignore in IgnoreExpressions)
            {
                if (!string.IsNullOrEmpty(ignore))
                {
                    Configuration.Instance.IgnoreExpressions.Add(new Regex(ignore, RegexOptions.IgnoreCase));
                }
            }

            var provider = new SchemaExplorerEntityProvider(SourceDatabase);
            _entities = new EntityManager(provider).Entities;

            return true;
        }

        private void Refresh()
        {
            DynamicRoot = DynamicRoot;
            EditableChild = EditableChild;
            EditableRoot = EditableRoot;
            ReadOnlyChild = ReadOnlyChild;
            ReadOnlyRoot = ReadOnlyRoot;
            SwitchableObject = SwitchableObject;
            DynamicListBase = DynamicListBase;
            DynamicRootList = DynamicRootList;
            EditableRootList = EditableRootList;
            EditableChildList = EditableChildList;
            ReadOnlyList = ReadOnlyList;
            ReadOnlyChildList = ReadOnlyChildList;
            NameValueList = NameValueList;
        }
    }
}
