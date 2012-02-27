//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2012 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using SchemaExplorer;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA
{
    /// <summary>
    /// 
    /// </summary>
    public class DataAccessCodeTemplate : QuickStartCodeTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        public DataAccessCodeTemplate()
        {
            UpdateTableCollections();
        }

        #region Hidden Properties

        [Browsable(false)]
        public new Language Language { get; set; }
        [Browsable(false)]
        public new bool LaunchVisualStudio { get; set; }
        [Browsable(false)]
        public new bool UseMemberVariables { get; set; }
        [Browsable(false)]
        public new string InterfaceProjectName { get; set; }

        #endregion

        [Browsable(false)]
        public List<IEntity> DynamicRootEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableChildEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableRootEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyChildEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyRootEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> SwitchableObjectEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> DynamicListBaseEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> DynamicRootListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableRootListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableChildListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyChildListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> NameValueListEntities = new List<IEntity>();

        #region 6a. Entities

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
                    DynamicRootEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.DynamicRoot)).Entities;
                    OnDynamicRootChanged();
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
                    EditableChildEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.EditableChild)).Entities;
                    OnEditableChildChanged();
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
                    EditableRootEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.EditableRoot)).Entities;
                    OnEditableRootChanged();
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
                    ReadOnlyChildEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.ReadOnlyChild)).Entities;
                    OnReadOnlyChildChanged();
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
                    ReadOnlyRootEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.ReadOnlyRoot)).Entities;
                    OnReadOnlyRootChanged();
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
                    SwitchableObjectEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.SwitchableObject)).Entities;
                    OnSwitchableObjectChanged();
                }
            }
        }

        #endregion

        #region 6b. List Entities

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
                    this.DynamicListBaseEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.DynamicListBase)).Entities;
                    OnDynamicListBaseChanged();
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
                    DynamicRootListEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.DynamicRootList)).Entities;
                    OnDynamicRootListChanged();
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
                    EditableRootListEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.EditableRootList)).Entities;
                    OnEditableRootListChanged();
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
                    EditableChildListEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.EditableChildList)).Entities;
                    OnEditableChildListChanged();
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
                    ReadOnlyListEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.ReadOnlyList)).Entities;
                    OnReadOnlyListChanged();
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
                    ReadOnlyChildListEntities = new EntityManager(new CSLASchemaExplorerEntityProvider(SourceDatabase, value, Constants.ReadOnlyChildList)).Entities;
                    OnReadOnlyChildListChanged();
                }
            }
        }

        #endregion

        #region Private methods

        #region OnEnitityChanged

        private void OnDynamicRootChanged()
        {
            CleanTemplateContextByValue(Constants.DynamicRoot);
            foreach (var entity in DynamicRootEntities)
            {
                if (ContextData.Get(entity.EntityKeyName) != null)
                    ContextData.Remove(entity.EntityKeyName);

                EditableChildEntities.Remove(entity);
                EditableRootEntities.Remove(entity);
                ReadOnlyChildEntities.Remove(entity);
                ReadOnlyRootEntities.Remove(entity);
                SwitchableObjectEntities.Remove(entity);

                ContextData.Add(entity.EntityKeyName, Constants.DynamicRoot);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in childEntity.Properties)
                    {
                        AddChildEntity(property.Property.Entity, false, false);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
                {
                    foreach (AssociationProperty property in childList.Properties)
                    {
                        AddChildList(property.Property.Entity, false, false);
                    }
                }
            }
        }

        private void OnEditableChildChanged()
        {
            CleanTemplateContextByValue(Constants.EditableChild);
            foreach (var entity in EditableChildEntities)
            {
                if (ContextData.Get(entity.EntityKeyName) != null)
                    ContextData.Remove(entity.EntityKeyName);

                DynamicRootEntities.Remove(entity);
                EditableRootEntities.Remove(entity);
                SwitchableObjectEntities.Remove(entity);

                ContextData.Add(entity.EntityKeyName, Constants.EditableChild);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in childEntity.Properties)
                    {
                        AddChildEntity(property.Property.Entity, false, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
                {
                    foreach (AssociationProperty property in childList.Properties)
                    {
                        AddChildList(property.Property.Entity, false, true);
                    }
                }
            }
        }

        private void OnEditableRootChanged()
        {
            CleanTemplateContextByValue(Constants.EditableRoot);

            foreach (var entity in EditableRootEntities)
            {
                if (ContextData.Get(entity.EntityKeyName) != null)
                    ContextData.Remove(entity.EntityKeyName);

                DynamicRootEntities.Remove(entity);
                EditableChildEntities.Remove(entity);
                SwitchableObjectEntities.Remove(entity);

                ContextData.Add(entity.EntityKeyName, Constants.EditableRoot);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in childEntity.Properties)
                    {
                        AddChildEntity(property.Association.Entity, false, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
                {
                    foreach (AssociationProperty property in childList.Properties)
                    {
                        AddChildList(property.Association.Entity, false, true);
                    }
                }
            }
        }

        private void OnReadOnlyChildChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyChild);

            foreach (var entity in ReadOnlyChildEntities)
            {
                string key = String.Format(Constants.ReadOnlyFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyRootEntities.Remove(entity);

                ContextData.Add(key, Constants.ReadOnlyChild);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in childEntity.Properties)
                    {
                        AddChildEntity(property.Association.Entity, true, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
                {
                    foreach (AssociationProperty property in childList.Properties)
                    {
                        AddChildList(property.Association.Entity, true, true);
                    }
                }
            }
        }

        private void OnReadOnlyRootChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyRoot);

            foreach (var entity in ReadOnlyRootEntities)
            {
                string key = String.Format(Constants.ReadOnlyFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyChildEntities.Remove(entity);

                ContextData.Add(key, Constants.ReadOnlyRoot);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in childEntity.Properties)
                    {
                        AddChildEntity(property.Association.Entity, true, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
                {
                    foreach (AssociationProperty property in childList.Properties)
                    {
                        AddChildList(property.Association.Entity, true, true);
                    }
                }
            }
        }

        private void OnSwitchableObjectChanged()
        {
            CleanTemplateContextByValue(Constants.SwitchableObject);

            foreach (var entity in SwitchableObjectEntities)
            {
                if (ContextData.Get(entity.EntityKeyName) != null)
                    ContextData.Remove(entity.EntityKeyName);

                DynamicRootEntities.Remove(entity);
                EditableChildEntities.Remove(entity);
                EditableRootEntities.Remove(entity);

                ContextData.Add(entity.EntityKeyName, Constants.SwitchableObject);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in childEntity.Properties)
                    {
                        AddChildEntity(property.Association.Entity, false, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
                {
                    foreach (AssociationProperty property in childList.Properties)
                    {
                        AddChildList(property.Association.Entity, false, true);
                    }
                }
            }
        }

        #endregion

        #region OnListEnitityChanged Methods

        private void OnDynamicRootListChanged()
        {
            CleanTemplateContextByValue(Constants.DynamicRootList);

            foreach (var entity in DynamicRootListEntities)
            {
                string key = String.Format(Constants.ListFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                EditableRootListEntities.Remove(entity);
                DynamicListBaseEntities.Remove(entity);
                EditableChildListEntities.Remove(entity);

                ContextData.Add(key, Constants.DynamicRootList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                AddChildEntity(entity, false, true);
            }
        }

        private void OnEditableRootListChanged()
        {
            CleanTemplateContextByValue(Constants.EditableRootList);

            foreach (var entity in EditableRootListEntities)
            {
                string key = String.Format(Constants.ListFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                DynamicRootListEntities.Remove(entity);
                DynamicListBaseEntities.Remove(entity);
                EditableChildListEntities.Remove(entity);

                ContextData.Add(key, Constants.EditableRootList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                AddChildEntity(entity, false, true);
            }
        }

        private void OnDynamicListBaseChanged()
        {
            CleanTemplateContextByValue(Constants.DynamicListBase);

            foreach (var entity in DynamicListBaseEntities)
            {
                string key = String.Format(Constants.ListFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                DynamicRootListEntities.Remove(entity);
                EditableRootListEntities.Remove(entity);
                EditableChildListEntities.Remove(entity);

                ContextData.Add(key, Constants.DynamicListBase);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                AddChildEntity(entity, false, false);
            }
        }

        private void OnEditableChildListChanged()
        {
            CleanTemplateContextByValue(Constants.EditableChildList);

            foreach (var entity in EditableChildListEntities)
            {
                string key = String.Format(Constants.ListFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                DynamicRootListEntities.Remove(entity);
                EditableRootListEntities.Remove(entity);
                DynamicListBaseEntities.Remove(entity);

                ContextData.Add(key, Constants.EditableChildList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                AddChildEntity(entity, false, true);
            }
        }

        private void OnReadOnlyListChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyList);

            foreach (var entity in ReadOnlyListEntities)
            {
                string key = String.Format(Constants.ReadOnlyListFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyChildListEntities.Remove(entity);

                ContextData.Add(key, Constants.ReadOnlyList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                AddChildEntity(entity, true, true);
            }
        }

        private void OnReadOnlyChildListChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyChildList);

            foreach (var entity in ReadOnlyChildListEntities)
            {
                string key = String.Format(Constants.ReadOnlyListFormat, entity.EntityKeyName);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyListEntities.Remove(entity);

                ContextData.Add(key, Constants.ReadOnlyChildList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    continue;

                AddChildEntity(entity, true, true);
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateTableCollections()
        {
            if (DynamicRoot == null) DynamicRoot = new TableSchemaCollection();
            if (EditableChild == null) EditableChild = new TableSchemaCollection();
            if (EditableRoot == null) EditableRoot = new TableSchemaCollection();
            if (ReadOnlyChild == null) ReadOnlyChild = new TableSchemaCollection();
            if (ReadOnlyRoot == null) ReadOnlyRoot = new TableSchemaCollection();
            if (SwitchableObject == null) SwitchableObject = new TableSchemaCollection();

            if (DynamicListBase == null) DynamicListBase = new TableSchemaCollection();
            if (DynamicRootList == null) DynamicRootList = new TableSchemaCollection();
            if (EditableRootList == null) EditableRootList = new TableSchemaCollection();
            if (EditableChildList == null) EditableChildList = new TableSchemaCollection();
            if (ReadOnlyList == null) ReadOnlyList = new TableSchemaCollection();
            if (ReadOnlyChildList == null) ReadOnlyChildList = new TableSchemaCollection();
        }

        private void AddChildList(IEntity entity, bool readOnly, bool child)
        {
            if (Configuration.Instance.ExcludeRegexIsMatch(entity.EntityKey()))
                return;

            if (readOnly)
            {
                if (ReadOnlyList.Count > 0 && ReadOnlyList.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (ReadOnlyChildList.Count > 0 && ReadOnlyChildList.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;

                if (child)
                    ReadOnlyChildListEntities.Add(entity);
                else
                    ReadOnlyListEntities.Add(entity);
            }
            else
            {
                if (DynamicRootList.Count > 0 && DynamicRootList.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (EditableRootList.Count > 0 && EditableRootList.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (EditableChildList.Count > 0 && EditableChildList.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (child)
                    EditableChildListEntities.Add(entity);
                else
                    EditableRootListEntities.Add(entity);
            }
        }

        private void AddChildEntity(IEntity entity, bool readOnly, bool child)
        {
            if (Configuration.Instance.ExcludeRegexIsMatch(entity.EntityKeyName)) return;

            if (readOnly)
            {
                if (ReadOnlyChild.Count > 0 && ReadOnlyChild.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (ReadOnlyRoot.Count > 0 && ReadOnlyRoot.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;

                if (child)
                    ReadOnlyChildEntities.Add(entity);
                else
                    ReadOnlyRootEntities.Add(entity);
            }
            else
            {
                if (DynamicRoot.Count > 0 && DynamicRoot.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (EditableChild.Count > 0 && EditableChild.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (EditableRoot.Count > 0 && EditableRoot.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;
                if (SwitchableObject.Count > 0 && SwitchableObject.Contains(entity.SchemaName, entity.EntityKeyName))
                    return;

                if (child)
                    EditableChildEntities.Add(entity);
                else
                    EditableRootEntities.Add(entity);
            }
        }


        private void CleanTemplateContextByValue(string value)
        {
            UpdateTableCollections();
            List<string> keys = (from key in ContextData.AllKeys
                                 let contextValues = ContextData.GetValues(key)
                                 where contextValues != null && contextValues.Length > 0 && contextValues[0] == value
                                 select key).ToList();

            foreach (string key in keys)
                ContextData.Remove(key);
        }

        #endregion

        #endregion

        #region Public Overriden Methods

        public override void OnDatabaseChanged()
        {
            base.OnDatabaseChanged();

            string basePath = Path.Combine(CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory,
                                           Path.Combine("CSLA", SourceDatabase.Name));
            if (Location == basePath)
                Location = Path.Combine(Location, BusinessProjectName);

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
                PopulateDefaultTables();
            }

        }

        public void PopulateDefaultTables()
        {
            //EditableRoot
            foreach (var entity in GetRootEntities())
            {
                if (!EditableRoot.Contains(entity.SchemaName, entity.EntityKeyName))
                    EditableRootEntities.Add(entity);
            }

            //EditableChild
            foreach (var entity in GetChildEntities().Values)
            {
                if (!EditableChild.Contains(entity.SchemaName, entity.EntityKeyName))
                    EditableChildEntities.Add(entity);
            }

            //EditableChild
            foreach (var entity in GetExcludedEntities())
            {
                if (!EditableChild.Contains(entity.SchemaName, entity.EntityKeyName))
                    EditableChildEntities.Add(entity);
            }

            //EditableChildList
            foreach (var entity in GetListEntities().Values)
            {
                if (!EditableChildList.Contains(entity.SchemaName, entity.EntityKeyName))
                    EditableChildListEntities.Add(entity);
            }
        }

        #endregion
    }
}
