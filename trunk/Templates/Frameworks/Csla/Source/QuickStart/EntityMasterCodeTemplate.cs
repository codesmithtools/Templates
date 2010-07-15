using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using SchemaExplorer;

namespace QuickStart
{
    public class EntityMasterCodeTemplate : QuickStartCodeTemplate
    {
        public EntityMasterCodeTemplate()
        {
            Criteria = new TableSchemaCollection();
            CommandObject = new TableSchemaCollection();
            UpdateTableCollections();
        }

        #region Private Member(s)

        private TableSchemaCollection _dynamicRoot = new TableSchemaCollection();
        private TableSchemaCollection _editableChild = new TableSchemaCollection();
        private TableSchemaCollection _editableRoot = new TableSchemaCollection();
        private TableSchemaCollection _readOnlyChild = new TableSchemaCollection();
        private TableSchemaCollection _readOnlyRoot = new TableSchemaCollection();
        private TableSchemaCollection _switchableObject = new TableSchemaCollection();
        private TableSchemaCollection _dynamicRootList = new TableSchemaCollection();
        private TableSchemaCollection _editableRootList = new TableSchemaCollection();
        private TableSchemaCollection _editableChildList = new TableSchemaCollection();
        private TableSchemaCollection _readOnlyList = new TableSchemaCollection();
        private TableSchemaCollection _readOnlyChildList = new TableSchemaCollection();
        private TableSchemaCollection _nameValueList = new TableSchemaCollection();

        #endregion

        #region 6a. Entities

        [Category("6a. Entities")]
        [Description("CommandObject")]
        [Optional]
        public TableSchemaCollection CommandObject { get; set; }

        [Category("6a. Entities")]
        [Description("Criteria")]
        [Optional]
        public TableSchemaCollection Criteria { get; set; }

        [Category("6a. Entities")]
        [Description("DynamicRoot")]
        [Optional]
        public TableSchemaCollection DynamicRoot
        {
            get { return _dynamicRoot; }
            set
            {
                if (value != null)
                {
                    _dynamicRoot = value;
                    OnDynamicRootChanged();
                }
            }
        }

        [Category("6a. Entities")]
        [Description("EditableChild")]
        [Optional]
        public TableSchemaCollection EditableChild
        {
            get { return _editableChild; }
            set
            {
                if (value != null)
                {
                    _editableChild = value;
                    OnEditableChildChanged();
                }
            }
        }

        [Category("6a. Entities")]
        [Description("EditableRoot")]
        [Optional]
        public TableSchemaCollection EditableRoot
        {
            get { return _editableRoot; }
            set
            {
                if (value != null)
                {
                    _editableRoot = value;
                    OnEditableRootChanged();
                }
            }
        }

        [Category("6a. Entities")]
        [Description("ReadOnlyChild")]
        [Optional]
        public TableSchemaCollection ReadOnlyChild
        {
            get { return _readOnlyChild; }
            set
            {
                if (value != null)
                {
                    _readOnlyChild = value;
                    OnReadOnlyChildChanged();
                }
            }
        }

        [Category("6a. Entities")]
        [Description("ReadOnlyRoot")]
        [Optional]
        public TableSchemaCollection ReadOnlyRoot
        {
            get { return _readOnlyRoot; }
            set
            {
                if (value != null)
                {
                    _readOnlyRoot = value;
                    OnReadOnlyRootChanged();
                }
            }
        }

        [Category("6a. Entities")]
        [Description("SwitchableObject")]
        [Optional]
        public TableSchemaCollection SwitchableObject
        {
            get { return _switchableObject; }
            set
            {
                if (value != null)
                {
                    _switchableObject = value;
                    OnSwitchableObjectChanged();
                }
            }
        }

        #endregion

        #region 6b. List Entities

        [Category("6b. List Entities")]
        [Description("DynamicRootList")]
        [Optional]
        public TableSchemaCollection DynamicRootList
        {
            get { return _dynamicRootList; }
            set
            {
                if (value != null)
                {
                    _dynamicRootList = value;
                    OnDynamicRootListChanged();
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("EditableRootList")]
        [Optional]
        public TableSchemaCollection EditableRootList
        {
            get { return _editableRootList; }
            set
            {
                if (value != null)
                {
                    _editableRootList = value;
                    OnEditableRootListChanged();
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("EditableChildList")]
        [Optional]
        public TableSchemaCollection EditableChildList
        {
            get { return _editableChildList; }
            set
            {
                if (value != null)
                {
                    _editableChildList = value;
                    OnEditableChildListChanged();
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("ReadOnlyList")]
        [Optional]
        public TableSchemaCollection ReadOnlyList
        {
            get { return _readOnlyList; }
            set
            {
                if (value != null)
                {
                    _readOnlyList = value;
                    OnReadOnlyListChanged();
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("ReadOnlyChildList")]
        [Optional]
        public TableSchemaCollection ReadOnlyChildList
        {
            get { return _readOnlyChildList; }
            set
            {
                if (value != null)
                {
                    _readOnlyChildList = value;
                    OnReadOnlyChildListChanged();
                }
            }
        }

        [Category("6b. List Entities")]
        [Description("NameValueList")]
        [Optional]
        public TableSchemaCollection NameValueList
        {
            get { return _nameValueList; }
            set
            {
                if (value != null)
                {
                    _nameValueList = value;
                    OnNameListChanged();
                }
            }
        }

        #endregion

        #region Private methods

        #region OnEnitityChanged

        private void OnDynamicRootChanged()
        {
            CleanTemplateContextByValue(Constants.DynamicRoot);

            EntityManager em = new EntityManager(DynamicRoot);
            foreach (Entity entity in em.Entities)
            {
                if (ContextData.Get(entity.Table.Name) != null)
                    ContextData.Remove(entity.Table.Name);

                EditableChild.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                ReadOnlyChild.Remove(entity.Table);
                ReadOnlyRoot.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                ContextData.Add(entity.Table.Name, Constants.DynamicRoot);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.AssociatedManyToOne)
                {
                    foreach (AssociationMember member in childEntity)
                    {
                        AddChildEntity(member.Entity.Table, false, false);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.AssociatedToManyUnion)
                {
                    foreach (AssociationMember member in childList)
                    {
                        AddChildList(member.Entity.Table, false, false);
                    }
                }
            }
        }

        private void OnEditableChildChanged()
        {
            CleanTemplateContextByValue(Constants.EditableChild);

            EntityManager em = new EntityManager(EditableChild);
            foreach (Entity entity in em.Entities)
            {
                if (ContextData.Get(entity.Table.Name) != null)
                    ContextData.Remove(entity.Table.Name);

                DynamicRoot.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                ContextData.Add(entity.Table.Name, Constants.EditableChild);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.AssociatedManyToOne)
                {
                    foreach (AssociationMember member in childEntity)
                    {
                    AddChildEntity(member.Entity.Table, false, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.AssociatedToManyUnion)
                {
                    foreach (AssociationMember member in childList)
                    {
                    AddChildList(member.Entity.Table, false, true);
                    }
                }
            }
        }

        private void OnEditableRootChanged()
        {
            CleanTemplateContextByValue(Constants.EditableRoot);

            EntityManager em = new EntityManager(EditableRoot);
            foreach (Entity entity in em.Entities)
            {
                if (ContextData.Get(entity.Table.Name) != null)
                    ContextData.Remove(entity.Table.Name);

                DynamicRoot.Remove(entity.Table);
                EditableChild.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                ContextData.Add(entity.Table.Name, Constants.EditableRoot);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.AssociatedManyToOne)
                {
                    foreach (AssociationMember member in childEntity)
                    {
                    AddChildEntity(member.Entity.Table, false, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.AssociatedToManyUnion)
                {
                    foreach (AssociationMember member in childList)
                    {
                    AddChildList(member.Entity.Table, false, true);
                    }
                }
            }
        }

        private void OnReadOnlyChildChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyChild);

            EntityManager em = new EntityManager(ReadOnlyChild);
            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ReadOnlyFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyRoot.Remove(entity.Table);

                ContextData.Add(key, Constants.ReadOnlyChild);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.AssociatedManyToOne)
                {
                    foreach (AssociationMember member in childEntity)
                    {
                    AddChildEntity(member.Entity.Table, true, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.AssociatedToManyUnion)
                {
                    foreach (AssociationMember member in childList)
                    {
                    AddChildList(member.Entity.Table, true, true);
                    }
                }
            }
        }

        private void OnReadOnlyRootChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyRoot);

            EntityManager em = new EntityManager(ReadOnlyRoot);
            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ReadOnlyFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyChild.Remove(entity.Table);

                ContextData.Add(key, Constants.ReadOnlyRoot);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.AssociatedManyToOne)
                {
                    foreach (AssociationMember member in childEntity)
                    {
                    AddChildEntity(member.Entity.Table, true, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.AssociatedToManyUnion)
                {
                    foreach (AssociationMember member in childList)
                    {
                    AddChildList(member.Entity.Table, true, true);
                    }
                }
            }
        }

        private void OnSwitchableObjectChanged()
        {
            CleanTemplateContextByValue(Constants.SwitchableObject);

            EntityManager em = new EntityManager(SwitchableObject);
            foreach (Entity entity in em.Entities)
            {
                if (ContextData.Get(entity.Table.Name) != null)
                    ContextData.Remove(entity.Table.Name);

                DynamicRoot.Remove(entity.Table);
                EditableChild.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);

                ContextData.Add(entity.Table.Name, Constants.SwitchableObject);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.AssociatedManyToOne)
                {
                    foreach (AssociationMember member in childEntity)
                    {
                    AddChildEntity(member.Entity.Table, false, true);
                    }
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.AssociatedToManyUnion)
                {
                    foreach (AssociationMember member in childList)
                    {
                    AddChildList(member.Entity.Table, false, true);
                    }
                }
            }
        }

        #endregion

        #region OnListEnitityChanged Methods

        private void OnDynamicRootListChanged()
        {
            CleanTemplateContextByValue(Constants.DynamicRootList);

            EntityManager em = new EntityManager(DynamicRootList);
            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ListFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                EditableRootList.Remove(entity.Table);
                EditableChildList.Remove(entity.Table);

                ContextData.Add(key, Constants.DynamicRootList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                AddChildEntity(entity.Table, false, true);
            }
        }

        private void OnEditableRootListChanged()
        {
            CleanTemplateContextByValue(Constants.EditableRootList);

            EntityManager em = new EntityManager(EditableRootList);
            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ListFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                DynamicRootList.Remove(entity.Table);
                EditableChildList.Remove(entity.Table);
                ReadOnlyList.Remove(entity.Table);
                ReadOnlyChildList.Remove(entity.Table);

                ContextData.Add(key, Constants.EditableRootList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                AddChildEntity(entity.Table, false, false);
            }
        }

        private void OnEditableChildListChanged()
        {
            CleanTemplateContextByValue(Constants.EditableChildList);

            EntityManager em = new EntityManager(EditableChildList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ListFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                EditableRootList.Remove(entity.Table);
                DynamicRootList.Remove(entity.Table);

                ContextData.Add(key, Constants.EditableChildList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                AddChildEntity(entity.Table, false, true);
            }
        }

        private void OnReadOnlyListChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyList);

            EntityManager em = new EntityManager(ReadOnlyList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ReadOnlyListFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyChildList.Remove(entity.Table);

                ContextData.Add(key, Constants.ReadOnlyList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                AddChildEntity(entity.Table, true, true);
            }
        }

        private void OnReadOnlyChildListChanged()
        {
            CleanTemplateContextByValue(Constants.ReadOnlyChildList);

            EntityManager em = new EntityManager(ReadOnlyChildList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ReadOnlyListFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyList.Remove(entity.Table);

                ContextData.Add(key, Constants.ReadOnlyChildList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;

                AddChildEntity(entity.Table, true, true);
            }
        }

        private void OnNameListChanged()
        {
            CleanTemplateContextByValue(Constants.NameValueList);

            EntityManager em = new EntityManager(NameValueList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format(Constants.ListFormat, entity.Table.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyList.Remove(entity.Table);

                ContextData.Add(key, Constants.NameValueList);

                if (this.State == TemplateState.RestoringProperties || SourceDatabase == null)
                    return;
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

            if (DynamicRootList == null) DynamicRootList = new TableSchemaCollection();
            if (EditableRootList == null) EditableRootList = new TableSchemaCollection();
            if (EditableChildList == null) EditableChildList = new TableSchemaCollection();
            if (ReadOnlyList == null) ReadOnlyList = new TableSchemaCollection();
            if (ReadOnlyChildList == null) ReadOnlyChildList = new TableSchemaCollection();
            if (NameValueList == null) NameValueList = new TableSchemaCollection();
        }

        private void AddChildList(TableSchema tableSchema, bool readOnly, bool child)
        {
            if (readOnly)
            {
                if (ReadOnlyList.Count > 0 && ReadOnlyList.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (ReadOnlyChildList.Count > 0 && ReadOnlyChildList.Contains(tableSchema.Owner, tableSchema.Name))
                    return;

                if (child)
                    ReadOnlyChildList.Add(tableSchema);
                else
                    ReadOnlyList.Add(tableSchema);
            }
            else
            {
                if (DynamicRootList.Count > 0 && DynamicRootList.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (EditableRootList.Count > 0 && EditableRootList.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (EditableChildList.Count > 0 && EditableChildList.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (child)
                    EditableChildList.Add(tableSchema);
                else
                    EditableRootList.Add(tableSchema);
            }
        }

        private void AddChildEntity(TableSchema tableSchema, bool readOnly, bool child)
        {
            if (readOnly)
            {
                if (ReadOnlyChild.Count > 0 && ReadOnlyChild.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (ReadOnlyRoot.Count > 0 && ReadOnlyRoot.Contains(tableSchema.Owner, tableSchema.Name))
                    return;

                if (child)
                    ReadOnlyChild.Add(tableSchema);
                else
                    ReadOnlyRoot.Add(tableSchema);
            }
            else
            {
                if (DynamicRoot.Count > 0 && DynamicRoot.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (EditableChild.Count > 0 && EditableChild.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (EditableRoot.Count > 0 && EditableRoot.Contains(tableSchema.Owner, tableSchema.Name))
                    return;
                if (SwitchableObject.Count > 0 && SwitchableObject.Contains(tableSchema.Owner, tableSchema.Name))
                    return;

                if (child)
                    EditableChild.Add(tableSchema);
                else
                    EditableRoot.Add(tableSchema);
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
            //EditableChild
            foreach (Entity entity in GetChildEntities().Values)
            {
                if (!EditableChild.Contains(entity.Table.Owner, entity.Table.Name))
                    EditableChild.Add(entity.Table);
            }

            //EditableChildList
            foreach (Entity entity in GetListEntities().Values)
            {
                if (!EditableChildList.Contains(entity.Table.Owner, entity.Table.Name)) 
                    EditableChildList.Add(entity.Table);
            }

            //EditableRoot
            foreach (Entity entity in GetEntities())
            {
                if (!EditableRoot.Contains(entity.Table.Owner, entity.Table.Name))
                    EditableRoot.Add(entity.Table);
            }

            //Criteria
            foreach (Entity entity in Entities)
            {
                if (!Criteria.Contains(entity.Table.Owner, entity.Table.Name))
                    Criteria.Add(entity.Table);
            }
        }

        #endregion
    }
}