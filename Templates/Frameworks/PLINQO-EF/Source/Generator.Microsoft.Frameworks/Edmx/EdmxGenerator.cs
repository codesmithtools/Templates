//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

using CodeSmith.SchemaHelper;
using LinqToEdmx.Designer;

namespace Generator.Microsoft.Frameworks
{
    using LinqToEdmx;

    public partial class EdmxGenerator
    {
        private Edmx _edmx;
        private readonly GeneratorSettings _settings;
        private static bool _includeForeignKeysInModel = true;
        private const string PROPERTY_KEY = "{0}-{1}"; 

        #region Events

        public event EventHandler<SchemaItemProcessedEventArgs> SchemaItemProcessed;

        #endregion

        #region Constructor

        public EdmxGenerator(GeneratorSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region Methods

        public void Create(IEnumerable<IEntity> entities)
        {
            if (entities == null || entities.Count() <= 0) throw new ArgumentException("Entity Collection cannot be empty", "entities");

            //1. Load Database Object from and existing Edmx file or create a new Edmx Object.
            _edmx = File.Exists(_settings.MappingFile) ? Edmx.Load(_settings.MappingFile) : new Edmx();

            bool temp;
            if (bool.TryParse(GetDesignerProperty("IncludeForeignKeysInModel"), out temp))
                _includeForeignKeysInModel = temp;

            //<DesignerProperty Name="EnablePluralization" Value="False" />
            //<DesignerProperty Name="CodeGenerationStrategy" Value="None" />

            foreach (var entity in entities)
            {
                if (!entity.HasKey)
                {
                    var message = string.Format("warning 6013: The table/view '{0}' does not have a primary key defined and no valid primary key could be inferred. This table/view has been excluded. To use the entity, you will need to review your schema, add the correct keys, and regenerate it.", entity.EntityKeyName);
                    Debug.WriteLine(message);
                    Trace.WriteLine(message);
                }
                else if (entity is CommandEntity && IsValidFunction(entity as CommandEntity))
                {
                    var message = string.Format("warning 6005: The function '{0}' has a parameter that has a data type (E.G., 'sql_variant') which is not supported. The function was excluded.", entity.EntityKeyName);
                    Debug.WriteLine(message);
                    Trace.WriteLine(message);
                }
            }

            entities = entities.Where(e => e.HasKey || (e is CommandEntity && IsValidFunction(e as CommandEntity)));

            //2. Sync and create the mapping models.
            //   This also builds up a list of renamed column names that we need to keep track of.
            MergeMappingModel(entities);

            foreach (IEntity entity in entities)
            {
                if (entity is TableEnumEntity)
                {
                    Debug.WriteLine("Getting Enum Table: {0}", entity.EntityKeyName);
                    //GetEnum(entity as TableEnumEntity);
                }
                else if (entity is TableEntity)
                {
                    Debug.WriteLine("Getting Table Schema: {0}", entity.EntityKeyName);
                    GetEntity(entity as TableEntity);
                }
                else if (Configuration.Instance.IncludeViews && entity is ViewEntity)
                {
                    Debug.WriteLine("Getting View Schema: {0}", entity.EntityKeyName);
                    GetEntity(entity as ViewEntity);
                }
                else if (Configuration.Instance.IncludeFunctions && entity is CommandEntity)
                {
                    Debug.WriteLine("Getting Function Schema: {0}", entity.EntityKeyName);
                    GetFunctionEntity(entity as CommandEntity);
                }

                OnSchemaItemProcessed(entity.EntityKeyName);
            }

            foreach (IEntity entity in entities)
            {
                if (entity is TableEntity)
                {
                    CreateStorageAssociations(entity as TableEntity);
                    CreateConceptualAssociations(entity as TableEntity);
                }
            }

            // validate Edmx File
            Validate();
            UpdateDesignerProperites();

            _edmx.Save(_settings.MappingFile);
            //_edmx.Save(@"A:\CodeSmith.Experimental\CodeSmith.SchemaHelper\Source\POCO.Test\small2.edmx");
        }

        #endregion

        #region Validation

        private void Validate()
        {
            ValidateStorageModel();
            ValidateConceptualModel();
            ValidateMappingModel();
        }

        #endregion

        #region Tables and Views

        private void GetEntity(ISchemaEntity entity)
        {
            CreateStorageEntity(entity);
            CreateConceptualEntity(entity);
        }

        #endregion

        #region Functions

        // http://msdn.microsoft.com/en-us/library/dd283136.aspx

        private void GetFunctionEntity(CommandEntity entity)
        {
            CreateStorageFunctionEntity(entity);
            CreateConceptualFunctionEntity(entity);
        }


        #endregion

        #region Helpers

        //private static bool IsDeleteOnNull(TableEntity entity)
        //{
        //   return (entity.ExtendedProperties.ContainsKey("CS_IsDeleteOnNull") && entity.ExtendedProperties["CS_IsDeleteOnNull"] != null &&
        //           entity.ExtendedProperties["CS_IsDeleteOnNull"] is bool && (bool)entity.ExtendedProperties["CS_IsDeleteOnNull"]);
        //}

        private static bool IsCascadeDelete(Association association)
        {
           return (association.ExtendedProperties.ContainsKey("CS_CascadeDelete") && association.ExtendedProperties["CS_CascadeDelete"] != null &&
                   association.ExtendedProperties["CS_CascadeDelete"] is bool && (bool)association.ExtendedProperties["CS_CascadeDelete"]);
        }

        protected void OnSchemaItemProcessed(string name)
        {
            if (SchemaItemProcessed != null)
            {
                SchemaItemProcessed(this, new SchemaItemProcessedEventArgs(name));
            }
        }

        private static string GetMaxLength(ISchemaProperty property)
        {
            switch (property.DataType)
            {
                case DbType.Binary:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    {
                        if (property.NativeType.ToLower() == "timestamp")
                            return "8";
                        if (property.NativeType.ToLower() == "text" || property.NativeType.ToLower() == "ntext" || property.NativeType.ToLower() == "image" || property.NativeType.ToLower() == "xml")
                            return "Max";

                        if (property.Size > 0)
                            return property.Size.ToString();
                        if (property.Size == -1)
                            return "Max";

                        break;
                    }
            }

            return null;
        }

        private void UpdateDesignerProperites()
        {
            SetDesignerProperty(EdmxConstants.ContextNamespace, _settings.ContextNamespace);
            SetDesignerProperty(EdmxConstants.EntityNamespace, _settings.EntityNamespace);
        }

        private static bool IsValidFunction(CommandEntity entity)
        {
            if(entity.SearchCriteria != null && entity.SearchCriteria.Count > 0)
            {
                foreach (CommandParameter property in entity.SearchCriteria[0].Properties)
                {
                    if (property.Name.Equals("ReturnValue", StringComparison.InvariantCultureIgnoreCase)) continue;

                    if (ExcludeProperty(property))
                        return false;
                }
            }

            return true;
        }

        private static bool ExcludeProperty(ISchemaProperty property)
        {
            if (property == null) return true;

            if (property.NativeType.Equals("sql_variant", StringComparison.InvariantCultureIgnoreCase) ||
                property.NativeType.Equals("geography", StringComparison.InvariantCultureIgnoreCase) ||
                property.NativeType.Equals("geometry", StringComparison.InvariantCultureIgnoreCase) ||
                property.NativeType.Equals("hierarchyid", StringComparison.InvariantCultureIgnoreCase))
            {
                Trace.WriteLine(string.Format("Skipping Property '{0}.{1}' because the type '{2}' is not supported.", property.Entity.Name, property.Name, property.NativeType));
                return true;
            }

            // Exclude FK's.
            if(!_includeForeignKeysInModel && property.IsForeignKey)
                return true;

            return false;
        }


        private static bool ExcludeAssociation(Association association)
        {
            foreach (var property in association.Properties)
            {
                var invalid = association.IsParentEntity 
                    ? (property.Property.PropertyType & PropertyType.Key) != PropertyType.Key && (property.ForeignProperty.PropertyType & PropertyType.Foreign) != PropertyType.Foreign
                    : (property.ForeignProperty.PropertyType & PropertyType.Key) != PropertyType.Key && (property.Property.PropertyType & PropertyType.Foreign) != PropertyType.Foreign;
                
                if (invalid)
                    return true;
            }

            return false;
        }

        private static void ResolveAssociationValues(Association association, out IEntity principalEntity, out IEntity dependentEntity, out bool isParentEntity)
        {
            bool isManyToManyEntity = association.IsParentManyToMany();
            principalEntity = !isManyToManyEntity ? association.Entity : association.ForeignEntity;
            dependentEntity = !isManyToManyEntity ? association.ForeignEntity : association.Entity;

            isParentEntity = association.IsParentEntity;
            if (association.AssociationType == AssociationType.ManyToMany)
                isParentEntity &= association.IsParentManyToMany();
        }

        #endregion

        private Runtime RunTime
        {
            get
            {
                if (_edmx.Runtimes.Count == 0)
                {
                    if (string.IsNullOrEmpty(_edmx.Version) || (!_edmx.Version.Equals("1.0") && !_edmx.Version.Equals("2.0")))
                    {
                        _edmx.Version = "2.0";
                    }

                    _edmx.Runtimes.Add(new Runtime());
                }

                var runtime = _edmx.Runtimes.First();
                if (runtime.StorageModels == null) runtime.StorageModels = new StorageModels();
                if (runtime.ConceptualModels == null) runtime.ConceptualModels = new ConceptualModels();
                if (runtime.Mappings == null) runtime.Mappings = new Mappings();

                return runtime;
            }
        }
    }
}