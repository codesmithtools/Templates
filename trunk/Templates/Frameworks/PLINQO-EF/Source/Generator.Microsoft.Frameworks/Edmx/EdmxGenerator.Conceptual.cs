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
using System.Linq;
using CodeSmith.SchemaHelper;
using CodeSmith.SchemaHelper.Util;
using Generator.Microsoft.Frameworks.Utility;
using LinqToEdmx.Model.Conceptual;
using Association = CodeSmith.SchemaHelper.Association;
using Constraint = LinqToEdmx.Model.Conceptual.Constraint;

namespace Generator.Microsoft.Frameworks
{
    public partial class EdmxGenerator
    {
        private readonly List<string> _conceptualEntitys = new List<string>();
        private readonly List<string> _conceptualComplexTypes = new List<string>();
        private readonly List<string> _conceptualAssociations = new List<string>();
        private readonly List<string> _conceptualFunctions = new List<string>();

        private void CreateConceptualEntity(IEntity entity)
        {
            if (entity.IsParentManyToMany())
            {
                _conceptualEntitiesToRemove.Add(ResolveEntityMappedName(entity.EntityKey(), entity.Name));
                return;
            }

            // Check to see if this has already been processed.
            if (_conceptualEntitys.Contains(entity.Name))
                return;

            bool isNewView;
            string previousName;
            var entitySet = CreateConceptualEntitySet(entity, out previousName, out isNewView);

            bool isNewEntityType;
            var entityType = CreateConceptualEntityType(entity, entitySet.Name, previousName, ref isNewView, out isNewEntityType);
            
            RemoveDuplicateConceptualEntityTypeKeysAndProperties(entityType);
            
            // Remove extra properties values.
            var properties = from property in entityType.Properties
                             where !(from prop in entity.Properties select prop.KeyName).Contains(property.Name) &&
                             _removedStorageEntityProperties.Contains(string.Format(PROPERTY_KEY, entity.EntityKeyName, property.Name).ToLower()) // And it has been removed from the storage model.
                             select property;

            // Remove all of the key properties that don't exist in the table entity.
            foreach (var property in properties)
            {
                entityType.Properties.Remove(property);
            }
            
            CreateConceptualEntityTypeKeys(entity, isNewView, entityType);
            CreateConceptualEntityTypeProperties(entity, entityType, isNewEntityType);
            ValidateConceptualEntityComplexProperties(entity, entityType);

            _conceptualEntitys.Add(entity.Name);
        }

        private void CreateConceptualAssociations(TableEntity entity)
        {
            foreach (var association in entity.Associations)
            {
                IEntity principalEntity;
                IEntity dependentEntity;
                bool isParentEntity;
                string key;
                string toRole;
                string fromRole;
                ResolveConceptualAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);
                if (ExcludeAssociation(association) || !isParentEntity || principalEntity is TableEnumEntity || dependentEntity is TableEnumEntity || _conceptualAssociations.Contains(key))
                    continue;

                CreateConceptualAssociationSet(association);
                CreateConceptualAssociation(association);
            }

            //<NavigationProperty Name="Products" Relationship="PetShopModel1.FK__Product__Categor__0CBAE877" FromRole="Category" ToRole="Product" />
            var entityType = ConceptualSchema.EntityTypes.Where(e => ResolveEntityMappedName(entity.EntityKey(), entity.Name).Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (!entity.IsParentManyToMany() && entityType != null)
            {
                // Add new Associations.
                foreach (var association in entity.Associations)
                {
                    IEntity principalEntity;
                    IEntity dependentEntity;
                    bool isParentEntity;
                    string key;
                    string toRole;
                    string fromRole;
                    ResolveConceptualAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);
                    if (principalEntity is TableEnumEntity || dependentEntity is TableEnumEntity)
                        continue;

                    CreateConceptualNavigationProperty(entityType, association);
                }
            }
        }

        private void CreateConceptualFunctionEntity(CommandEntity entity)
        {
            // Check to see if this has already been processed.
            if (entity.IsFunction || _conceptualFunctions.Contains(ResolveEntityMappedName(entity.EntityKey(), entity.Name)) || !Configuration.Instance.IncludeFunctions) return;

            //<FunctionImport Name="GetCategoryById">
            //    <Parameter Name="ID" Mode="In" Type="String" />
            //</FunctionImport>
            
            //<FunctionImport Name="GetCategoryById" ReturnType="Collection(PetShopModel.GetCategoryById_Result1)">
            //   <Parameter Name="ID" Mode="In" Type="String" />
            //</FunctionImport>

            //  <FunctionImport Name="GetCategoryById" EntitySet="Categories" ReturnType="Collection(PetShopModel.Category)">
            //    <Parameter Name="ID" Mode="In" Type="String" />
            //  </FunctionImport>

            //  <FunctionImport Name="GetCategoryById" ReturnType="Collection(Int32)">
            //    <Parameter Name="ID" Mode="In" Type="String" />
            //  </FunctionImport>
            #region Validate the Function exists

            bool exists = true;
            var function = ConceptualSchemaEntityContainer.FunctionImports.Where(f =>
                entity.Name.Equals(f.Name, StringComparison.InvariantCultureIgnoreCase) ||
                entity.EntityKeyName.Equals(f.Name, StringComparison.InvariantCultureIgnoreCase) ||
                (!string.IsNullOrEmpty(f.EntitySet) && entity.EntityKeyName.Equals(f.EntitySet, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault();

            if (function == null)
            {
                exists = false;
                function = new EntityContainer.FunctionImportLocalType
                               {
                                   Name = ResolveEntityMappedName(entity.EntityKey(), entity.Name)
                               };

                ConceptualSchemaEntityContainer.FunctionImports.Add(function);
            }

            #endregion


            //http://msdn.microsoft.com/en-us/library/bb738614.aspx
            function.Name = ResolveEntityMappedName(entity.EntityKey(), entity.Name);
            var entitySetName = entity.IsAssociated
                                    ? ResolveEntityMappedName(entity.AssociatedEntity.EntityKey(), entity.AssociatedEntity.Name)
                                    : ResolveEntityMappedName(entity.EntityKey(), entity.Name);
            if(exists)
            {
                var type = function.ReturnType != null ? function.ReturnType.ToString().Replace("Collection(", string.Empty).Replace(")", string.Empty).Trim() : string.Empty;
                var complexTypeName = NamingConventions.PropertyName(type.Replace(ConceptualSchema.Namespace + ".", string.Empty));
                if (entity.IsStronglyTypedAssociatedEntity)
                {
                    function.EntitySet = entitySetName;
                    function.ReturnType = string.Format("Collection({0}.{1})", ConceptualSchema.Namespace, entitySetName);
                }
                else if (function.IsComplexType(ConceptualSchema.Namespace) && ConceptualSchema.ComplexTypes.Exists(complexTypeName) && entity.Properties.Count > 0)
                {
                    function.EntitySet = null;
                    function.ReturnType = string.Format("Collection({0}.{1})", ConceptualSchema.Namespace, complexTypeName);
                    CreateConceptualComplexType(entity, complexTypeName);
                }
                else if (!string.IsNullOrEmpty(type) && !SystemTypeMapper.EfConceptualTypeToSystemType.ContainsKey(type))
                {
                    function.ReturnType = null;
                    function.EntitySet = null;
                }
            }
            else if (entity.IsStronglyTypedAssociatedEntity)
            {
                function.EntitySet = entitySetName;
                function.ReturnType = string.Format("Collection({0}.{1})", ConceptualSchema.Namespace, entitySetName);
            }
            else if(entity.Properties.Count > 0)
            {
                function.ReturnType = null;
                //By default create a new ComplexType for a procedure's resultset if it contains more a column in the result set.
                function.ReturnType = string.Format("Collection({0}.{1}Result)", ConceptualSchema.Namespace, ResolveEntityMappedName(entity.EntityKey(), entity.Name));
                CreateConceptualComplexType(entity);
            }
            else
            {
                function.ReturnType = null;
                function.EntitySet = null;
            }

            //<Parameter Name="ApplicationName" Type="nvarchar" Mode="In" />
            //<Parameter Name="ApplicationId" Type="uniqueidentifier" Mode="InOut" />
            #region Process Parameters

            if (entity.SearchCriteria != null && entity.SearchCriteria.Count > 0)
            {
                #region Remove extra properties values.

                var properties = from property in function.Parameters
                                 where !(from prop in entity.SearchCriteria[0].Properties select prop.KeyName).Contains(property.Name) || property.Name.Equals("RETURN_VALUE", StringComparison.InvariantCultureIgnoreCase)
                                 select property;

                // Remove all of the key properties that don't exist in the table entity.
                foreach (var property in properties)
                {
                    function.Parameters.Remove(property);
                }

                #endregion

                foreach (CommandParameter property in entity.SearchCriteria[0].Properties)
                {
                    if (property.KeyName.Equals("RETURN_VALUE", StringComparison.InvariantCultureIgnoreCase)) continue;

                    var parameter = function.Parameters.Where(p => property.KeyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase) || property.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (parameter == null)
                    {
                        parameter = new FunctionImportParameter() { Name = property.KeyName };
                        function.Parameters.Add(parameter);
                    }

                    //http://msdn.microsoft.com/en-us/library/ee705451.aspx
                    parameter.Name = property.KeyName;
                    parameter.Mode = property.ParameterDirection == ParameterDirection.Input ? "In" : property.ParameterDirection == ParameterDirection.InputOutput ? "InOut" : "Out";
                    parameter.Type = GetSystemType(property);
                }
            }
            else
            {
                function.Parameters.Clear();
            }

            #endregion

            _conceptualFunctions.Add(ResolveEntityMappedName(entity.EntityKey(), entity.Name));
        }

        private void CreateConceptualComplexType(IEntity entity, string complexTypeName = "")
        {
            complexTypeName = !string.IsNullOrEmpty(complexTypeName)
                                  ? complexTypeName
                                  : entity is CommandEntity
                                        ? ResolveEntityMappedName(entity.EntityKey() + "complex", entity.Name)
                                        : ResolveEntityMappedName(entity.EntityKey(), entity.Name);

            //<ComplexType Name="GetCategoryById_Result">
            //  <Property Type="String" Name="CategoryId" Nullable="false" MaxLength="10" />
            //  <Property Type="String" Name="Name" Nullable="true" MaxLength="80" />
            //  <Property Type="String" Name="Descn" Nullable="true" MaxLength="255" />
            //</ComplexType>
            // Check to see if this has already been processed.
            if (_conceptualComplexTypes.Contains(entity.Name)) return;

            var type = ConceptualSchema.ComplexTypes.Where(c => c.Name.Equals(complexTypeName)).FirstOrDefault();
            if (type == null)
            {
                type = new ComplexType() { Name = complexTypeName };
                ConceptualSchema.ComplexTypes.Add(type);
            }

            type.Name = complexTypeName;
            type.SetAttributeValue(EdmxConstants.IsFunctionEntityCustomAttribute, entity is CommandEntity ? Boolean.TrueString : null);

            RemoveDuplicateConceptualComplexTypeProperties(entity, type, null);
            CreateConceptualComplexProperties(entity, type);

            _conceptualComplexTypes.Add(entity.Name);
        }

        private void ValidateConceptualModel()
        {
            ValidateConceptualAssociations();
            ValidateConceptualComplexTypes();
            ValidateConceptualEntities();
            ValidateConceptualAssociations();

            // Sort the entities.
            ConceptualSchemaEntityContainer.EntitySets = (from e in ConceptualSchemaEntityContainer.EntitySets orderby e.Name select e).ToList();
            ConceptualSchema.EntityTypes = (from e in ConceptualSchema.EntityTypes orderby e.Name select e).ToList();

            // Sort the associations.
            ConceptualSchemaEntityContainer.AssociationSets = (from a in ConceptualSchemaEntityContainer.AssociationSets  orderby a.Name select a).ToList();
            ConceptualSchema.Associations = (from a in ConceptualSchema.Associations orderby a.Name select a).ToList();
      
            ValidateConceptualFunctions();

            // Update the DataContextName.
            ConceptualSchemaEntityContainer.Name = _settings.DataContextName;
            MappingEntityContainer.CdmEntityContainer = _settings.DataContextName;
        }

        private void ValidateConceptualFunctions()
        {
            if (!Configuration.Instance.IncludeFunctions) return;

            var invalidFunctions = ConceptualSchemaEntityContainer.FunctionImports.Where(f => !_conceptualFunctions.Contains(f.Name)).ToList();
            foreach (var type in invalidFunctions)
            {
                ConceptualSchemaEntityContainer.FunctionImports.Remove(type);
            }

            ConceptualSchemaEntityContainer.FunctionImports = (from f in ConceptualSchemaEntityContainer.FunctionImports select f).ToList();
        }

        private void ValidateConceptualAssociations()
        {
            var processed = new List<string>();

            var associationSetsToRemove = new List<EntityContainer.AssociationSetLocalType>();
            foreach (var a in ConceptualSchemaEntityContainer.AssociationSets)
            {
               var isEndPointValid = a.Ends.Count(e => ConceptualSchemaEntityContainer.EntitySets.Count(es => es.EntityType.Equals(string.Concat(ConceptualSchema.Namespace, ".", e.EntitySet), StringComparison.InvariantCultureIgnoreCase)) > 0) == 2;
                if (processed.Contains(a.Name) || !isEndPointValid)
                    associationSetsToRemove.Add(a);
                else
                    processed.Add(a.Name);
            }

            foreach (var a in associationSetsToRemove)
            {
                ConceptualSchemaEntityContainer.AssociationSets.Remove(a);
            }

            processed.Clear();

            var associationsToRemove = new List<LinqToEdmx.Model.Conceptual.Association>();
            foreach (var a in ConceptualSchema.Associations)
            {
                var associationSetExists = ConceptualSchemaEntityContainer.AssociationSets.Count(e => ResolveAssociationMappedName(e.Name).Equals(a.Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
                if (processed.Contains(a.Name) || !associationSetExists)
                    associationsToRemove.Add(a);
                else
                    processed.Add(a.Name);
            }

            foreach (var a in associationsToRemove)
            {
                ConceptualSchema.Associations.Remove(a);
            }

            processed.Clear();
        }

        private void ValidateConceptualEntities()
        {
            var processed = new List<string>();
            var entitiesToRemove = new List<EntityType>();
            foreach (var entity in ConceptualSchema.EntityTypes)
            {
                RemoveDuplicateConceptualEntityTypeKeysAndProperties(entity);

                var isEntitySetDefined = ConceptualSchemaEntityContainer.EntitySets.Count(e =>
                    e.EntityType.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.Name), StringComparison.InvariantCultureIgnoreCase) ||
                    (!string.IsNullOrEmpty(entity.BaseType) && e.EntityType.Equals(entity.BaseType, StringComparison.InvariantCultureIgnoreCase))) > 0;

                if (_conceptualEntitiesToRemove.Contains(entity.Name) || processed.Contains(entity.Name) || !isEntitySetDefined || entity.Key == null || entity.Key.PropertyRefs.Count == 0)
                    entitiesToRemove.Add(entity);
                else
                    processed.Add(entity.Name);
            }

            foreach (var e in entitiesToRemove)
            {
                ConceptualSchema.EntityTypes.Remove(e);
            }

            processed.Clear();

            var entitySetsToRemove = new List<EntityContainer.EntitySetLocalType>();
            foreach (var e in ConceptualSchemaEntityContainer.EntitySets)
            {
                var isEntityTypeDefined = ConceptualSchema.EntityTypes.Count(et => et.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)) > 0; 
                //var isEntityDefinedInMappingLayer = MappingEntityContainer.EntitySetMappings.Count(es => es.EntityTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", ResolveEntityMappedName(e.Name)), StringComparison.InvariantCultureIgnoreCase)) > 0) > 0;
                if (processed.Contains(e.Name) || !isEntityTypeDefined)// || !isEntityDefinedInMappingLayer)
                    entitySetsToRemove.Add(e);
                else
                    processed.Add(e.Name);
            }

            foreach (var e in entitySetsToRemove)
            {
                ConceptualSchemaEntityContainer.EntitySets.Remove(e);
            }


            foreach (var entityType in ConceptualSchema.EntityTypes)
            {
                ValidateConceptualEntityAssociations(entityType);
            }

            //TODO: should we remove entities that are not defined in the mapping model?
        }

        private void ValidateConceptualComplexTypes()
        {
            var processed = new List<string>();

            var complexTypesToRemove = new List<ComplexType>();
            foreach (var entity in ConceptualSchema.ComplexTypes)
            {
                var hasProperties = entity.Properties.Count > 0;
                if (processed.Contains(entity.Name) || !hasProperties && entity.GetAttributeValue(EdmxConstants.IsFunctionEntityCustomAttribute) != null && !_conceptualComplexTypes.Contains(entity.Name))
                    complexTypesToRemove.Add(entity);
                else
                    processed.Add(entity.Name);
            }

            foreach (var e in complexTypesToRemove)
            {
                ConceptualSchema.ComplexTypes.Remove(e);
            }
        }

        private ConceptualSchema ConceptualSchema
        {
            get
            {
                if (RunTime.ConceptualModels.Untyped.IsEmpty || RunTime.ConceptualModels.ConceptualSchema == null)
                {
                    RunTime.ConceptualModels.ConceptualSchema = new ConceptualSchema
                    {

                        Namespace = _settings.ContextNamespace,
                        Alias = "Self"
                    };
                }

                return RunTime.ConceptualModels.ConceptualSchema;
            }
        }

        private EntityContainer ConceptualSchemaEntityContainer
        {
            get
            {
                if (ConceptualSchema.EntityContainers.Count == 0)
                {
                    ConceptualSchema.EntityContainers.Add(new EntityContainer
                    {
                        Name = _settings.DataContextName
                    });
                }

                return ConceptualSchema.EntityContainers.First();
            }
        }

        #region Helpers

        private EntityContainer.EntitySetLocalType CreateConceptualEntitySet(IEntity entity, out string previousName, out bool isNewView)
        {
            previousName = string.Empty;

            //<EntitySet Name="Categories" EntityType="PetShopModel1.Category" />
            var entitySet = ConceptualSchemaEntityContainer.EntitySets.Where(e =>
                                                                             e.EntityType.Equals(string.Concat(ConceptualSchema.Namespace, ".", ResolveEntityMappedName(entity.EntityKey(), entity.Name)), StringComparison.InvariantCultureIgnoreCase) ||
                                                                             e.EntityType.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.EntityKeyName), StringComparison.InvariantCultureIgnoreCase) ||
                                                                             entity.EntityKeyName.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                                                             ResolveEntityMappedName(entity.EntityKey(), entity.Name).Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            isNewView = entitySet == null && entity is ViewEntity;
            if (entitySet == null)
            {
                entitySet = new EntityContainer.EntitySetLocalType { Name = ResolveEntityMappedName(entity.EntityKey(), entity.Name) };
                ConceptualSchemaEntityContainer.EntitySets.Add(entitySet);
            }
            else
            {
                previousName = entitySet.Name;
            }

            // Set or sync the default values.
            entitySet.Name = ResolveEntityMappedName(entity.EntityKey(), entity.Name);
            entitySet.EntityType = string.Concat(ConceptualSchema.Namespace, ".", ResolveEntityMappedName(entity.EntityKey(), entity.Name));

            return entitySet;
        }

        private EntityType CreateConceptualEntityType(IEntity entity, string entitySetName, string previousName, ref bool isNewView, out bool isNewEntityType)
        {
            //<EntityType Name="Category">
            var entityType = ConceptualSchema.EntityTypes.Where(e =>
                                                                previousName.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                                                ResolveEntityMappedName(entity.EntityKey(), entity.Name).Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                                                entitySetName.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            isNewEntityType = false;
            if (entityType == null)
            {
                entityType = new EntityType()
                {
                    Name = ResolveEntityMappedName(entity.EntityKey(), entity.Name),
                    Key = new EntityKeyElement()
                };

                ConceptualSchema.EntityTypes.Add(entityType);

                isNewView = entity is ViewEntity;
                isNewEntityType = true;
            }

            entityType.Name = ResolveEntityMappedName(entity.EntityKey(), entity.Name);
            entityType.SetAttributeValue(EdmxConstants.IsViewEntityCustomAttribute, entity is ViewEntity ? Boolean.TrueString : null);
            
            return entityType;
        }

        /// <summary>
        /// Validates the properties on an Entity.
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveDuplicateConceptualEntityTypeKeysAndProperties(EntityType entity)
        {
            var processed = new List<string>();
            var propertiesToRemove = new List<EntityProperty>();
            foreach (var property in entity.Properties)
            {
                var isInvalidComplexType = property.IsComplexType(ConceptualSchema.Namespace) && !ConceptualSchema.ComplexTypes.Exists(property.Type.ToString(), ConceptualSchema.Namespace);
                if (isInvalidComplexType || processed.Contains(property.Name))
                    propertiesToRemove.Add(property);
                else
                    processed.Add(property.Name);
            }

            foreach (var e in propertiesToRemove)
            {
                entity.Properties.Remove(e);
            }

            entity.Properties = (from p in entity.Properties select p).ToList();

            if (entity.Key != null)
            {
                var keysProcessed = new List<string>();
                var keysToRemove = new List<PropertyRef>();
                foreach (PropertyRef property in entity.Key.PropertyRefs)
                {
                    if (!processed.Contains(property.Name) || keysProcessed.Contains(property.Name))
                        keysToRemove.Add(property);
                    else
                        keysProcessed.Add(property.Name);
                }

                foreach (var e in keysToRemove)
                {
                    entity.Key.PropertyRefs.Remove(e);
                }
            }
        }

        private void CreateConceptualEntityTypeKeys(IEntity entity, bool isNewView, EntityType entityType)
        {
            //<Key>
            //  <PropertyRef Name="CategoryId"  />
            //</Key>
            if (entity.HasKey || isNewView)
            {
                #region Remove extra key values.
                
                var items = from property in entityType.Key.PropertyRefs
                            where !(from prop in entity.Key.Properties select prop.Name).Contains(property.Name)
                            select property;

                // Remove all of the key properties that don't exist in the table entity.
                foreach (var property in items)
                {
                    entityType.Key.PropertyRefs.Remove(property);
                }
                
                #endregion
                
                foreach (var property in entity.Key.Properties.Where(p => entityType.Key.PropertyRefs.Count(pr => pr.Name == p.Name) == 0))
                {
                    entityType.Key.PropertyRefs.Add(new PropertyRef() { Name =  ResolveEntityPropertyMappedName(entity.Name, property.KeyName, property.Name) });
                }
            }
            else if (entity is TableEntity)
            {
                entityType.Key.PropertyRefs.Clear();
            }
        }
        
        private void CreateConceptualEntityTypeProperties(IEntity entity, EntityType entityType, bool isNewEntityType)
        {
            //<Property Name="CategoryId" Type="String" Nullable="false" MaxLength="10" Unicode="false" FixedLength="false" />
            foreach (ISchemaProperty property in entity.Properties)
            {
                var propertyName = ResolveEntityPropertyMappedName(entity.Name, property.KeyName, property.Name);
                var entityProperty = entityType.Properties.Where(p => propertyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase) || property.KeyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (entityProperty == null)
                {
                    if (ExcludeProperty(property) || !isNewEntityType && !_newStorageEntityProperties.Contains(string.Format("{0}-{1}", entity.Name, property.Name)))
                        continue;

                    entityProperty = new EntityProperty() { Name = ResolveEntityPropertyMappedName(entity.Name, property.KeyName, property.Name) };
                    entityType.Properties.Add(entityProperty);
                }
                else if (ExcludeProperty(property))
                {
                    entityType.Properties.Remove(entityProperty);
                    continue;
                }

                entityProperty.Name = propertyName;

                //Note: If the SystemType is prefixed with System than it will throw an error saying it cannot be resolved.
                entityProperty.Type = GetSystemType(property);

                if (!property.IsNullable)
                    entityProperty.Nullable = property.IsNullable;

                entityProperty.DefaultValue = string.IsNullOrEmpty(entityProperty.DefaultValue) && !string.IsNullOrEmpty(property.DefaultValue) ? property.DefaultValue : null;
                entityProperty.MaxLength = !string.IsNullOrEmpty(GetMaxLength(property)) ? GetMaxLength(property) : null;
                entityProperty.Precision = (property.DataType == DbType.Decimal || property.DataType == DbType.Currency && property.Precision > 0) ? property.Precision : (decimal?)null;
                entityProperty.Scale = (property.DataType == DbType.Decimal || property.DataType == DbType.Currency && property.Scale > 0) ? property.Scale : (decimal?)null;
                entityProperty.FixedLength = entityProperty.Type.Equals("String") || property.DataType == DbType.Binary ? property.FixedLength : (bool?)null;
                entityProperty.Unicode = entityProperty.Type.Equals("String") ? property.Unicode : (bool?)null;

                entityProperty.SetAttributeValue(EdmxConstants.IsIndexCustomAttribute, property.IsUnique ? Boolean.TrueString : null);

                if (property.IsIdentity || property.IsComputed)
                {
                    entityProperty.StoreGeneratedPattern = property.IsIdentity ? EdmxConstants.StoreGeneratedPatternIdentity : EdmxConstants.StoreGeneratedPatternComputed;
                    entityProperty.ConcurrencyMode = null;
                }
                else if (property.IsRowVersion)
                {
                    entityProperty.ConcurrencyMode = EdmxConstants.ConcurrencyModeFixed;
                    entityProperty.StoreGeneratedPattern = null;
                }
                else
                {
                    entityProperty.ConcurrencyMode = null;
                    entityProperty.StoreGeneratedPattern = null;
                }
            }
        }

        private void ValidateConceptualEntityComplexProperties(IEntity entity, EntityType entityType)
        {
            //<Property Name="ComplexProperty" Type="PetShopModel.Test" Nullable="false" />
            var invalidComplexTypeProperties = new List<EntityProperty>();
            foreach (var property in entityType.Properties.Where(p => p.IsComplexType(ConceptualSchema.Namespace)))
            {
                //<ComplexType Name="GetCategoryById_Result">
                var complexType = ConceptualSchema.ComplexTypes.Find(property.Type.ToString(), ConceptualSchema.Namespace);
                if (complexType == null)
                {
                    invalidComplexTypeProperties.Add(property);
                    continue;
                }

                RemoveDuplicateConceptualComplexTypeProperties(entity, complexType, entityType);

                if (!_conceptualComplexTypes.Contains(entity.Name))
                    _conceptualComplexTypes.Add(entity.Name);
            }

            foreach (var property in invalidComplexTypeProperties)
            {
                entityType.Properties.Remove(property);
            }
        }

        private void RemoveDuplicateConceptualComplexTypeProperties(IEntity entity, ComplexType type, EntityType entityType)
        {
            var processed = new List<string>();
            var propertiesToRemove = new List<ComplexTypeProperty>();
            foreach (var property in type.Properties)
            {
                if (processed.Contains(property.Name))
                    propertiesToRemove.Add(property);
                else
                    processed.Add(property.Name);
            }

            // Remove extra properties contained in the ComplexType that are not in the IEntity.
            propertiesToRemove.AddRange(from property in type.Properties
                                        where
                                            !(from prop in entity.Properties select prop.KeyName).Contains(property.Name) &&
                                            _removedStorageEntityProperties.Contains(string.Format(PROPERTY_KEY, entity.EntityKeyName, property.Name).ToLower()) && // And it has been removed from the storage model.
                                            !propertiesToRemove.Contains(property)
                                        select property);

            if (entityType != null)
            {
                // Remove extra properties contained in the ComplexType that are not in the IEntity.
                propertiesToRemove.AddRange(from property in type.Properties
                                            where !(from prop in entityType.Properties select entityType.Name).Contains(property.Name) && !propertiesToRemove.Contains(property)
                                            select property);
            }

            foreach (var e in propertiesToRemove)
            {
                type.Properties.Remove(e);
            }

            type.Properties = (from p in type.Properties select p).ToList();
        }

        private void CreateConceptualComplexProperties(IEntity entity, ComplexType type)
        {
            //<Property Type="String" Name="Descn" Nullable="true" MaxLength="255" />
            foreach (ISchemaProperty property in entity.Properties)
            {
                var propertyName = ResolveEntityPropertyMappedName(entity.Name, property.KeyName, property.Name);
                var entityProperty = type.Properties.Where(p => propertyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase) || property.KeyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (entityProperty == null)
                {
                    if (ExcludeProperty(property) && !_newStorageEntityProperties.Contains(string.Format("{0}-{1}", entity.Name, property.Name)))
                        continue;

                    entityProperty = new ComplexTypeProperty() { Name = ResolveEntityPropertyMappedName(entity.Name, property.KeyName, property.Name) };
                    type.Properties.Add(entityProperty);
                }
                else if (ExcludeProperty(property))
                {
                    type.Properties.Remove(entityProperty);
                    continue;
                }

                entityProperty.Name = propertyName;

                //Note: If the SystemType is prefixed with System than it will throw an error saying it cannot be resolved.
                entityProperty.Type = GetSystemType(property);

                if (!property.IsNullable)
                    entityProperty.Nullable = property.IsNullable;

                entityProperty.MaxLength = !string.IsNullOrEmpty(GetMaxLength(property)) ? GetMaxLength(property) : null;
            }
        }

        private void CreateConceptualAssociationSet(Association association)
        {
            IEntity principalEntity;
            IEntity dependentEntity;
            bool isParentEntity;
            string key;
            string toRole;
            string fromRole;
            ResolveConceptualAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);

            //<AssociationSet Name="FK__Product__Categor__0CBAE877" Association="PetShopModel1.FK__Product__Categor__0CBAE877">
            //  <End Role="Category" EntitySet="Categories" />
            //  <End Role="Product" EntitySet="Products" />
            //</AssociationSet
            var associationSet = ConceptualSchemaEntityContainer.AssociationSets.Where(e => e.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (associationSet == null)
            {
                associationSet = new EntityContainer.AssociationSetLocalType
                {
                    Name = key,
                    Ends = new List<EntityContainer.AssociationSetLocalType.EndLocalType>()
                };

                ConceptualSchemaEntityContainer.AssociationSets.Add(associationSet);
            }
            else
            {
                _mappingAssociationNames[key] = associationSet.Association.Replace(string.Concat(ConceptualSchema.Namespace, "."), string.Empty);

                // Remove the AssociationEnd's that don't exist.
                var items = associationSet.Ends.Where(e => !e.Role.Equals(toRole) && !e.Role.Equals(principalEntity.Name) && !e.Role.Equals(fromRole) && !e.Role.Equals(dependentEntity.Name));
                foreach (var associationEnd in items)
                {
                    associationSet.Ends.Remove(associationEnd);
                }
            }

            // Set or sync the default values.
            associationSet.Name = key;
            associationSet.Association = string.Concat(ConceptualSchema.Namespace, ".", key);

            var principalEnd = CreateConceptualAssociationSetEnd(principalEntity, toRole, associationSet);
            var dependentEnd = CreateConceptualAssociationSetEnd(dependentEntity, fromRole, associationSet, principalEnd);

            // Update the Ends (forces the ends to be grouped in the edmx file).
            associationSet.Ends = (from a in associationSet.Ends orderby a.Role select a).ToList();
        }

        private EntityContainer.AssociationSetLocalType.EndLocalType CreateConceptualAssociationSetEnd(IEntity entity, string role, EntityContainer.AssociationSetLocalType set, EntityContainer.AssociationSetLocalType.EndLocalType otherEnd = null)
        {
            var end = otherEnd == null ?
                set.Ends.Where(e => e.Role.Equals(entity.EntityKeyName) || e.Role.Equals(role)).FirstOrDefault() :
                set.Ends.Where(e => !e.Equals(otherEnd)).FirstOrDefault();

            if (end == null)
            {
                end = new EntityContainer.AssociationSetLocalType.EndLocalType() { Role = role };
                set.Ends.Add(end);
            }

            end.Role = role;
            end.EntitySet = ResolveEntityMappedName(entity.EntityKey(), entity.Name);

            return end;
        }

        private void CreateConceptualAssociation(Association association)
        {
            IEntity principalEntity;
            IEntity dependentEntity;
            bool isParentEntity;
            string key;
            string toRole;
            string fromRole;
            ResolveConceptualAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);
            
            if (_conceptualAssociations.Contains(key))
                return;
            //<Association Name="FK__Product__Categor__0CBAE877">
            //  <End Role="Category" Type="PetShopModel1.Category" Multiplicity="1" />
            //  <End Role="Product" Type="PetShopModel1.Product" Multiplicity="*" />
            //  <ReferentialConstraint>
            //      <Principal Role="Category">
            //          <PropertyRef Name="CategoryId" />
            //      </Principal>
            //      <Dependent Role="Product">
            //          <PropertyRef Name="CategoryId" />
            //      </Dependent>
            //  </ReferentialConstraint>
            //</Association>
            var assoc = ConceptualSchema.Associations.Where(a => a.Name.Equals(key)).FirstOrDefault();
            if (assoc != null) ConceptualSchema.Associations.Remove(assoc);
            assoc = new LinqToEdmx.Model.Conceptual.Association()
                            {
                                Name = key,
                                Ends = new List<AssociationEnd>()
                            };

            ConceptualSchema.Associations.Add(assoc);

            var principalEnd = CreateConceptualAssociationEnd(principalEntity, toRole, assoc, false);
            var dependentEnd = CreateConceptualAssociationEnd(dependentEntity, fromRole, assoc, false);

            UpdateConceptualAssociationEndMultiplicity(association, principalEnd, dependentEnd);

            #region ReferentialConstraint

            if (!association.IsParentManyToMany())
            {
                assoc.ReferentialConstraint = new Constraint
                {
                    Principal = new ReferentialConstraintRoleElement() { Role = toRole },
                    Dependent = new ReferentialConstraintRoleElement() { Role = fromRole }
                };

                CreateStorageAssociationReferentialConstraintProperties(assoc, association.Properties, principalEntity, dependentEntity, false);
            }

            #endregion

            _conceptualAssociations.Add(key);
        }

        private AssociationEnd CreateConceptualAssociationEnd(IEntity entity, string role, LinqToEdmx.Model.Conceptual.Association assocication, bool isCascadeDelete)
        {
            //  <End Role="Category" Type="PetShopModel1.Category" Multiplicity="1">
            //     <OnDelete Action="Cascade" /> 
            //  </End> 
            //  <End Role="Product" Type="PetShopModel1.Product" Multiplicity="*" />
            var end = new AssociationEnd()
            {
                Role = role,
                Type = string.Concat(ConceptualSchema.Namespace, ".", ResolveEntityMappedName(entity.EntityKey(), entity.Name))
            };

            if (isCascadeDelete)
            {
                end.OnDelete.Add(new OnAction() { Action = EdmxConstants.OnDeleteActionCascade });
            }

            assocication.Ends.Add(end);

            return end;
        }

        private static void UpdateConceptualAssociationEndMultiplicity(Association association, AssociationEnd principalEnd, AssociationEnd dependentEnd)
        {
            switch (association.AssociationType)
            {
                case AssociationType.OneToMany:
                    principalEnd.Multiplicity = MultiplicityConstants.One;
                    dependentEnd.Multiplicity = MultiplicityConstants.Many;
                    break;
                case AssociationType.OneToOne:
                    principalEnd.Multiplicity = MultiplicityConstants.One;
                    dependentEnd.Multiplicity = MultiplicityConstants.One;
                    break;
                case AssociationType.OneToZeroOrOne:
                    principalEnd.Multiplicity = MultiplicityConstants.One;
                    dependentEnd.Multiplicity = MultiplicityConstants.ZeroToOne;
                    break;
                case AssociationType.ZeroOrOneToMany:
                case AssociationType.ManyToZeroOrOne:
                    principalEnd.Multiplicity = MultiplicityConstants.ZeroToOne;
                    dependentEnd.Multiplicity = MultiplicityConstants.Many;
                    break;
                case AssociationType.ManyToMany:
                    principalEnd.Multiplicity = MultiplicityConstants.Many;
                    dependentEnd.Multiplicity = MultiplicityConstants.Many;
                    break;
                default:
                    principalEnd.Multiplicity = MultiplicityConstants.One;
                    dependentEnd.Multiplicity = MultiplicityConstants.Many;
                    break;
            }
        }

        private void CreateStorageAssociationReferentialConstraintProperties(LinqToEdmx.Model.Conceptual.Association association, IEnumerable<AssociationProperty> properties, IEntity principalEntity, IEntity dependentEntity, bool isParentEntity)
        {
            //  <ReferentialConstraint>
            //      <Principal Role="Category">
            //          <PropertyRef Name="CategoryId" />
            //      </Principal>
            //      <Dependent Role="Product">
            //          <PropertyRef Name="CategoryId" />
            //      </Dependent>
            //  </ReferentialConstraint>
            foreach (var property in properties)
            {
                var principalProp = !isParentEntity ? property.Property : property.ForeignProperty;
                var dependentProp = !isParentEntity ? property.ForeignProperty : property.Property;

                var principalProperty = association.ReferentialConstraint.Principal.PropertyRefs.Where(p => p.Name == principalProp.Name).FirstOrDefault();
                if (principalProperty == null)
                {
                    principalProperty = new PropertyRef()
                    {
                        Name = ResolveEntityPropertyMappedName(principalEntity.Name, principalProp.KeyName, principalProp.Name)
                    };
                    association.ReferentialConstraint.Principal.PropertyRefs.Add(principalProperty);
                }

                var dependentProperty = association.ReferentialConstraint.Dependent.PropertyRefs.Where(p => p.Name == dependentProp.Name).FirstOrDefault();
                if (dependentProperty == null)
                {
                    dependentProperty = new PropertyRef()
                    {
                        Name = ResolveEntityPropertyMappedName(dependentEntity.Name, dependentProp.KeyName, dependentProp.Name)
                    };
                    association.ReferentialConstraint.Dependent.PropertyRefs.Add(dependentProperty);
                }
            }
        }

        private void CreateConceptualNavigationProperty(EntityType entity, Association association)
        {
            IEntity principalEntity;
            IEntity dependentEntity;
            bool isParentEntity;
            string key;
            string toRole;
            string fromRole;
            ResolveConceptualAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);

            //11/4/2011 If an entity has an association to itself then we will modify the key if it's the child association.
            bool isSelfReferencingChild = !isParentEntity && principalEntity == dependentEntity;
            if (!isSelfReferencingChild && !association.IsParentManyToMany())
            {
                var temp = fromRole;
                fromRole = toRole;
                toRole = temp;
            }

            // 11/4/2011 Updated to check to see if a self referencing entity exists then we check the too and from roles. By checking only the to and from roles if it's a self referencing this allows custom names to be picked up.
            var navigationProperty = principalEntity == dependentEntity 
                ? entity.NavigationProperties.FirstOrDefault(n => n.Relationship.Equals(String.Concat(ConceptualSchema.Namespace, ".", key)) && n.ToRole.Equals(toRole) && n.FromRole.Equals(fromRole))
                : entity.NavigationProperties.FirstOrDefault(n => n.Relationship.Equals(String.Concat(ConceptualSchema.Namespace, ".", key)));

            if (navigationProperty == null)
            {
                navigationProperty = new NavigationProperty() { Name = association.Name };
                entity.NavigationProperties.Add(navigationProperty);
            }

            if (string.IsNullOrEmpty(navigationProperty.Name) || association.Name.StartsWith(navigationProperty.Name))
                navigationProperty.Name = association.Name;

            navigationProperty.Relationship = string.Concat(ConceptualSchema.Namespace, ".", key);
            navigationProperty.FromRole = fromRole;
            navigationProperty.ToRole = toRole;
        }

        /// <summary>
        /// Validates an EntityType's Associations.
        /// </summary>
        /// <param name="entity"></param>
        private void ValidateConceptualEntityAssociations(EntityType entity)
        {
            var proccessed = new List<string>();
            var navigationsToRemove = new List<NavigationProperty>();
            foreach (var nav in entity.NavigationProperties)
            {
                //from navigationProperty in type.NavigationProperties
                //                       where !(from a in entity.Associations select string.Concat(ConceptualSchema.Namespace, ".", ResolveAssociationMappedName(a.AssociationKeyName))).Contains(navigationProperty.Relationship)
                //                       select navigationProperty;

                if (!proccessed.Contains(nav.Name) && ValidateConceptualNavigationProperty(nav))
                    proccessed.Add(nav.Name);
                else
                    navigationsToRemove.Add(nav);
            }

            foreach (var nav in navigationsToRemove)
            {
                entity.NavigationProperties.Remove(nav);
            }

            entity.NavigationProperties = (from n in entity.NavigationProperties select n).ToList();
        }

        private bool ValidateConceptualNavigationProperty(NavigationProperty nav)
        {
            var associationSet = ConceptualSchemaEntityContainer.AssociationSets.Where(a => a.Association == nav.Relationship).FirstOrDefault();
            if (associationSet == null) 
                return false;

            var association = ConceptualSchema.Associations.Where(a => a.Name == associationSet.Association.Replace(string.Concat(ConceptualSchema.Namespace, "."), "")).FirstOrDefault();
            if (association == null) 
                return false;

            var fromRoleEnd = association.Ends.Where(e => e.Role.Equals(nav.FromRole, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            var toRoleEnd = association.Ends.Where(e => e.Role.Equals(nav.ToRole, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (fromRoleEnd == null || toRoleEnd == null)
                return false;

            var isValidFromRoleEntity = ConceptualSchema.EntityTypes.Count(es => fromRoleEnd.Type.Replace(string.Concat(ConceptualSchema.Namespace, "."), "").Equals(es.Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
            var isValidToRoleEntity = ConceptualSchema.EntityTypes.Count(es => toRoleEnd.Type.Replace(string.Concat(ConceptualSchema.Namespace, "."), "").Equals(es.Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
            if (!isValidFromRoleEntity || !isValidToRoleEntity)
                return false;

            if (nav.FromRole.Equals(fromRoleEnd.Role, StringComparison.InvariantCultureIgnoreCase) && nav.ToRole.Equals(toRoleEnd.Role, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        private void ResolveConceptualAssociationValues(Association association, out IEntity principalEntity, out IEntity dependentEntity, out bool isParentEntity, out string keyName, out string toRole, out string fromRole)
        {
            bool isManyToManyEntity = association.IsParentManyToMany();
            principalEntity = !isManyToManyEntity ? association.Entity : association.ForeignEntity;
            dependentEntity = !isManyToManyEntity ? (association.AssociationType == AssociationType.ManyToMany) ? association.IntermediaryAssociation.Entity : association.ForeignEntity : association.IntermediaryAssociation.ForeignEntity;

            toRole = ResolveEntityMappedName(principalEntity.EntityKey(), principalEntity.Name);
            fromRole = ResolveEntityMappedName(dependentEntity.EntityKey(), dependentEntity.Name);
            if (toRole.Equals(fromRole)) fromRole += 1;

            keyName = ResolveAssociationMappedName(isManyToManyEntity ? association.Entity.EntityKeyName : association.AssociationKeyName);

            isParentEntity = association.IsParentEntity;
            if (association.AssociationType == AssociationType.ManyToMany)
                isParentEntity &= association.IsParentManyToMany();
        }

        private static string GetSystemType(ISchemaProperty property)
        {
            if (property.BaseSystemType.Equals("System.Byte[]"))
                return "Binary";

            if (property.BaseSystemType.Equals("System.Xml.XmlDocument"))
                return "String";

            if (property.BaseSystemType.Equals("System.TimeSpan"))
                return "Time";

            return property.BaseSystemType.Replace("System.", "");
        }

        #endregion
    }
}
