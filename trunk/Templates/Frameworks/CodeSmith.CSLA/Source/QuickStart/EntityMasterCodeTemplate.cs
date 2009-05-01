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
            CommandObject = new TableSchemaCollection();
            Criteria = new TableSchemaCollection();
            NameValueList = new TableSchemaCollection();
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
            }
        }

        #endregion

        private void CleanTemplateContextByValue(string value)
        {
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

        #region Public Overriden Methods

        public override void OnDatabaseChanged()
        {
            if (string.IsNullOrEmpty(Location))
                Location = Path.Combine(Path.Combine(
                    CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory,
                    Path.Combine("CSLA", SourceDatabase.Name)), BusinessProjectName);

            base.OnDatabaseChanged();

            //EditableChild
            foreach (Entity entity in GetChildEntities())
            {
                EditableChild.Add(entity.Table);
            }

            //EditableChildList
            foreach (Entity entity in GetListEntities())
            {
                foreach (AssociationMember association in entity.ToManyUnion)
                {
                    EditableChildList.Add(new TableSchema(SourceDatabase, association.TableName, entity.Table.Owner,
                                                          DateTime.Now));
                }
            }

            //EditableRoot
            foreach (Entity entity in GetEntities())
            {
                EditableRoot.Add(entity.Table);
            }

            //Criteria
            foreach (Entity entity in Entities)
            {
                Criteria.Add(entity.Table);
            }
        }

        #endregion
    }
}