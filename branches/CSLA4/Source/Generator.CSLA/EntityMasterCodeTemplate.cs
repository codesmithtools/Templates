//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;
//using CodeSmith.Engine;
//using CodeSmith.SchemaHelper;
//using SchemaExplorer;
//using Configuration = CodeSmith.SchemaHelper.Configuration;

//namespace Generator.CSLA
//{
//    public class EntityMasterCodeTemplate : QuickStartCodeTemplate
//    {
//        public EntityMasterCodeTemplate()
//        {
//            Criteria = new TableSchemaCollection();
//            CommandObject = new TableSchemaCollection();
//            UpdateTableCollections();
//        }

//        #region Private Member(s)

//        private TableSchemaCollection _dynamicRoot = new TableSchemaCollection();
//        private TableSchemaCollection _editableChild = new TableSchemaCollection();
//        private TableSchemaCollection _editableRoot = new TableSchemaCollection();
//        private TableSchemaCollection _readOnlyChild = new TableSchemaCollection();
//        private TableSchemaCollection _readOnlyRoot = new TableSchemaCollection();
//        private TableSchemaCollection _switchableObject = new TableSchemaCollection();
//        private TableSchemaCollection _dynamicListBase = new TableSchemaCollection();
//        private TableSchemaCollection _dynamicRootList = new TableSchemaCollection();
//        private TableSchemaCollection _editableRootList = new TableSchemaCollection();
//        private TableSchemaCollection _editableChildList = new TableSchemaCollection();
//        private TableSchemaCollection _readOnlyList = new TableSchemaCollection();
//        private TableSchemaCollection _readOnlyChildList = new TableSchemaCollection();
//        private TableSchemaCollection _nameValueList = new TableSchemaCollection();

//        #endregion

//        #region 6a. Entities

//        [Category("6a. Entities")]
//        [Description("CommandObject")]
//        [Optional]
//        public TableSchemaCollection CommandObject { get; set; }

//        [Category("6a. Entities")]
//        [Description("Criteria")]
//        [Optional]
//        public TableSchemaCollection Criteria { get; set; }

//        [Category("6a. Entities")]
//        [Description("DynamicRoot")]
//        [Optional]
//        public TableSchemaCollection DynamicRoot
//        {
//            get { return _dynamicRoot; }
//            set
//            {
//                if (value != null)
//                {
//                    _dynamicRoot = value;
//                    OnDynamicRootChanged();
//                }
//            }
//        }

//        [Category("6a. Entities")]
//        [Description("EditableChild")]
//        [Optional]
//        public TableSchemaCollection EditableChild
//        {
//            get { return _editableChild; }
//            set
//            {
//                if (value != null)
//                {
//                    _editableChild = value;
//                    OnEditableChildChanged();
//                }
//            }
//        }

//        [Category("6a. Entities")]
//        [Description("EditableRoot")]
//        [Optional]
//        public TableSchemaCollection EditableRoot
//        {
//            get { return _editableRoot; }
//            set
//            {
//                if (value != null)
//                {
//                    _editableRoot = value;
//                    OnEditableRootChanged();
//                }
//            }
//        }

//        [Category("6a. Entities")]
//        [Description("ReadOnlyChild")]
//        [Optional]
//        public TableSchemaCollection ReadOnlyChild
//        {
//            get { return _readOnlyChild; }
//            set
//            {
//                if (value != null)
//                {
//                    _readOnlyChild = value;
//                    OnReadOnlyChildChanged();
//                }
//            }
//        }

//        [Category("6a. Entities")]
//        [Description("ReadOnlyRoot")]
//        [Optional]
//        public TableSchemaCollection ReadOnlyRoot
//        {
//            get { return _readOnlyRoot; }
//            set
//            {
//                if (value != null)
//                {
//                    _readOnlyRoot = value;
//                    OnReadOnlyRootChanged();
//                }
//            }
//        }

//        [Category("6a. Entities")]
//        [Description("SwitchableObject")]
//        [Optional]
//        public TableSchemaCollection SwitchableObject
//        {
//            get { return _switchableObject; }
//            set
//            {
//                if (value != null)
//                {
//                    _switchableObject = value;
//                    OnSwitchableObjectChanged();
//                }
//            }
//        }

//        #endregion

//        #region 6b. List Entities

//        [Category("6b. List Entities")]
//        [Description("DynamicListBase")]
//        [Optional]
//        public TableSchemaCollection DynamicListBase
//        {
//            get { return this._dynamicListBase; }
//            set
//            {
//                if (value != null)
//                {
//                    this._dynamicListBase = value;
//                    OnDynamicListBaseChanged();
//                }
//            }
//        }

//        [Category("6b. List Entities")]
//        [Description("DynamicRootList")]
//        [Optional]
//        public TableSchemaCollection DynamicRootList
//        {
//            get { return _dynamicRootList; }
//            set
//            {
//                if (value != null)
//                {
//                    _dynamicRootList = value;
//                    OnDynamicRootListChanged();
//                }
//            }
//        }

//        [Category("6b. List Entities")]
//        [Description("EditableRootList")]
//        [Optional]
//        public TableSchemaCollection EditableRootList
//        {
//            get { return _editableRootList; }
//            set
//            {
//                if (value != null)
//                {
//                    _editableRootList = value;
//                    OnEditableRootListChanged();
//                }
//            }
//        }

//        [Category("6b. List Entities")]
//        [Description("EditableChildList")]
//        [Optional]
//        public TableSchemaCollection EditableChildList
//        {
//            get { return _editableChildList; }
//            set
//            {
//                if (value != null)
//                {
//                    _editableChildList = value;
//                    OnEditableChildListChanged();
//                }
//            }
//        }

//        [Category("6b. List Entities")]
//        [Description("ReadOnlyList")]
//        [Optional]
//        public TableSchemaCollection ReadOnlyList
//        {
//            get { return _readOnlyList; }
//            set
//            {
//                if (value != null)
//                {
//                    _readOnlyList = value;
//                    OnReadOnlyListChanged();
//                }
//            }
//        }

//        [Category("6b. List Entities")]
//        [Description("ReadOnlyChildList")]
//        [Optional]
//        public TableSchemaCollection ReadOnlyChildList
//        {
//            get { return _readOnlyChildList; }
//            set
//            {
//                if (value != null)
//                {
//                    _readOnlyChildList = value;
//                    OnReadOnlyChildListChanged();
//                }
//            }
//        }

//        [Category("6b. List Entities")]
//        [Description("NameValueList")]
//        [Optional]
//        public TableSchemaCollection NameValueList
//        {
//            get { return _nameValueList; }
//            set
//            {
//                if (value != null)
//                {
//                    _nameValueList = value;
//                    OnNameListChanged();
//                }
//            }
//        }

//        #endregion

//        #region Private methods

//        #region OnEnitityChanged

//        private void OnDynamicRootChanged()
//        {
//            CleanTemplateContextByValue(Constants.DynamicRoot);

//            EntityManager em = new EntityManager(DynamicRoot);
//            foreach (IEntity entity in em.Entities)
//            {
//                if (ContextData.Get(entity.EntityKeyName) != null)
//                    ContextData.Remove(entity.EntityKeyName);

//                EditableChild.Remove(entity);
//                EditableRoot.Remove(entity);
//                ReadOnlyChild.Remove(entity);
//                ReadOnlyRoot.Remove(entity);
//                SwitchableObject.Remove(entity);

//                ContextData.Add(entity.EntityKeyName, Constants.DynamicRoot);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                //Many-To-One
//                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
//                {
//                    foreach (AssociationProperty property in childEntity.Properties)
//                    {
//                        AddChildEntity(property.Property.Entity, false, false);
//                    }
//                }

//                //One-To-Many & Many-To-Many
//                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
//                {
//                    foreach (AssociationProperty property in childList.Properties)
//                    {
//                        AddChildList(property.Property.Entity, false, false);
//                    }
//                }
//            }
//        }

//        private void OnEditableChildChanged()
//        {
//            CleanTemplateContextByValue(Constants.EditableChild);

//            EntityManager em = new EntityManager(EditableChild);
//            foreach (IEntity entity in em.Entities)
//            {
//                if (ContextData.Get(entity.Name) != null)
//                    ContextData.Remove(entity.Name);

//                DynamicRoot.Remove(entity);
//                EditableRoot.Remove(entity);
//                SwitchableObject.Remove(entity);

//                ContextData.Add(entity.Name, Constants.EditableChild);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                //Many-To-One
//                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
//                {
//                    foreach (AssociationProperty property in childEntity.Properties)
//                    {
//                        AddChildEntity(property.Property.Entity, false, true);
//                    }
//                }

//                //One-To-Many & Many-To-Many
//                foreach (var childList in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToMany))
//                {
//                    foreach (AssociationProperty property in childList.Properties)
//                    {
//                        AddChildList(property.Property.Entity, false, true);
//                    }
//                }
//            }
//        }

//        private void OnEditableRootChanged()
//        {
//            CleanTemplateContextByValue(Constants.EditableRoot);

//            EntityManager em = new EntityManager(EditableRoot);
//            foreach (Entity entity in em.Entities)
//            {
//                if (ContextData.Get(entity.Name) != null)
//                    ContextData.Remove(entity.Name);

//                DynamicRoot.Remove(entity);
//                EditableChild.Remove(entity);
//                SwitchableObject.Remove(entity);

//                ContextData.Add(entity.Name, Constants.EditableRoot);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                //Many-To-One
//                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
//                {
//                    foreach (AssociationProperty property in childEntity)
//                    {
//                    AddChildEntity(property.entity, false, true);
//                    }
//                }

//                //One-To-Many & Many-To-Many
//                foreach (var childList in entity.AssociatedToManyUnion)
//                {
//                    foreach (AssociationProperty property in childList)
//                    {
//                    AddChildList(property.entity, false, true);
//                    }
//                }
//            }
//        }

//        private void OnReadOnlyChildChanged()
//        {
//            CleanTemplateContextByValue(Constants.ReadOnlyChild);

//            EntityManager em = new EntityManager(ReadOnlyChild);
//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ReadOnlyFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                ReadOnlyRoot.Remove(entity);

//                ContextData.Add(key, Constants.ReadOnlyChild);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                //Many-To-One
//                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
//                {
//                    foreach (AssociationProperty property in childEntity)
//                    {
//                    AddChildEntity(property.entity, true, true);
//                    }
//                }

//                //One-To-Many & Many-To-Many
//                foreach (var childList in entity.AssociatedToManyUnion)
//                {
//                    foreach (AssociationProperty property in childList)
//                    {
//                    AddChildList(property.entity, true, true);
//                    }
//                }
//            }
//        }

//        private void OnReadOnlyRootChanged()
//        {
//            CleanTemplateContextByValue(Constants.ReadOnlyRoot);

//            EntityManager em = new EntityManager(ReadOnlyRoot);
//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ReadOnlyFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                ReadOnlyChild.Remove(entity);

//                ContextData.Add(key, Constants.ReadOnlyRoot);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                //Many-To-One
//                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
//                {
//                    foreach (AssociationProperty property in childEntity)
//                    {
//                    AddChildEntity(property.entity, true, true);
//                    }
//                }

//                //One-To-Many & Many-To-Many
//                foreach (var childList in entity.AssociatedToManyUnion)
//                {
//                    foreach (AssociationProperty property in childList)
//                    {
//                    AddChildList(property.entity, true, true);
//                    }
//                }
//            }
//        }

//        private void OnSwitchableObjectChanged()
//        {
//            CleanTemplateContextByValue(Constants.SwitchableObject);

//            EntityManager em = new EntityManager(SwitchableObject);
//            foreach (Entity entity in em.Entities)
//            {
//                if (ContextData.Get(entity.Name) != null)
//                    ContextData.Remove(entity.Name);

//                DynamicRoot.Remove(entity);
//                EditableChild.Remove(entity);
//                EditableRoot.Remove(entity);

//                ContextData.Add(entity.Name, Constants.SwitchableObject);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                //Many-To-One
//                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
//                {
//                    foreach (AssociationProperty property in childEntity)
//                    {
//                    AddChildEntity(property.entity, false, true);
//                    }
//                }

//                //One-To-Many & Many-To-Many
//                foreach (var childList in entity.AssociatedToManyUnion)
//                {
//                    foreach (AssociationProperty property in childList)
//                    {
//                    AddChildList(property.entity, false, true);
//                    }
//                }
//            }
//        }

//        #endregion

//        #region OnListEnitityChanged Methods

//        private void OnDynamicRootListChanged()
//        {
//            CleanTemplateContextByValue(Constants.DynamicRootList);

//            EntityManager em = new EntityManager(DynamicRootList);
//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                EditableRootList.Remove(entity);
//                DynamicListBase.Remove(entity);
//                EditableChildList.Remove(entity);

//                ContextData.Add(key, Constants.DynamicRootList);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                AddChildEntity(entity, false, true);
//            }
//        }

//        private void OnEditableRootListChanged()
//        {
//            CleanTemplateContextByValue(Constants.EditableRootList);

//            EntityManager em = new EntityManager(EditableRootList);
//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                DynamicRootList.Remove(entity);
//                DynamicListBase.Remove(entity);
//                EditableChildList.Remove(entity);

//                ContextData.Add(key, Constants.EditableRootList);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                AddChildEntity(entity, false, true);
//            }
//        }

//        private void OnDynamicListBaseChanged()
//        {
//            CleanTemplateContextByValue(Constants.DynamicListBase);

//            EntityManager em = new EntityManager(DynamicListBase);
//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                DynamicRootList.Remove(entity);
//                EditableRootList.Remove(entity);
//                EditableChildList.Remove(entity);

//                ContextData.Add(key, Constants.DynamicListBase);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                AddChildEntity(entity, false, false);
//            }
//        }

//        private void OnEditableChildListChanged()
//        {
//            CleanTemplateContextByValue(Constants.EditableChildList);

//            EntityManager em = new EntityManager(EditableChildList);

//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                DynamicRootList.Remove(entity);
//                EditableRootList.Remove(entity);
//                DynamicListBase.Remove(entity);

//                ContextData.Add(key, Constants.EditableChildList);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                AddChildEntity(entity, false, true);
//            }
//        }

//        private void OnReadOnlyListChanged()
//        {
//            CleanTemplateContextByValue(Constants.ReadOnlyList);

//            EntityManager em = new EntityManager(ReadOnlyList);

//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ReadOnlyListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                ReadOnlyChildList.Remove(entity);

//                ContextData.Add(key, Constants.ReadOnlyList);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                AddChildEntity(entity, true, true);
//            }
//        }

//        private void OnReadOnlyChildListChanged()
//        {
//            CleanTemplateContextByValue(Constants.ReadOnlyChildList);

//            EntityManager em = new EntityManager(ReadOnlyChildList);

//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ReadOnlyListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                ReadOnlyList.Remove(entity);

//                ContextData.Add(key, Constants.ReadOnlyChildList);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;

//                AddChildEntity(entity, true, true);
//            }
//        }

//        private void OnNameListChanged()
//        {
//            CleanTemplateContextByValue(Constants.NameValueList);

//            EntityManager em = new EntityManager(NameValueList);

//            foreach (Entity entity in em.Entities)
//            {
//                string key = String.Format(Constants.ListFormat, entity.Name);

//                if (ContextData.Get(key) != null)
//                    ContextData.Remove(key);

//                ReadOnlyList.Remove(entity);

//                ContextData.Add(key, Constants.NameValueList);

//                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
//                    continue;
//            }
//        }

//        #endregion

//        #region Helper Methods

//        private void UpdateTableCollections()
//        {
//            if (DynamicRoot == null) DynamicRoot = new TableSchemaCollection();
//            if (EditableChild == null) EditableChild = new TableSchemaCollection();
//            if (EditableRoot == null) EditableRoot = new TableSchemaCollection();
//            if (ReadOnlyChild == null) ReadOnlyChild = new TableSchemaCollection();
//            if (ReadOnlyRoot == null) ReadOnlyRoot = new TableSchemaCollection();
//            if (SwitchableObject == null) SwitchableObject = new TableSchemaCollection();

//            if (DynamicListBase == null) DynamicListBase = new TableSchemaCollection();
//            if (DynamicRootList == null) DynamicRootList = new TableSchemaCollection();
//            if (EditableRootList == null) EditableRootList = new TableSchemaCollection();
//            if (EditableChildList == null) EditableChildList = new TableSchemaCollection();
//            if (ReadOnlyList == null) ReadOnlyList = new TableSchemaCollection();
//            if (ReadOnlyChildList == null) ReadOnlyChildList = new TableSchemaCollection();
//            if (NameValueList == null) NameValueList = new TableSchemaCollection();
//        }

//        private void AddChildList(IEntity entity, bool readOnly, bool child)
//        {
//            if (Configuration.Instance.ExcludeRegexIsMatch(entity.EntityKeyName)) return;

//            if (readOnly)
//            {
//                if (ReadOnlyList.Count > 0 && ReadOnlyList.Contains(entity.Owner, entity.Name))
//                    return;
//                if (ReadOnlyChildList.Count > 0 && ReadOnlyChildList.Contains(entity.Owner, entity.Name))
//                    return;

//                if (child)
//                    ReadOnlyChildList.Add(entity);
//                else
//                    ReadOnlyList.Add(entity);
//            }
//            else
//            {
//                if (DynamicRootList.Count > 0 && DynamicRootList.Contains(entity.Owner, entity.Name))
//                    return;
//                if (EditableRootList.Count > 0 && EditableRootList.Contains(entity.Owner, entity.Name))
//                    return;
//                if (EditableChildList.Count > 0 && EditableChildList.Contains(entity.Owner, entity.Name))
//                    return;
//                if (child)
//                    EditableChildList.Add(entity);
//                else
//                    EditableRootList.Add(entity);
//            }
//        }

//        private void AddChildEntity(IEntity entity, bool readOnly, bool child)
//        {
//            if (Configuration.Instance.ExcludeRegexIsMatch(entity.EntityKeyName)) return;

//            if (readOnly)
//            {
//                if (ReadOnlyChild.Count > 0 && ReadOnlyChild.Contains(entity.Owner, entity.Name))
//                    return;
//                if (ReadOnlyRoot.Count > 0 && ReadOnlyRoot.Contains(entity.Owner, entity.Name))
//                    return;

//                if (child)
//                    ReadOnlyChild.Add(entity);
//                else
//                    ReadOnlyRoot.Add(entity);
//            }
//            else
//            {
//                if (DynamicRoot.Count > 0 && DynamicRoot.Contains(entity.Owner, entity.Name))
//                    return;
//                if (EditableChild.Count > 0 && EditableChild.Contains(entity.Owner, entity.Name))
//                    return;
//                if (EditableRoot.Count > 0 && EditableRoot.Contains(entity.Owner, entity.Name))
//                    return;
//                if (SwitchableObject.Count > 0 && SwitchableObject.Contains(entity.Owner, entity.Name))
//                    return;

//                if (child)
//                    EditableChild.Add(entity);
//                else
//                    EditableRoot.Add(entity);
//            }
//        }


//        private void CleanTemplateContextByValue(string value)
//        {
//            UpdateTableCollections();
//            List<string> keys = (from key in ContextData.AllKeys
//                                 let contextValues = ContextData.GetValues(key)
//                                 where contextValues != null && contextValues.Length > 0 && contextValues[0] == value
//                                 select key).ToList();

//            foreach (string key in keys)
//                ContextData.Remove(key);
//        }

//        #endregion

//        #endregion

//        #region Public Overriden Methods

//        public override void OnDatabaseChanged()
//        {
//            base.OnDatabaseChanged();

//            string basePath = Path.Combine(CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory,
//                                           Path.Combine("CSLA", SourceDatabase.Name));
//            if (Location == basePath)
//                Location = Path.Combine(Location, BusinessProjectName);

//            if (DynamicRoot.Count == 0 &&
//                EditableChild.Count == 0 &&
//                EditableRoot.Count == 0 &&
//                ReadOnlyChild.Count == 0 &&
//                ReadOnlyRoot.Count == 0 &&
//                SwitchableObject.Count == 0 &&
//                DynamicListBase.Count == 0 &&
//                DynamicRootList.Count == 0 &&
//                EditableRootList.Count == 0 &&
//                EditableChildList.Count == 0 &&
//                ReadOnlyList.Count == 0 &&
//                ReadOnlyChildList.Count == 0)
//            {
//                PopulateDefaultTables();
//            }

//        }

//        public void PopulateDefaultTables()
//        {
//            //EditableRoot
//            foreach (IEntity entity in GetRootEntities())
//            {
//                if (!EditableRoot.Contains(entity.Owner, entity.Name))
//                    EditableRoot.Add(entity);
//            }

//            //EditableChild
//            foreach (IEntity entity in GetChildEntities().Values)
//            {
//                if (!EditableChild.Contains(entity.Owner, entity.Name))
//                    EditableChild.Add(entity);
//            }

//            //EditableChild
//            foreach (IEntity entity in GetExcludedEntities())
//            {
//                if (!EditableChild.Contains(entity.Owner, entity.Name))
//                    EditableChild.Add(entity);
//            }

//            //EditableChildList
//            foreach (IEntity entity in GetListEntities().Values)
//            {
//                if (!EditableChildList.Contains(entity.Owner, entity.Name)) 
//                    EditableChildList.Add(entity);
//            }

//            //Criteria
//            foreach (IEntity entity in Entities)
//            {
//                if (!Criteria.Contains(entity.Owner, entity.Name))
//                    Criteria.Add(entity);
//            }
//        }

//        #endregion
//    }
//}