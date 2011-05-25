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
using System.Linq;

using CodeSmith.SchemaHelper;
using LinqToEdmx.Designer;
using LinqToEdmx.Map;
using ComplexProperty = LinqToEdmx.Map.ComplexProperty;

namespace Generator.Microsoft.Frameworks
{
    using System.Diagnostics;

    public partial class EdmxGenerator
    {
        private readonly List<string> _mappingEntitys = new List<string>();

        private const string MappingCategory = "Mapping Model";
        private readonly Dictionary<string, string> _mappingEntityNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _mappingAssociationNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _mappingEntityPropertyNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _mappingDroppedEntityPropertyNames = new Dictionary<string, string>();

        /// <summary>
        /// 2.0: Sync and create the mapping models.
        /// </summary>
        /// <param name="entities">All of the entities needing to be merged.</param>
        private void MergeMappingModel(IEnumerable<IEntity> entities)
        {
            // 1. Update all of the Mapping Entities so we can guarantee and verify that our Mapping Associations End's exist.
            foreach (IEntity entity in entities)
            {
                if ((!entity.IsParentManyToMany() && entity is TableEntity) || (Configuration.Instance.IncludeViews && entity is ViewEntity))
                    CreateMappingEntity(entity);
            }

            // 2. Create Designer.
            CreateDesigner();

            // 3. Update the Mapping Associations and check the end points.
            foreach (IEntity entity in entities)
            {
                if (Configuration.Instance.IncludeFunctions && entity is CommandEntity)
                    CreateFunctionMappingEntity(entity as CommandEntity);

                if (entity.IsParentManyToMany() && entity is TableEntity)
                    CreateMappingAssociations(entity as TableEntity);
            }
        }

        /// <summary>
        /// 2.1: Update all of the Mapping Entities so we can guarantee and verify that our Mapping Associations End's exist.
        /// 
        /// <EntitySetMapping Name="Account">
        ///  <EntityTypeMapping TypeName="PetShopModel.Account">
        ///   <MappingFragment StoreEntitySet="Account">
        ///     <ScalarProperty Name="AccountId" ColumnName="AccountId" />
        ///   </MappingFragment>
        ///  </EntityTypeMapping>
        /// </EntitySetMapping>
        /// 
        /// </summary>
        /// <param name="entity">The Entity.</param>
        private void CreateMappingEntity(IEntity entity)
        {
            if (_mappingEntitys.Contains(entity.Name))
            {
                Debug.WriteLine(string.Format("Already Processed Mapping Model Entity {0}", entity.Name), MappingCategory);
                return;
            }

            //<EntitySetMapping Name="Categories">
            #region Validate that an EntitySet Exists in the MappingStorageContainer.

            var entitySet = MappingEntityContainer.EntitySetMappings.Where(e => 
                entity.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) || // Safe Name.
                entity.EntityKeyName.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) || // Database Name.
                (e.EntityTypeMappings.Count > 0 &&
                    e.EntityTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.Name), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.EntityTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.EntityKeyName), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.EntityTypeMappings.Count(et => et.TypeName.Equals(string.Format("IsTypeOf({0}.{1})", ConceptualSchema.Namespace, entity.Name), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.EntityTypeMappings.Count(et => et.TypeName.Equals(string.Format("IsTypeOf({0}.{1})", ConceptualSchema.Namespace, entity.EntityKeyName), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.EntityTypeMappings.Count(et => et.MappingFragments.Count > 0 && et.MappingFragments.Count(mf => mf.StoreEntitySet.Equals(entity.EntityKeyName, StringComparison.InvariantCultureIgnoreCase)) > 0) > 0)
                    ).FirstOrDefault();

            //NOTE: We could also possibly look up the table name by looking at the StorageModel's EntitySet Tables Property.

            // If the Entity Set does not exist than create a new one.
            if (entitySet == null)
            {
                entitySet = new EntitySetMapping() { Name = entity.Name };
                MappingEntityContainer.EntitySetMappings.Add(entitySet);
            }

            #endregion

            //<EntityTypeMapping TypeName="PetShopModel1.Category">
            //<EntityTypeMapping TypeName="PetShopModel.CategoryChanged">
            #region Validate the EntityType Mapping

            string entityName = entity.Name;
            var mapping = entitySet.EntityTypeMappings.FirstOrDefault();
            if (mapping == null)
            {
                mapping = new EntityTypeMapping() { TypeName = string.Concat(ConceptualSchema.Namespace, ".", entity.Name) };
                entitySet.EntityTypeMappings.Add(mapping);
            }
            else if (!string.IsNullOrEmpty(mapping.TypeName))
            {
                entityName = mapping.TypeName.Replace("IsTypeOf(", "").Replace(string.Format("{0}.", ConceptualSchema.Namespace), "").Replace(")", "");
                entityName = entityName.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)
                                 ? entity.Name
                                 : entityName;
            }

            entitySet.Name = entityName;

            // Check for inheritance.
            mapping.TypeName = mapping.TypeName != null && mapping.TypeName.StartsWith("IsTypeOf") ?
                string.Format("IsTypeOf({0}.{1})", ConceptualSchema.Namespace, entityName) :
                string.Concat(ConceptualSchema.Namespace, ".", entityName);

            _mappingEntityNames.Add(entity.EntityKey(), entityName);

            // <MappingFragment StoreEntitySet="Category">
            //  <ScalarProperty Name="CategoryId" ColumnName="CategoryId" />
            //</MappingFragment>
            var mappingFragment = mapping.MappingFragments.Where(m => m.StoreEntitySet.Equals(entity.Name) || m.StoreEntitySet.Equals(entity.EntityKeyName)).FirstOrDefault();
            if (mappingFragment == null)
            {
                mappingFragment = new MappingFragment() { StoreEntitySet = entity.Name };
                mapping.MappingFragments.Add(mappingFragment);
            }

            mappingFragment.StoreEntitySet = entity.EntityKeyName;

            //<ScalarProperty Name="LineNum" ColumnName="LineNum" />
            MergeScalarProperties(mappingFragment, entity);

            #endregion

            _mappingEntitys.Add(entity.Name);
        }

        /// <summary>
        /// 2.3: Update the Mapping Associations and check the end points.
        /// 
        ///<AssociationSetMapping Name="aspnet_UsersInRoles" TypeName="PetShopModel.aspnet_UsersInRoles" StoreEntitySet="aspnet_UsersInRoles">
        ///  <EndProperty Name="aspnet_Roles">
        ///    <ScalarProperty Name="RoleId" ColumnName="RoleId" />
        ///  </EndProperty>
        ///  <EndProperty Name="aspnet_Users">
        ///    <ScalarProperty Name="UserId" ColumnName="UserId" />
        ///  </EndProperty>
        ///</AssociationSetMapping>
        /// 
        /// </summary>
        /// <param name="entity">The Entity</param>
        private void CreateMappingAssociations(TableEntity entity)
        {
            // <AssociationSetMapping Name="aspnet_UsersInRoles" TypeName="PetShopModel.aspnet_UsersInRoles" StoreEntitySet="aspnet_UsersInRoles">
            #region Validate that an AssociationSetMapping Exists in the MappingEntityContainer.

            foreach (var association in entity.Associations)
            {
                IEntity principalEntity;
                IEntity dependentEntity;
                bool isParentEntity;
                string key;
                string toRole;
                string fromRole;
                ResolveConceptualAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);
                if (!(association.AssociationType == AssociationType.ManyToMany && association.IntermediaryAssociation != null) || principalEntity is TableEnumEntity || dependentEntity is TableEnumEntity)
                    continue;

                var typeName = association.Entity.EntityKeyName;
                var associationSetMapping = MappingEntityContainer.AssociationSetMappings.Where(e => e.Name.Equals(association.Entity.EntityKeyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (associationSetMapping == null)
                {
                    associationSetMapping = new AssociationSetMapping
                    {
                        Name = association.Entity.EntityKeyName,
                        EndProperties = new List<EndProperty>()
                    };

                    MappingEntityContainer.AssociationSetMappings.Add(associationSetMapping);
                }
                else if (!string.IsNullOrEmpty(associationSetMapping.TypeName))
                {
                    typeName = associationSetMapping.TypeName.Replace(string.Format("{0}.", ConceptualSchema.Namespace), "");
                    typeName = typeName.Equals(association.Entity.EntityKeyName, StringComparison.InvariantCultureIgnoreCase)
                                     ? association.Entity.EntityKeyName : typeName;
                }

                // Set or sync the default values.
                associationSetMapping.Name = association.Entity.EntityKeyName;
                associationSetMapping.TypeName = string.Concat(ConceptualSchema.Namespace, ".", typeName);
                associationSetMapping.StoreEntitySet = association.Entity.EntityKeyName;

                _mappingAssociationNames[association.AssociationKeyName] = typeName;
                _mappingAssociationNames[association.IntermediaryAssociation.AssociationKeyName] = typeName;

                var properties = new List<EndProperty>();

                //<EndProperty Name="aspnet_Roles">
                //  <ScalarProperty Name="RoleId" ColumnName="RoleId" />
                //</EndProperty>
                var principalEnd = associationSetMapping.EndProperties.Where(e => e.Name.Equals(principalEntity.EntityKeyName) || e.Name.Equals(ResolveEntityMappedName(principalEntity.EntityKey(), principalEntity.Name))).FirstOrDefault() ??
                    new EndProperty() { Name = ResolveEntityMappedName(principalEntity.EntityKey(), principalEntity.Name) };

                principalEnd.Name = ResolveEntityMappedName(principalEntity.EntityKey(), principalEntity.Name);
                MergeScalarProperties(principalEnd, association);
                properties.Add(principalEnd);

                var dependentEnd = associationSetMapping.EndProperties.Where(e => !e.Name.Equals(principalEnd.Name)).FirstOrDefault() ??
                    new EndProperty() { Name = ResolveEntityMappedName(dependentEntity.EntityKey(), dependentEntity.Name) };

                dependentEnd.Name = ResolveEntityMappedName(dependentEntity.EntityKey(), dependentEntity.Name);
                if (dependentEnd.Name.Equals(principalEnd.Name)) dependentEnd.Name += 1;

                MergeScalarProperties(dependentEnd, association.IntermediaryAssociation);
                properties.Add(dependentEnd);

                associationSetMapping.EndProperties = properties;
            }

            #endregion
        }

        private void ValidateMappingModel()
        {
            ValidateMappingEntities();
            ValidateMappingAssociations();
            ValidateMappingFunctions();

            MappingEntityContainer.EntitySetMappings = (from s in MappingEntityContainer.EntitySetMappings orderby s.Name select s).ToList();
            MappingEntityContainer.AssociationSetMappings = (from a in MappingEntityContainer.AssociationSetMappings orderby a.Name select a).ToList();
        }

        private void ValidateMappingEntities()
        {
            var processed = new List<string>();
            var entitySetMappingsToRemove = new List<EntitySetMapping>();
            foreach (var m in MappingEntityContainer.EntitySetMappings)
            {
                var entityTypeMappingsToRemove = new List<EntityTypeMapping>();
                foreach (var entityTypeMapping in m.EntityTypeMappings)
                {
                    var name = entityTypeMapping.TypeName.Replace("IsTypeOf(", "").Replace(")", "");
                    var isConceptualEntityDefined = ConceptualSchemaEntityContainer.EntitySets.Count(es => es.EntityType.Equals(name, StringComparison.InvariantCultureIgnoreCase)) > 0;
                    var isStorageEntityDefined = entityTypeMapping.MappingFragments.Count > 0 && StorageSchemaEntityContainer.EntitySets.Count(es => es.Name.Equals(entityTypeMapping.MappingFragments[0].StoreEntitySet, StringComparison.InvariantCultureIgnoreCase)) > 0;
                    var hasFunctionMapping = entityTypeMapping.ModificationFunctionMapping != null;
                    if (!hasFunctionMapping && (processed.Contains(name) || !isConceptualEntityDefined || !isStorageEntityDefined))
                        entityTypeMappingsToRemove.Add(entityTypeMapping);
                    else
                        processed.Add(name);
                }

                foreach (var e in entityTypeMappingsToRemove)
                {
                    m.EntityTypeMappings.Remove(e);
                }

                if (m.EntityTypeMappings.Count == 0)
                    entitySetMappingsToRemove.Add(m);

                //TODO: We should also validate the Mapping Fragments.
            }

            foreach (var e in entitySetMappingsToRemove)
            {
                MappingEntityContainer.EntitySetMappings.Remove(e);
            }
        }

        private void ValidateMappingAssociations()
        {
            //  <AssociationSetMapping Name="FK_Account_Profiles" TypeName="PetShopModel.FK_Account_Profiles" StoreEntitySet="Account">
            //  <EndProperty Name="Account">
            //    <ScalarProperty Name="AccountId" ColumnName="AccountId" />
            //  </EndProperty>
            //  <EndProperty Name="Profiles">
            //    <ScalarProperty Name="UniqueID" ColumnName="UniqueID" />
            //  </EndProperty>
            //</AssociationSetMapping>

            var processed = new List<string>();
            var associationSetMappingsToRemove = new List<AssociationSetMapping>();
            foreach (var a in MappingEntityContainer.AssociationSetMappings)
            {
                var isValidConceptualAssociation = ConceptualSchemaEntityContainer.AssociationSets.Count(es => a.TypeName.Equals(es.Association, StringComparison.InvariantCultureIgnoreCase)) > 0;
                var isValidStorageAssociation = StorageSchemaEntityContainer.EntitySets.Count(es => a.StoreEntitySet.Equals(es.Name, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(es.Table)) > 0;
                if (processed.Contains(a.Name) || (!isValidConceptualAssociation || !_conceptualAssociations.Contains(a.Name) || !isValidStorageAssociation))
                    associationSetMappingsToRemove.Add(a);
                else
                    processed.Add(a.Name);
            }

            foreach (var a in associationSetMappingsToRemove)
            {
                MappingEntityContainer.AssociationSetMappings.Remove(a);
            }
        }

        private void ValidateMappingFunctions()
        {
            if (!Configuration.Instance.IncludeFunctions) return;

            var processed = new List<string>();
            var functionImportMappingsToRemove = new List<FunctionImportMapping>();
            foreach (var m in MappingEntityContainer.FunctionImportMappings)
            {
                if (!ConceptualSchemaEntityContainer.FunctionImports.Exists(m.FunctionImportName) || !StorageSchema.Functions.Exists(m.FunctionName, StorageSchema.Namespace))
                {
                    functionImportMappingsToRemove.Add(m);
                    continue;
                }

                // If the mapping exists then we need to check to make sure that the Conceptual ComplexType is still valid.
                if (m.ResultMapping == null) continue;

                var complexTypeMappingsToRemove = new List<FunctionImportComplexTypeMapping>();
                foreach (var etm in m.ResultMapping.ComplexTypeMappings)
                {
                    if (processed.Contains(etm.TypeName) || !ConceptualSchema.ComplexTypes.Exists(etm.TypeName, ConceptualSchema.Namespace))
                        complexTypeMappingsToRemove.Add(etm);
                    else
                        processed.Add(etm.TypeName);
                }

                foreach (var e in complexTypeMappingsToRemove)
                {
                    m.ResultMapping.ComplexTypeMappings.Remove(e);
                }

                if((m.ResultMapping.EntityTypeMappings == null ||  m.ResultMapping.EntityTypeMappings.Count == 0) &&
                    (m.ResultMapping.ComplexTypeMappings == null ||  m.ResultMapping.ComplexTypeMappings.Count == 0))
                {
                    m.ResultMapping = null;
                    functionImportMappingsToRemove.Add(m);
                }
            }

            foreach (var e in functionImportMappingsToRemove)
            {
                MappingEntityContainer.FunctionImportMappings.Remove(e);
            }

            processed.Clear();
        }

        private void CreateFunctionMappingEntity(CommandEntity entity)
        {
            if (entity.IsFunction || _mappingEntitys.Contains(entity.Name) || !Configuration.Instance.IncludeFunctions)
            {
                Debug.WriteLine(string.Format("Already Processed Mapping Model Entity {0}", entity.Name), MappingCategory);
                return;
            }

            // <FunctionImportMapping FunctionImportName="GetCategoryById" FunctionName="PetShopModel.Store.GetCategoryById" >
            #region Validate that an EntitySet Exists in the MappingStorageContainer.

            var importMapping = MappingEntityContainer.FunctionImportMappings.Where(e =>
                entity.Name.Equals(e.FunctionImportName, StringComparison.InvariantCultureIgnoreCase) || // Safe Name.
                entity.EntityKeyName.Equals(e.FunctionImportName, StringComparison.InvariantCultureIgnoreCase) || // Database Name.
                (e.ResultMapping != null && e.ResultMapping.ComplexTypeMappings.Count > 0 &&
                    (e.ResultMapping.ComplexTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.Name), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.ResultMapping.ComplexTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.Name, "Result"), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.ResultMapping.ComplexTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.EntityKeyName), StringComparison.InvariantCultureIgnoreCase)) > 0 ||
                    e.ResultMapping.ComplexTypeMappings.Count(et => et.TypeName.Equals(string.Concat(ConceptualSchema.Namespace, ".", entity.EntityKeyName, "Result"), StringComparison.InvariantCultureIgnoreCase)) > 0))).FirstOrDefault();

            //NOTE: We could also possibly look up the table name by looking at the StorageModel's EntitySet Tables Property.

            // If the Entity Set does not exist than create a new one.
            if (importMapping == null)
            {
                importMapping = new FunctionImportMapping
                                    {
                                        FunctionImportName = entity.Name,
                                        ResultMapping = new FunctionImportMappingResultMapping()
                                    };

                MappingEntityContainer.FunctionImportMappings.Add(importMapping);
            }

            importMapping.FunctionName = string.Concat(StorageSchema.Namespace, ".", entity.EntityKeyName);

            if (string.IsNullOrEmpty(importMapping.FunctionImportName) || !ConceptualSchemaEntityContainer.FunctionImports.Exists(importMapping.FunctionImportName))
                importMapping.FunctionImportName = entity.Name;

            #endregion

            if (entity.Properties.Count > 0)
                CreateFunctionMappingComplexTypeMapping(entity, importMapping);
            else if(importMapping.ResultMapping != null && importMapping.ResultMapping.ComplexTypeMappings != null)
                importMapping.ResultMapping.ComplexTypeMappings.Clear();

            _mappingEntitys.Add(entity.Name);
        }

        private void CreateFunctionMappingComplexTypeMapping(CommandEntity entity, FunctionImportMapping importMapping)
        {
            //<ResultMapping>
            //  <ComplexTypeMapping TypeName="PetShopModel.GetCategoryById_Result">
            string entityName = string.Concat(entity.Name, "Result");
            var mapping = importMapping.ResultMapping != null && importMapping.ResultMapping.ComplexTypeMappings != null
                              ? importMapping.ResultMapping.ComplexTypeMappings.FirstOrDefault()
                              : null;
            
            if (mapping == null)
            {
                importMapping.ResultMapping = new FunctionImportMappingResultMapping()
                                                  {
                                                      ComplexTypeMappings = new List<FunctionImportComplexTypeMapping>()
                                                  };

                mapping = new FunctionImportComplexTypeMapping() { TypeName = string.Concat(ConceptualSchema.Namespace, ".", entityName) };
                importMapping.ResultMapping.ComplexTypeMappings.Add(mapping);
            }
            else if (!string.IsNullOrEmpty(mapping.TypeName))
            {
                entityName = mapping.TypeName.Replace("IsTypeOf(", "").Replace(string.Format("{0}.", ConceptualSchema.Namespace), "").Replace(")", "");
                entityName = entityName.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase) ? entity.Name : entityName;
            }

            if(ConceptualSchema.ComplexTypes.Count(c => c.Name.Equals(entityName, StringComparison.InvariantCultureIgnoreCase)) == 0)
                entityName = string.Concat(entity.Name, "Result");

            // Check for inheritance.
            mapping.TypeName = string.Format("{0}.{1}", ConceptualSchema.Namespace, entityName);

            _mappingEntityNames.Add(entity.EntityKey(), importMapping.FunctionImportName);
            _mappingEntityNames.Add(entity.EntityKey() + "complex", entityName);

            //<ComplexTypeMapping TypeName="PetShopModel.GetCategoryById_Result">
            //  <ScalarProperty Name="CategoryId" ColumnName="CategoryId" />
            //  <ScalarProperty Name="Name" ColumnName="Name" />
            //  <ScalarProperty Name="Description" ColumnName="Descn" />
            //</ComplexTypeMapping>
            MergeScalarProperties(mapping, entity);
        }

        #region Helpers

        /// <summary>
        /// Merges and creates a list of all of the Properties. <ScalarProperty Name="LineNum" ColumnName="LineNum" />
        /// </summary>
        /// <param name="mappingFragment">The MappingFragment.</param>
        /// <param name="entity">The Entity.</param>
        private void MergeScalarProperties(MappingFragment mappingFragment, IEntity entity)
        {
            foreach (var property in mappingFragment.ScalarProperties.Where(p => entity.Properties.Count(prop => prop.KeyName.Equals(p.ColumnName, StringComparison.InvariantCultureIgnoreCase)) == 0))
                _mappingDroppedEntityPropertyNames[string.Format(PROPERTY_KEY, entity.EntityKeyName, property.ColumnName)] = property.Name;
        
            var properties = new List<ScalarProperty>();
            foreach (var property in entity.Properties)
            {
                var prop = mappingFragment.ScalarProperties.Where(p => p.ColumnName.Equals(property.KeyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                
                ScalarProperty complexProperty = null;
                if(mappingFragment.ComplexProperties != null && mappingFragment.ComplexProperties.Count > 0)
                {
                    foreach (ComplexProperty complexProp in mappingFragment.ComplexProperties)
                    {
                        complexProperty = complexProp.ScalarProperties.Where(p => p.ColumnName.Equals(property.KeyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (complexProperty != null) break;
                    }
                }

                if (prop == null)
                {
                    // The property doesn't exist so lets create it.
                    prop = new ScalarProperty() { Name = property.Name };
                }
                else if (!property.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase)) // Column matches that in the database.. If the names are different, it wins.
                {
                    // The propertyName has been updated.
                    // TODO: Is there a better way to find out if they renamed the Property?
                    prop.Name = prop.Name;
                }
                else
                {
                    // Update the propertyName so it is always current with SchemaHelper.
                    prop.Name = property.Name;
                }

                // Forces complex properties to win.
                if (complexProperty != null)
                {
                    complexProperty.ColumnName = property.KeyName;
                    _mappingEntityPropertyNames[string.Format("{0}-{1}", entity.Name, property.KeyName)] = complexProperty.Name;
                }
                else
                {
                    prop.ColumnName = property.KeyName;
                    _mappingEntityPropertyNames[string.Format("{0}-{1}", entity.Name, property.KeyName)] = prop.Name;

                    if (!ExcludeProperty(property as ISchemaProperty))
                        properties.Add(prop);
                }
            }

            mappingFragment.ScalarProperties = properties.Distinct().ToList();
        }

        private void MergeScalarProperties(FunctionImportComplexTypeMapping mappingFragment, CommandEntity entity)
        {
            foreach (var property in mappingFragment.ScalarProperties.Where(p => entity.Properties.Count(prop => prop.KeyName.Equals(p.ColumnName, StringComparison.InvariantCultureIgnoreCase)) == 0))
                _mappingDroppedEntityPropertyNames[string.Format(PROPERTY_KEY, entity.EntityKeyName, property.ColumnName)] = property.Name;

            var properties = new List<ScalarProperty>();
            foreach (var property in entity.Properties)
            {
                var prop = mappingFragment.ScalarProperties.Where(p => p.ColumnName.Equals(property.KeyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (prop == null)
                {
                    // The property doesn't exist so lets create it.
                    prop = new ScalarProperty() { Name = property.Name };
                }
                else if (!property.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase)) // Column matches that in the database.. If the names are different, it wins.
                {
                    // The propertyName has been updated.
                    // TODO: Is there a better way to find out if they renamed the Property?
                    //prop.Name = prop.Name;
                }
                else
                {
                    // Update the propertyName so it is always current with SchemaHelper.
                    prop.Name = property.Name;
                }

                prop.ColumnName = property.KeyName;
                if (!ExcludeProperty(property as ISchemaProperty) && properties.Count(p => p.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase)) == 0)
                {
                    properties.Add(prop);
                    _mappingEntityPropertyNames[string.Format("{0}-{1}", entity.Name, property.KeyName)] = prop.Name;
                }
            }

            mappingFragment.ScalarProperties = properties.Distinct().ToList();
        }

        /// <summary>
        /// Merges and creates a list of all of the Properties. <ScalarProperty Name="LineNum" ColumnName="LineNum" />
        /// </summary>
        /// <param name="endProperty">The EndProperty.</param>
        /// <param name="association">The association.</param>
        private void MergeScalarProperties(EndProperty endProperty, Association association)
        {
            var properties = new List<ScalarProperty>();

            foreach (var property in association.Properties)
            {
                var associationProperty = !association.Entity.Name.Equals(property.Property.Entity.Name) ? property.Property : property.ForeignProperty;
                var columnProperty = association.Entity.Name.Equals(property.Property.Entity.Name) ? property.Property : property.ForeignProperty;

                var prop = endProperty.ScalarProperties.Where(p => p.ColumnName.Equals(columnProperty.KeyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (prop == null)
                {
                    // The property doesn't exist so lets create it.
                    prop = new ScalarProperty() { Name = associationProperty.Name };
                }
                else if (!associationProperty.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase)) // Column matches that in the database.. If the names are different, it wins.
                {
                    // The propertyName has been updated.
                    // TODO: Is there a better way to find out if they renamed the Property?
                    //prop.Name = CodeSmith.SchemaHelper.Util.NamingConventions.PropertyName(prop.Name, false);
                }
                else
                {
                    // Update the propertyName so it is always current with SchemaHelper.
                    prop.Name = associationProperty.Name;
                }

                prop.ColumnName = columnProperty.KeyName;
                _mappingEntityPropertyNames[string.Format("{0}-{1}", associationProperty.Entity.Name, associationProperty.KeyName)] = prop.Name;

                properties.Add(prop);
            }

            endProperty.ScalarProperties = properties.Distinct().ToList();
        }

        private string ResolveAssociationMappedName(string associationKeyName)
        {
            if (_mappingAssociationNames.ContainsKey(associationKeyName))
                return _mappingAssociationNames[associationKeyName];

            return associationKeyName;
        }

        private string ResolveEntityMappedName(string key, string entityName)
        {
            if (_mappingEntityNames.ContainsKey(key))
                return _mappingEntityNames[key];

            return entityName;
        }

        private string ResolveEntityPropertyMappedName(string entityName, string keyName, string propertyName)
        {
            string key = string.Format("{0}-{1}", entityName, keyName);

            if (_mappingEntityPropertyNames.ContainsKey(key))
                return _mappingEntityPropertyNames[key];

            return propertyName;
        }

        private string ResolveConceptualPropertyNameFromStorageColumnName(string storageName, string columnName)
        {
            var key = string.Format(PROPERTY_KEY, storageName, columnName);
            return _mappingDroppedEntityPropertyNames.ContainsKey(key) ? _mappingDroppedEntityPropertyNames[key] : columnName;
        }

        private string ResolveConceptualNameFromStorageName(string storageName)
        {
            foreach (var mapping in MappingEntityContainer.EntitySetMappings)
            {
                if (mapping.EntityTypeMappings.Any(entityTypeMapping => entityTypeMapping.MappingFragments.Any(mappingFragment => mappingFragment.StoreEntitySet.Equals(storageName, StringComparison.InvariantCultureIgnoreCase))))
                {
                    return mapping.Name;
                }
            }

            return storageName;
        }

        #endregion

        private Mapping Mapping
        {
            get
            {
                if (RunTime.Mappings.Untyped.IsEmpty || RunTime.Mappings.Mapping == null)
                {
                    RunTime.Mappings = new Mappings()
                    {
                        Mapping = new Mapping()
                        {
                            Space = "C-S"
                        }
                    };
                }

                return RunTime.Mappings.Mapping;
            }
        }

        //<EntityContainerMapping StorageEntityContainer="PetShopModel1StoreContainer" CdmEntityContainer="PetShopEntities1">
        private EntityContainerMapping MappingEntityContainer
        {
            get
            {
                if (Mapping.EntityContainerMapping == null)
                {

                    Mapping.EntityContainerMapping = new EntityContainerMapping()
                    {
                        CdmEntityContainer = ConceptualSchemaEntityContainer.Name,
                        StorageEntityContainer = StorageSchemaEntityContainer.Name
                    };
                }

                return Mapping.EntityContainerMapping;
            }
        }
    }
}
