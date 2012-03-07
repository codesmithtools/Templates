using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA.CodeTemplates
{
    public class EntitiesCodeTemplate : CSLABaseTemplate
    {
        public EntitiesCodeTemplate()
        {
        }

        [Browsable(false)]
        public List<IEntity> CommandObjectEntities = new List<IEntity>();

        [Browsable(false)]
        public List<IEntity> CriteriaEntities
        {
            get { return DynamicRootEntities
                .Union(EditableChildEntities)
                .Union(EditableRootEntities)
                .Union(ReadOnlyChildEntities)
                .Union(ReadOnlyRootEntities)
                .Union(SwitchableObjectEntities)
                .Union(DynamicRootListEntities)
                .Union(EditableRootListEntities)
                .Union(EditableChildListEntities)
                .Union(ReadOnlyListEntities)
                .Union(ReadOnlyChildListEntities)
                .Union(NameValueListEntities).ToList();
            }
        }

        private List<IEntity> _dynamicRootEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> DynamicRootEntities
        {
            get { return _dynamicRootEntities; }
            set
            {
                if (value != null)
                {
                    _dynamicRootEntities = value; 
                    OnDynamicRootChanged();
                }
            }
        }
        
        private List<IEntity> _editableChildEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableChildEntities
        {
            get { return _editableChildEntities; }
            set
            {
                if (value != null)
                {
                    _editableChildEntities = value;
                    OnEditableChildChanged();
                }
            }
        }

        private List<IEntity> _editableRootEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableRootEntities
        {
            get { return _editableRootEntities; }
            set
            {
                if (value != null)
                {
                    _editableRootEntities = value;
                    OnEditableRootChanged();
                }
            }
        }

        private List<IEntity> _readOnlyChildEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyChildEntities
        {
            get { return _readOnlyChildEntities; }
            set
            {
                if (value != null)
                {
                    _readOnlyChildEntities = value;
                    OnReadOnlyChildChanged();
                }
            }
        }

        private List<IEntity> _readOnlyRootEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyRootEntities
        {
            get { return _readOnlyRootEntities; }
            set
            {
                if (value != null)
                {
                    _readOnlyRootEntities = value;
                    OnReadOnlyRootChanged();
                }
            }
        }

        private List<IEntity> _switchableObjectEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> SwitchableObjectEntities
        {
            get { return _switchableObjectEntities; }
            set
            {
                if (value != null)
                {
                    _switchableObjectEntities = value;
                    OnSwitchableObjectChanged();
                }
            }
        }

        private List<IEntity> _dynamicListBaseEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> DynamicListBaseEntities
        {
            get { return _dynamicListBaseEntities; }
            set
            {
                if (value != null)
                {
                    _dynamicListBaseEntities = value;
                    OnDynamicListBaseChanged();
                }
            }
        }

        private List<IEntity> _dynamicRootListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> DynamicRootListEntities
        {
            get { return _dynamicRootListEntities; }
            set
            {
                if (value != null)
                {
                    _dynamicRootListEntities = value;
                    OnDynamicRootListChanged();
                }
            }
        }

        private List<IEntity> _editableRootListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableRootListEntities
        {
            get { return _editableRootListEntities; }
            set
            {
                if (value != null)
                {
                    _editableRootListEntities = value;
                    OnEditableRootListChanged();
                }
            }
        }

        private List<IEntity> _editableChildListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> EditableChildListEntities
        {
            get { return _editableChildListEntities; }
            set
            {
                if (value != null)
                {
                    _editableChildListEntities = value;
                    OnEditableChildListChanged();
                }
            }
        }

        private List<IEntity> _readOnlyListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyListEntities
        {
            get { return _readOnlyListEntities; }
            set
            {
                if (value != null)
                {
                    _readOnlyListEntities = value;
                    OnReadOnlyListChanged();
                }
            }
        }

        private List<IEntity> _readOnlyChildListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> ReadOnlyChildListEntities
        {
            get { return _readOnlyChildListEntities; }
            set
            {
                if (value != null)
                {
                    _readOnlyChildListEntities = value;
                    OnReadOnlyChildListChanged();
                }
            }
        }

        private List<IEntity> _nameValueListEntities = new List<IEntity>();
        [Browsable(false)]
        public List<IEntity> NameValueListEntities
        {
            get { return _nameValueListEntities; }
            set
            {
                if (value != null)
                {
                    _nameValueListEntities = value;
                    OnNameListChanged();
                }
            }
        }

        public void PopulateDefaultEntities(List<IEntity> entities)
        {
            //EditableRoot
            foreach (var entity in GetRootEntities(entities))
            {
                if (!EditableRootEntities.Contains(entity))
                    EditableRootEntities.Add(entity);
            }

            //EditableChild
            foreach (var entity in GetChildEntities(entities).Values)
            {
                if (!EditableChildEntities.Contains(entity))
                    EditableChildEntities.Add(entity);
            }

            //EditableChild
            foreach (var entity in GetExcludedEntities(entities))
            {
                if (!EditableChildEntities.Contains(entity))
                    EditableChildEntities.Add(entity);
            }

            //EditableChildList
            foreach (var entity in GetListEntities(entities).Values)
            {
                if (!EditableChildListEntities.Contains(entity))
                    EditableChildListEntities.Add(entity);
            }
        }

        private void OnDynamicRootChanged()
        {
            CleanTemplateContextByValue(Constants.DynamicRoot);
            foreach (var entity in DynamicRootEntities)
            {
                if (ContextData.Get(entity.EntityKeyName) != null)
                    ContextData.Remove(entity.EntityKeyName);

                EditableChildEntities.Remove(entity);
                EditableRootEntities.Remove(entity);
                SwitchableObjectEntities.Remove(entity);

                ContextData.Add(entity.EntityKeyName, Constants.DynamicRoot);

                if (State == TemplateState.RestoringProperties)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
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

                if (State == TemplateState.RestoringProperties)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
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

                if (State == TemplateState.RestoringProperties)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
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

                if (State == TemplateState.RestoringProperties)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
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

                if (State == TemplateState.RestoringProperties)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
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

                if (State == TemplateState.RestoringProperties)
                    continue;

                //Many-To-One
                foreach (var childEntity in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
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

                if (State == TemplateState.RestoringProperties)
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

                if (State == TemplateState.RestoringProperties)
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

                if (State == TemplateState.RestoringProperties)
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

                if (State == TemplateState.RestoringProperties)
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

                if (State == TemplateState.RestoringProperties)
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

                if (State == TemplateState.RestoringProperties)
                    continue;

                AddChildEntity(entity, true, true);
            }
        }

        private void OnNameListChanged()
        {
            CleanTemplateContextByValue(Constants.NameValueList);

            foreach (var entity in NameValueListEntities)
            {
                string key = String.Format(Constants.ListFormat, entity.Name);

                if (ContextData.Get(key) != null)
                    ContextData.Remove(key);

                ReadOnlyListEntities.Remove(entity);

                ContextData.Add(key, Constants.NameValueList);
            }
        }

        private void AddChildList(IEntity entity, bool readOnly, bool child)
        {
            if (Configuration.Instance.ExcludeRegexIsMatch(entity.EntityKey()))
                return;

            if (readOnly)
            {
                if (ReadOnlyListEntities.Count > 0 && ReadOnlyListEntities.Contains(entity))
                    return;
                if (ReadOnlyChildListEntities.Count > 0 && ReadOnlyChildListEntities.Contains(entity))
                    return;

                if (child)
                    ReadOnlyChildListEntities.Add(entity);
                else
                    ReadOnlyListEntities.Add(entity);
            }
            else
            {
                if (DynamicRootListEntities.Count > 0 && DynamicRootListEntities.Contains(entity))
                    return;
                if (EditableRootListEntities.Count > 0 && EditableRootListEntities.Contains(entity))
                    return;
                if (EditableChildListEntities.Count > 0 && EditableChildListEntities.Contains(entity))
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
                if (ReadOnlyChildEntities.Count > 0 && ReadOnlyChildEntities.Contains(entity))
                    return;
                if (ReadOnlyRootEntities.Count > 0 && ReadOnlyRootEntities.Contains(entity))
                    return;

                if (child)
                    ReadOnlyChildEntities.Add(entity);
                else
                    ReadOnlyRootEntities.Add(entity);
            }
            else
            {
                if (DynamicRootEntities.Count > 0 && DynamicRootEntities.Contains(entity))
                    return;
                if (EditableChildEntities.Count > 0 && EditableChildEntities.Contains(entity))
                    return;
                if (EditableRootEntities.Count > 0 && EditableRootEntities.Contains(entity))
                    return;
                if (SwitchableObjectEntities.Count > 0 && SwitchableObjectEntities.Contains(entity))
                    return;

                if (child)
                    EditableChildEntities.Add(entity);
                else
                    EditableRootEntities.Add(entity);
            }
        }

        private void CleanTemplateContextByValue(string value)
        {
            List<string> keys = (from key in ContextData.AllKeys
                                 let contextValues = ContextData.GetValues(key)
                                 where contextValues != null && contextValues.Length > 0 && contextValues[0] == value
                                 select key).ToList();

            foreach (string key in keys)
                ContextData.Remove(key);
        }

        private Dictionary<string, IEntity> GetChildEntities(IEnumerable<IEntity> list)
        {
            var entities = new Dictionary<string, IEntity>();
            foreach (var entity in list)
            {
                foreach (Association associationProperty in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
                {
                    foreach (AssociationProperty property in associationProperty.Properties)
                    {
                        if (!entities.ContainsKey(property.Property.Entity.EntityKeyName))
                        {
                            entities.Add(property.Property.Entity.EntityKeyName, property.Property.Entity);
                        }
                    }
                }

                foreach (Association associationProperty in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToZeroOrOne))
                {
                    foreach (AssociationProperty property in associationProperty.Properties)
                    {
                        if (!entities.ContainsKey(property.Property.Entity.EntityKeyName))
                        {
                            entities.Add(property.Property.Entity.EntityKeyName, property.Property.Entity);
                        }
                    }
                }
            }

            return entities;
        }

        private Dictionary<string, IEntity> GetListEntities(IEnumerable<IEntity> list)
        {
            var entities = new Dictionary<string, IEntity>();
            foreach (var entity in list)
            {
                foreach (Association associationProperty in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany || a.AssociationType == AssociationType.ManyToOne))
                {
                    foreach (AssociationProperty property in associationProperty.Properties)
                    {
                        if (!entities.ContainsKey(property.Property.Entity.EntityKeyName))
                        {
                            entities.Add(property.Property.Entity.EntityKeyName, property.Property.Entity);
                        }
                    }
                }
            }

            return entities;
        }

        private IEnumerable<IEntity> GetRootEntities(IEnumerable<IEntity> list)
        {
            var entities = new Dictionary<string, IEntity>();
            foreach (var entity in list)
            {
                if (entity.Associations.Count(a => a.AssociationType == AssociationType.ManyToOne) == 0)
                {
                    if (!entities.ContainsKey(entity.EntityKeyName))
                    {
                        entities.Add(entity.EntityKeyName, entity);
                    }
                }
            }

            return entities.Values;
        }

        private IEnumerable<IEntity> GetExcludedEntities(List<IEntity> list)
        {
            if(list == null)
                return new List<IEntity>();

            var excludedEntities = GetChildEntities(list);
            if (excludedEntities == null || excludedEntities.Count == 0)
                return list;

            return from entity in list
                   where !excludedEntities.ContainsKey(entity.EntityKeyName)
                   select entity;
        }
    }
}
