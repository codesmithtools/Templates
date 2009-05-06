using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using SchemaExplorer;

namespace QuickStart
{
    public class EntityMasterCodeTemplate : QuickStartCodeTemplate
    {
        public EntityMasterCodeTemplate()
        {
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
        public TableSchemaCollection NameValueList { get; set; }

        #endregion

        #region Private methods

        #region OnEnitityChanged

        private void OnDynamicRootChanged()
        {
            CleanTemplateContextByValue("DynamicRoot");

            EntityManager em = new EntityManager(DynamicRoot);
            foreach (Entity entity in em.Entities)
            {
                if (TemplateContext.ContainsKey(entity.ClassName))
                    TemplateContext.Remove(entity.ClassName);

                EditableChild.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                ReadOnlyChild.Remove(entity.Table);
                ReadOnlyRoot.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                TemplateContext.Add(entity.ClassName, "DynamicRoot");

                if (this.State != TemplateState.Default)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.ManyToOne)
                {
                    var table = new TableSchema(SourceDatabase, childEntity.TableName, childEntity.TableOwner,
                                                DateTime.Now);
                    AddChildEntity(table, false, false);
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, childList.TableName, childList.TableOwner, DateTime.Now);
                    AddChildList(table, false, false);
                }
            }
        }

        private void OnEditableChildChanged()
        {
            CleanTemplateContextByValue("EditableChild");

            EntityManager em = new EntityManager(EditableChild);
            foreach (Entity entity in em.Entities)
            {
                if (TemplateContext.ContainsKey(entity.ClassName))
                    TemplateContext.Remove(entity.ClassName);

                DynamicRoot.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                ReadOnlyChild.Remove(entity.Table);
                ReadOnlyRoot.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                TemplateContext.Add(entity.ClassName, "EditableChild");

                if (this.State != TemplateState.Default)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.ManyToOne)
                {
                    var table = new TableSchema(SourceDatabase, childEntity.TableName, childEntity.TableOwner,
                                                DateTime.Now);
                    AddChildEntity(table, false, true);
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, childList.TableName, childList.TableOwner, DateTime.Now);
                    AddChildList(table, false, true);
                }
            }
        }

        private void OnEditableRootChanged()
        {
            CleanTemplateContextByValue("EditableRoot");

            EntityManager em = new EntityManager(EditableRoot);
            foreach (Entity entity in em.Entities)
            {
                if (TemplateContext.ContainsKey(entity.ClassName))
                    TemplateContext.Remove(entity.ClassName);

                DynamicRoot.Remove(entity.Table);
                EditableChild.Remove(entity.Table);
                ReadOnlyChild.Remove(entity.Table);
                ReadOnlyRoot.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                TemplateContext.Add(entity.ClassName, "EditableRoot");

                if (this.State != TemplateState.Default)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.ManyToOne)
                {
                    var table = new TableSchema(SourceDatabase, childEntity.TableName, childEntity.TableOwner,
                                                DateTime.Now);
                    AddChildEntity(table, false, true);
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, childList.TableName, childList.TableOwner, DateTime.Now);
                    AddChildList(table, false, true);
                }
            }
        }

        private void OnReadOnlyChildChanged()
        {
            CleanTemplateContextByValue("ReadOnlyChild");

            EntityManager em = new EntityManager(ReadOnlyChild);
            foreach (Entity entity in em.Entities)
            {
                if (TemplateContext.ContainsKey(entity.ClassName))
                    TemplateContext.Remove(entity.ClassName);

                DynamicRoot.Remove(entity.Table);
                EditableChild.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                ReadOnlyRoot.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                TemplateContext.Add(entity.ClassName, "ReadOnlyChild");

                if (this.State != TemplateState.Default)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.ManyToOne)
                {
                    var table = new TableSchema(SourceDatabase, childEntity.TableName, childEntity.TableOwner,
                                                DateTime.Now);
                    AddChildEntity(table, true, true);
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, childList.TableName, childList.TableOwner, DateTime.Now);
                    AddChildList(table, true, true);
                }
            }
        }

        private void OnReadOnlyRootChanged()
        {
            CleanTemplateContextByValue("ReadOnlyRoot");

            EntityManager em = new EntityManager(ReadOnlyRoot);
            foreach (Entity entity in em.Entities)
            {
                if (TemplateContext.ContainsKey(entity.ClassName))
                    TemplateContext.Remove(entity.ClassName);

                DynamicRoot.Remove(entity.Table);
                EditableChild.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                ReadOnlyChild.Remove(entity.Table);
                SwitchableObject.Remove(entity.Table);

                TemplateContext.Add(entity.ClassName, "ReadOnlyRoot");

                if (this.State != TemplateState.Default)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.ManyToOne)
                {
                    var table = new TableSchema(SourceDatabase, childEntity.TableName, childEntity.TableOwner,
                                                DateTime.Now);
                    AddChildEntity(table, true, true);
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, childList.TableName, childList.TableOwner, DateTime.Now);
                    AddChildList(table, true, true);
                }
            }
        }

        private void OnSwitchableObjectChanged()
        {
            CleanTemplateContextByValue("SwitchableObject");

            EntityManager em = new EntityManager(SwitchableObject);
            foreach (Entity entity in em.Entities)
            {
                if (TemplateContext.ContainsKey(entity.ClassName))
                    TemplateContext.Remove(entity.ClassName);

                DynamicRoot.Remove(entity.Table);
                EditableChild.Remove(entity.Table);
                EditableRoot.Remove(entity.Table);
                ReadOnlyChild.Remove(entity.Table);
                ReadOnlyRoot.Remove(entity.Table);

                TemplateContext.Add(entity.ClassName, "SwitchableObject");

                if (this.State != TemplateState.Default)
                    return;

                //Many-To-One
                foreach (var childEntity in entity.ManyToOne)
                {
                    var table = new TableSchema(SourceDatabase, childEntity.TableName, childEntity.TableOwner,
                                                DateTime.Now);
                    AddChildEntity(table, false, true);
                }

                //One-To-Many & Many-To-Many
                foreach (var childList in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, childList.TableName, childList.TableOwner, DateTime.Now);
                    AddChildList(table, false, true);
                }
            }
        }

        #endregion

        #region OnListEnitityChanged Methods

        private void OnDynamicRootListChanged()
        {
            CleanTemplateContextByValue("DynamicRootList");

            EntityManager em = new EntityManager(DynamicRootList);
            foreach (Entity entity in em.Entities)
            {
                string key = string.Format("{0}List", entity.ClassName);

                if (TemplateContext.ContainsKey(key))
                    TemplateContext.Remove(key);

                EditableRootList.Remove(entity.Table);
                EditableChildList.Remove(entity.Table);
                ReadOnlyList.Remove(entity.Table);
                ReadOnlyChildList.Remove(entity.Table);

                TemplateContext.Add(key, "DynamicRootList");

                if (this.State != TemplateState.Default)
                    return;

                AddChildEntity(entity.Table, false, true);
            }
        }

        private void OnEditableRootListChanged()
        {
            CleanTemplateContextByValue("EditableRootList");

            EntityManager em = new EntityManager(EditableRootList);
            foreach (Entity entity in em.Entities)
            {
                string key = string.Format("{0}List", entity.ClassName);

                if (TemplateContext.ContainsKey(key))
                    TemplateContext.Remove(key);

                DynamicRootList.Remove(entity.Table);
                EditableChildList.Remove(entity.Table);
                ReadOnlyList.Remove(entity.Table);
                ReadOnlyChildList.Remove(entity.Table);

                TemplateContext.Add(key, "EditableRootList");

                if (this.State != TemplateState.Default)
                    return;

                AddChildEntity(entity.Table, false, true);
            }
        }

        private void OnEditableChildListChanged()
        {
            CleanTemplateContextByValue("EditableChildList");

            EntityManager em = new EntityManager(EditableChildList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format("{0}List", entity.ClassName);

                if (TemplateContext.ContainsKey(key))
                    TemplateContext.Remove(key);

                EditableRootList.Remove(entity.Table);
                DynamicRootList.Remove(entity.Table);
                ReadOnlyList.Remove(entity.Table);
                ReadOnlyChildList.Remove(entity.Table);

                TemplateContext.Add(key, "EditableChildList");

                if (this.State != TemplateState.Default)
                    return;

                AddChildEntity(entity.Table, false, true);
            }
        }

        private void OnReadOnlyListChanged()
        {
            CleanTemplateContextByValue("ReadOnlyList");

            EntityManager em = new EntityManager(ReadOnlyList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format("{0}List", entity.ClassName);

                if (TemplateContext.ContainsKey(key))
                    TemplateContext.Remove(key);

                EditableRootList.Remove(entity.Table);
                EditableChildList.Remove(entity.Table);
                DynamicRootList.Remove(entity.Table);
                ReadOnlyChildList.Remove(entity.Table);

                TemplateContext.Add(key, "ReadOnlyList");

                if (this.State != TemplateState.Default)
                    return;

                AddChildEntity(entity.Table, true, true);
            }
        }

        private void OnReadOnlyChildListChanged()
        {
            CleanTemplateContextByValue("ReadOnlyChildList");

            EntityManager em = new EntityManager(ReadOnlyChildList);

            foreach (Entity entity in em.Entities)
            {
                string key = string.Format("{0}List", entity.ClassName);

                if (TemplateContext.ContainsKey(key))
                    TemplateContext.Remove(key);

                EditableRootList.Remove(entity.Table);
                EditableChildList.Remove(entity.Table);
                DynamicRootList.Remove(entity.Table);
                ReadOnlyList.Remove(entity.Table);

                TemplateContext.Add(key, "ReadOnlyChildList");

                if (this.State != TemplateState.Default)
                    return;

                AddChildEntity(entity.Table, true, true);
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateTableCollections()
        {
            if (CommandObject == null) CommandObject = new TableSchemaCollection();
            if (Criteria == null) Criteria = new TableSchemaCollection();
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
            if (DynamicRootList.Count > 0 && DynamicRootList.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (EditableRootList.Count > 0 && EditableRootList.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (EditableChildList.Count > 0 && EditableChildList.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (ReadOnlyList.Count > 0 && ReadOnlyList.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (ReadOnlyChildList.Count > 0 && ReadOnlyChildList.Contains(tableSchema.Owner, tableSchema.Name))
                return;

            if (readOnly)
            {
                if (child)
                    ReadOnlyChildList.Add(tableSchema);
                else
                    ReadOnlyList.Add(tableSchema);
            }
            else
            {
                if (child)
                    EditableChildList.Add(tableSchema);
                else
                    EditableRootList.Add(tableSchema);
            }
        }

        private void AddChildEntity(TableSchema tableSchema, bool readOnly, bool child)
        {
           if (DynamicRoot.Count > 0 && DynamicRoot.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (EditableChild.Count > 0 && EditableChild.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (EditableRoot.Count > 0 && EditableRoot.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (ReadOnlyChild.Count > 0 && ReadOnlyChild.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (ReadOnlyRoot.Count > 0 && ReadOnlyRoot.Contains(tableSchema.Owner, tableSchema.Name))
                return;
            if (SwitchableObject.Count > 0 && SwitchableObject.Contains(tableSchema.Owner, tableSchema.Name))
                return;

            if (readOnly)
            {
                if (child)
                    ReadOnlyChild.Add(tableSchema);
                else
                    ReadOnlyRoot.Add(tableSchema);
            }
            else
            {
                if (child)
                    EditableChild.Add(tableSchema);
                else
                    EditableRoot.Add(tableSchema);
            }
        }


        private void CleanTemplateContextByValue(string value)
        {
            UpdateTableCollections();

            if (TemplateContext == null)
                TemplateContext = new Dictionary<string, string>();

            List<string> keys = new List<string>();

            foreach (KeyValuePair<string, string> pair in TemplateContext)
            {
                if (pair.Value == value)
                    keys.Add(pair.Key);
            }

            foreach (string key in keys)
                TemplateContext.Remove(key);
        }

        #endregion

        #endregion

        #region Public Overriden Methods

        public override void OnDatabaseChanged()
        {

            if (this.State != TemplateState.Default)
                return;

            base.OnDatabaseChanged();

            string basePath = Path.Combine(CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory,
                                           Path.Combine("CSLA", SourceDatabase.Name));
            if (Location == basePath)
                Location = Path.Combine(Location, BusinessProjectName);

            //EditableChild
            foreach (Entity entity in GetChildEntities())
            {
                if (!EditableChild.Contains(entity.Table))
                    EditableChild.Add(entity.Table);
            }

            //EditableChildList
            foreach (Entity entity in GetListEntities())
            {
                foreach (AssociationMember association in entity.ToManyUnion)
                {
                    var table = new TableSchema(SourceDatabase, association.TableName, entity.Table.Owner,
                                                DateTime.Now);

                    if (!EditableChildList.Contains(table))
                        EditableChildList.Add(table);
                }
            }

            //EditableRoot
            foreach (Entity entity in GetEntities())
            {
                if (!EditableRoot.Contains(entity.Table))
                    EditableRoot.Add(entity.Table);
            }

            //Criteria
            foreach (Entity entity in Entities)
            {
                if (!Criteria.Contains(entity.Table))
                    Criteria.Add(entity.Table);
            }
        }

        #endregion
    }
}