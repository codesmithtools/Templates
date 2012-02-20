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
using System.Data;
using System.Linq;

using CodeSmith.SchemaHelper;
using LinqToEdmx.Model.Storage;
using Association = CodeSmith.SchemaHelper.Association;

namespace Generator.Microsoft.Frameworks
{
    public partial class EdmxGenerator
    {
        #region Private Members
        
        private readonly List<string> _newStorageEntityProperties = new List<string>();
        private readonly List<string> _removedStorageEntityProperties = new List<string>();

        private readonly List<string> _storageEntitys = new List<string>();
        private readonly Dictionary<string, string> _storageEntityNames = new Dictionary<string, string>();
        private readonly List<string> _storageAssociations = new List<string>();
        private readonly List<string> _storageFunctions = new List<string>();
        private readonly List<string> _conceptualEntitiesToRemove = new List<string>();

        #endregion

        private void CreateStorageEntity(ISchemaEntity entity)
        {
            // Check to see if this has already been processed.
            if (_storageEntitys.Contains(entity.EntityKey())) return;

            bool isNewView;
            
            var entitySet = CreateStorageEntitySet(entity, out isNewView);
            var entityType = CreateStorageEntityType(entity, entitySet.Name, ref isNewView);
            
            // Remove the duplicate properties.
            RemoveDuplicateStorageEntityTypeKeysAndProperties(entityType);

            // Remove extra properties values.
            var properties = from property in entityType.Properties
                             where !(from prop in entity.Properties select prop.KeyName).Contains(property.Name)
                             select property;

            // Remove all of the key properties that don't exist in the table entity.
            foreach (var property in properties)
            {
                var propertyName = ResolveConceptualPropertyNameFromStorageColumnName(entityType.Name, property.Name);
                _removedStorageEntityProperties.Add(String.Format(PROPERTY_KEY, entity.EntityKeyName, propertyName).ToLower());
                entityType.Properties.Remove(property);
            }

            CreateStorageEntityTypeKeys(entity, isNewView, entityType);
            CreateStorageEntityTypeProperties(entity, entityType);

            _storageEntitys.Add(entity.EntityKeyName);
        }

        private void CreateStorageAssociations(TableEntity entity)
        {
            foreach (var association in entity.Associations)
            {
                IEntity principalEntity;
                IEntity dependentEntity;
                bool isParentEntity;
                ResolveAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity);
                if (ExcludeAssociation(association) || !association.IsParentEntity || principalEntity is TableEnumEntity || dependentEntity is TableEnumEntity || _storageAssociations.Contains(association.AssociationKeyName))
                    continue;

                CreateStorageAssociationSet(association);
                CreateStorageAssociation(association);

                if (association.IsParentManyToMany())
                {
                    CreateStorageAssociationSet(association.IntermediaryAssociation);
                    CreateStorageAssociation(association.IntermediaryAssociation);
                }
            }
        }

        private void CreateStorageFunctionEntity(CommandEntity entity)
        {
            // Check to see if this has already been processed.
            if (_storageFunctions.Contains(entity.EntityKeyName) || !Configuration.Instance.IncludeFunctions) return;

            //<Function Name="aspnet_Applications_CreateApplication" Aggregate="false" BuiltIn="false" NiladicFunction="false" IsComposable="false" ParameterTypeSemantics="AllowImplicitConversion" Schema="dbo">
            #region Validate the Function exists

            var function = StorageSchema.Functions.Where(f =>
                (entity.EntityKeyName.Equals(f.Name, StringComparison.InvariantCultureIgnoreCase) && entity.Owner.Equals(f.Schema, StringComparison.InvariantCultureIgnoreCase)) ||
                (entity.EntityKeyName.Equals(f.Name1, StringComparison.InvariantCultureIgnoreCase) && entity.Owner.Equals(f.Schema1, StringComparison.InvariantCultureIgnoreCase)) ||
                 entity.EntityKeyName.Equals(f.Name1, StringComparison.InvariantCultureIgnoreCase) || entity.EntityKeyName.Equals(f.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            
            if (function == null)
            {
                function = new Function()
                {
                    Name = entity.EntityKeyName
                };

                StorageSchema.Functions.Add(function);
            }

            //http://msdn.microsoft.com/en-us/library/bb738614.aspx
            function.Name = entity.EntityKeyName;
            function.Name1 = entity.EntityKeyName;
            function.Aggregate = false; //TODO: True if the stored procedure returns an aggregate value; otherwise False.
            function.BuiltIn = false; //TODO: True if the function is a built-in1 function; otherwise False. (A built-in function is a function that is defined in the database. For information about functions that are defined in the storage model)
            function.NiladicFunction = false; //TODO:  A niladic function is a function that accepts no parameters and, when called, does not require parentheses.
            function.IsComposable = entity.IsFunction; 
            function.ParameterTypeSemantics = "AllowImplicitConversion"; //TODO: Determine if this stored procedure is an AllowImplicitConversion.
            function.Schema = entity.Owner;
            function.Schema1 = entity.Owner;

            // Return type cannot be set if the function is composible.
            if (entity.IsFunction)
                function.ReturnType = entity.ReturnValueParameter != null ? entity.ReturnValueParameter.NativeType : "int";

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

                    var parameter = function.Parameters.Where(p => property.KeyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (parameter == null)
                    {
                        parameter = new Parameter() { Name = property.KeyName };
                        function.Parameters.Add(parameter);
                    }

                    //http://msdn.microsoft.com/en-us/library/ee705451.aspx
                    parameter.Name = property.KeyName;
                    parameter.Mode = property.ParameterDirection == ParameterDirection.Input ? "In" : property.ParameterDirection == ParameterDirection.InputOutput ? "InOut" : "Out";
                    parameter.Type = property.NativeType;
                }
            }
            else
            {
                function.Parameters.Clear();
            }

            #endregion

            #endregion

            _storageFunctions.Add(entity.EntityKeyName);
        }

        private void ValidateStorageModel()
        {
            ValidateStorageEntities();
            ValidateStorageAssociations();

            // Sort the entities.
            StorageSchemaEntityContainer.EntitySets = (from s in StorageSchemaEntityContainer.EntitySets orderby s.Name select s).ToList();
            StorageSchema.EntityTypeStores = (from s in StorageSchema.EntityTypeStores orderby s.Name select s).ToList();
            
            // Sort the associations.
            StorageSchemaEntityContainer.AssociationSets = (from a in StorageSchemaEntityContainer.AssociationSets orderby a.Name select a).ToList();
            StorageSchema.Associations = (from a in StorageSchema.Associations orderby a.Name select a).ToList();

            ValidateStorageFunctions();
       }

        private void ValidateStorageFunctions()
        {
            if (!Configuration.Instance.IncludeFunctions) return;

            var invalidFunctions = StorageSchema.Functions.Where(f => !_storageFunctions.Contains(f.Name)).ToList();
            foreach (var type in invalidFunctions)
            {
                StorageSchema.Functions.Remove(type);
            }

            StorageSchema.Functions = (from f in StorageSchema.Functions orderby f.Name select f).ToList();
        }

        private void ValidateStorageEntities()
        {
            var processed = new List<string>();

            var entitiesToRemove = new List<EntityTypeStore>();
            foreach (var entity in StorageSchema.EntityTypeStores)
            {
                RemoveDuplicateStorageEntityTypeKeysAndProperties(entity);

                var isEntitySetDefined = StorageSchemaEntityContainer.EntitySets.Count(e => entity.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
                if (processed.Contains(entity.Name) || !isEntitySetDefined || !_storageEntitys.Contains(entity.Name) || entity.Key == null || entity.Key.PropertyRefs.Count == 0)
                {
                    if (_storageEntitys.Contains(entity.Name))
                        _storageEntitys.Remove(entity.Name);

                    _conceptualEntitiesToRemove.Add(ResolveConceptualNameFromStorageName(entity.Name));
                    entitiesToRemove.Add(entity);
                }
                else
                    processed.Add(entity.Name);
            }

            foreach (var e in entitiesToRemove)
            {
                StorageSchema.EntityTypeStores.Remove(e);
            }

            processed.Clear();

            var entitySetsToRemove = new List<EntityContainer.EntitySetLocalType>();
            foreach (var e in StorageSchemaEntityContainer.EntitySets)
            {
                var isTableOrView = _storageEntitys.Contains(e.Name) && !string.IsNullOrEmpty(e.Type);
                var isEntityDefined = string.IsNullOrEmpty(e.Type) && StorageSchema.EntityTypeStores.Count(et => et.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
                if (processed.Contains(e.Name) || (!isTableOrView && !isEntityDefined))
                    entitySetsToRemove.Add(e);
                else
                    processed.Add(e.Name);
            }

            foreach (var e in entitySetsToRemove)
            {
                StorageSchemaEntityContainer.EntitySets.Remove(e);
            }

            //TODO: should we remove entities that are not defined in the mapping model?
        }

        private void ValidateStorageAssociations()
        {
            var processed = new List<string>();

            var associationSetsToRemove = new List<EntityContainer.AssociationSetLocalType>();
            foreach (var a in StorageSchemaEntityContainer.AssociationSets)
            {
                var isEndPointValid = a.Ends.Count(e => StorageSchemaEntityContainer.EntitySets.Count(es => es.Name.Equals(e.EntitySet, StringComparison.InvariantCultureIgnoreCase)) > 0) == 2;
                if (processed.Contains(a.Name) || !isEndPointValid || !_storageAssociations.Contains(a.Name))
                    associationSetsToRemove.Add(a);
                else
                    processed.Add(a.Name);
            }

            foreach (var a in associationSetsToRemove)
            {
                StorageSchemaEntityContainer.AssociationSets.Remove(a);
            }

            processed.Clear();

            var associationsToRemove = new List<LinqToEdmx.Model.Storage.Association>();
            foreach (var a in StorageSchema.Associations)
            {
                var associationSetExists = StorageSchemaEntityContainer.AssociationSets.Count(e => e.Name.Equals(a.Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
                if (processed.Contains(a.Name) || !associationSetExists || !_storageAssociations.Contains(a.Name))
                    associationsToRemove.Add(a);
                else
                    processed.Add(a.Name);
            }

            foreach (var a in associationsToRemove)
            {
                StorageSchema.Associations.Remove(a);
            }

            processed.Clear();
        }

        private StorageSchema StorageSchema
        {
            get
            {
                if (RunTime.StorageModels.Untyped.IsEmpty || RunTime.StorageModels.StorageSchema == null)
                {
                    RunTime.StorageModels.StorageSchema = new StorageSchema
                    {
                        Namespace = string.Concat(_settings.DatabaseName, "Model.Store"),
                        Alias = "Self",
                        Provider = "System.Data.SqlClient",
                        ProviderManifestToken = "2008"
                    };
                }

                return RunTime.StorageModels.StorageSchema;
            }
        }

        private EntityContainer StorageSchemaEntityContainer
        {
            get
            {
                if (StorageSchema.EntityContainers.Count == 0)
                {
                    StorageSchema.EntityContainers.Add(new EntityContainer()
                    {
                        Name = string.Concat(_settings.DatabaseName, "ModelStoreContainer")
                    });
                }

                return StorageSchema.EntityContainers.First();
            }
        }

        #region Helpers

        private EntityContainer.EntitySetLocalType CreateStorageEntitySet(ISchemaEntity entity, out bool isNewView)
        {
            //<EntitySet Name="Category" EntityType="PetShopModel1.Store.Category" store:Type="Tables" Schema="dbo" />
            //<EntitySet Name="vw_aspnet_Applications" EntityType="PetShopModel1.Store.vw_aspnet_Applications" store:Type="Views" store:Schema="dbo" store:Name="vw_aspnet_Applications">
            var entitySet = StorageSchemaEntityContainer.EntitySets.Where(e =>
                                                                          (entity.EntityKeyName.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) && entity.SchemaName.Equals(e.Schema, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                          (entity.EntityKeyName.Equals(e.Name1, StringComparison.InvariantCultureIgnoreCase) && entity.SchemaName.Equals(e.Schema1, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                          entity.EntityKeyName.Equals(e.Name1, StringComparison.InvariantCultureIgnoreCase) || entity.EntityKeyName.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            isNewView = entitySet == null && entity is ViewEntity;
            if (entitySet == null)
            {
                entitySet = new EntityContainer.EntitySetLocalType { Name = entity.EntityKeyName };
                StorageSchemaEntityContainer.EntitySets.Add(entitySet);
            }

            _storageEntityNames[entity.EntityKeyName] = entitySet.Name;

            // Set or sync the default values.
            // http://msdn.microsoft.com/en-us/library/bb387152.aspx
            entitySet.Name = entity.EntityKeyName;
            entitySet.EntityType = string.Concat(StorageSchema.Namespace, ".", entity.EntityKeyName);
            entitySet.Type = entity is TableEntity ? EdmxConstants.StorageSchemaGenerationTypeAttributeValueTables : null;
            entitySet.Schema = entity is TableEntity ? entity.SchemaName : null;

            if (entity is ViewEntity)
            {
                entitySet.Type = EdmxConstants.StorageSchemaGenerationTypeAttributeValueViews;
                entitySet.DefiningQuery = ((ViewEntity)entity).SourceText.Remove(0, ((ViewEntity)entity).SourceText.IndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase));
            }
            else
                entitySet.Table = entity.EntityKeyName;

            if (!string.IsNullOrEmpty(entitySet.Schema1)) entitySet.Schema1 = entity.SchemaName;
            if (!string.IsNullOrEmpty(entitySet.Name1)) entitySet.Name1 = entity.EntityKeyName;

            return entitySet;
        }

        private EntityTypeStore CreateStorageEntityType(ISchemaEntity entity, string name, ref bool isNewView)
        {
            EntityTypeStore entityType = StorageSchema.EntityTypeStores.Where(e => ResolveStorageEntityName(entity.EntityKeyName).Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) || name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (entityType == null)
            {
                entityType = new EntityTypeStore()
                {
                    Name = name,
                    Key = new EntityKeyElement()
                };

                StorageSchema.EntityTypeStores.Add(entityType);

                isNewView = entity is ViewEntity;
            }

            // Sync the name.
            entityType.Name = name;

            return entityType;
        }

        private static void RemoveDuplicateStorageEntityTypeKeysAndProperties(EntityTypeStore entity)
        {
            var processed = new List<string>();
            var propertiesToRemove = new List<EntityProperty>();
            foreach (var property in entity.Properties)
            {
                if (processed.Contains(property.Name))
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
                foreach (var property in entity.Key.PropertyRefs)
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

        private static void CreateStorageEntityTypeKeys(ISchemaEntity entity, bool isNewView, EntityTypeStore entityType)
        {
            //<Key>
            //  <PropertyRef Name="CategoryId"  />
            //</Key>
            if (entity.HasKey || isNewView)
            {
                #region Remove extra key values.
               
                var items = from property in entityType.Key.PropertyRefs
                            where !(from prop in entity.Key.Properties select prop.KeyName).Contains(property.Name)
                            select property;

                // Remove all of the key properties that don't exist in the table entity.
                foreach (var property in items)
                {
                    entityType.Key.PropertyRefs.Remove(property);
                }

                #endregion

                foreach (var property in entity.Key.Properties.Where(p => entityType.Key.PropertyRefs.Count(pr => pr.Name == p.Name) == 0))
                {
                    entityType.Key.PropertyRefs.Add(new PropertyRef() { Name = property.KeyName });
                }
            }
            else if (entity is TableEntity)
            {
                entityType.Key.PropertyRefs.Clear();
            }
        }

        private void CreateStorageEntityTypeProperties(ISchemaEntity entity, EntityTypeStore entityType)
        {
            //<Property Name="CategoryId" Type="varchar" Nullable="false" MaxLength="10"  />
            foreach (ISchemaProperty property in entity.Properties)
            {
                var entityProperty = entityType.Properties.Where(p => property.KeyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase) || property.KeyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (entityProperty == null)
                {
                    if (ExcludeProperty(property))
                        continue;

                    entityProperty = new EntityProperty() { Name = property.KeyName };
                    entityType.Properties.Add(entityProperty);

                    _newStorageEntityProperties.Add(String.Format("{0}-{1}", entity.Name, property.Name));
                }
                else if (ExcludeProperty(property))
                {
                    entityType.Properties.Remove(entityProperty);
                    continue;
                }

                entityProperty.Name = property.KeyName;
                entityProperty.Type = GetNativeType(property);

                if (!property.IsNullable)
                    entityProperty.Nullable = property.IsNullable;

                entityProperty.DefaultValue = string.IsNullOrEmpty(entityProperty.DefaultValue) && !string.IsNullOrEmpty(property.DefaultValue) ? property.DefaultValue : null;
                entityProperty.MaxLength = !string.IsNullOrEmpty(GetMaxLength(property)) && !GetMaxLength(property).Equals("Max", StringComparison.InvariantCultureIgnoreCase) && !GetNativeType(property).Equals("timestamp", StringComparison.InvariantCultureIgnoreCase) ? GetMaxLength(property) : null;

                entityProperty.StoreGeneratedPattern = property.IsIdentity ? EdmxConstants.StoreGeneratedPatternIdentity : property.IsComputed ? EdmxConstants.StoreGeneratedPatternComputed : null;
            }
        }

        private void CreateStorageAssociationSet(Association association)
        {
            IEntity principalEntity;
            IEntity dependentEntity;
            bool isParentEntity;
            string key;
            string toRole;
            string fromRole;
            ResolveStorageAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);
            //<AssociationSet Name="FK__Product__Categor__0CBAE877" Association="PetShopModel1.Store.FK__Product__Categor__0CBAE877">
            //    <End Role="Category" EntitySet="Category" />
            //    <End Role="Product" EntitySet="Product" />
            //</AssociationSet>
            var associationSet = StorageSchemaEntityContainer.AssociationSets.Where(e => e.Name.Equals(association.AssociationKeyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (associationSet == null)
            {
                associationSet = new EntityContainer.AssociationSetLocalType
                {
                    Name = association.AssociationKeyName,
                    Ends = new List<EntityContainer.AssociationSetLocalType.EndLocalType>()
                };

                StorageSchemaEntityContainer.AssociationSets.Add(associationSet);
            }
            else
            {
                // Remove the AssociationEnd's that don't exist.
                var items = associationSet.Ends.Where(e => (!e.Role.Equals(toRole) || !e.Role.Equals(ResolveStorageEntityName(association.Entity.EntityKeyName))) &&
                                                           (!e.Role.Equals(fromRole) || !e.Role.Equals(ResolveStorageEntityName(association.ForeignEntity.EntityKeyName))));
                foreach (var associationEnd in items)
                {
                    associationSet.Ends.Remove(associationEnd);
                }
            }

            // Set or sync the default values.
            associationSet.Name = association.AssociationKeyName;
            associationSet.Association = string.Concat(StorageSchema.Namespace, ".", association.AssociationKeyName);

            var principalEnd = CreateStorageAssociationSetEnd(principalEntity, toRole, associationSet);
            var dependentEnd = CreateStorageAssociationSetEnd(dependentEntity, fromRole, associationSet, principalEnd);

            // Update the Ends (forces the ends to be grouped in the edmx file).
            associationSet.Ends = (from a in associationSet.Ends orderby a.Role select a).ToList();
        }

        private EntityContainer.AssociationSetLocalType.EndLocalType CreateStorageAssociationSetEnd(IEntity entity, string role, EntityContainer.AssociationSetLocalType set, EntityContainer.AssociationSetLocalType.EndLocalType otherEnd = null)
        {
            var end = otherEnd == null ?
                set.Ends.Where(e => e.Role.Equals(role) || e.Role.Equals(ResolveStorageEntityName(entity.EntityKeyName))).FirstOrDefault() :
                set.Ends.Where(e => !e.Equals(otherEnd)).FirstOrDefault();

            if (end == null)
            {
                end = new EntityContainer.AssociationSetLocalType.EndLocalType() { Role = role };
                set.Ends.Add(end);
            }

            end.Role = role;
            end.EntitySet = entity.EntityKeyName;

            return end;
        }

        private void CreateStorageAssociation(Association association)
        {   
            //<Association Name="FK__Product__Categor__0CBAE877">
            //  <End Role="Category" Type="PetShopModel1.Store.Category" Multiplicity="1" />
            //  <End Role="Product" Type="PetShopModel1.Store.Product" Multiplicity="*" />
            //  <ReferentialConstraint>
            //      <Principal Role="Category">
            //          <PropertyRef Name="CategoryId" />
            //      </Principal>
            //      <Dependent Role="Product">
            //          <PropertyRef Name="CategoryId" />
            //      </Dependent>
            //  </ReferentialConstraint>
            //</Association>
            IEntity principalEntity;
            IEntity dependentEntity;
            bool isParentEntity;
            string key;
            string toRole;
            string fromRole;
            ResolveStorageAssociationValues(association, out principalEntity, out dependentEntity, out isParentEntity, out key, out toRole, out fromRole);

            // The associations are stupid and if an end is added after the ReferentialConstraint, than the API doesn't detect it...
            var assoc = StorageSchema.Associations.Where(a => a.Name.Equals(association.AssociationKeyName)).FirstOrDefault();
            if (assoc != null) StorageSchema.Associations.Remove(assoc);
            assoc = new LinqToEdmx.Model.Storage.Association()
            {
                Name = association.AssociationKeyName,
                Ends = new List<AssociationEnd>()
            };

            StorageSchema.Associations.Add(assoc);

            var principalEnd = CreateStorageAssociationEnd(principalEntity, toRole, assoc, IsCascadeDelete(association));
            var dependentEnd = CreateStorageAssociationEnd(dependentEntity, fromRole, assoc, false);

            UpdateStorageAssociationEndMultiplicity(association, principalEnd, dependentEnd);

            assoc.ReferentialConstraint = new LinqToEdmx.Model.Storage.Constraint
            {
                Principal = new ReferentialConstraintRoleElement() { Role = toRole },
                Dependent = new ReferentialConstraintRoleElement() { Role = fromRole }
            };

            CreateStorageAssociationReferentialConstraintProperties(assoc, association.Properties, association.IsParentManyToMany());

            _storageAssociations.Add(association.AssociationKeyName);
        }

        private AssociationEnd CreateStorageAssociationEnd(IEntity entity, string role, LinqToEdmx.Model.Storage.Association assocication, bool isCascadeDelete)
        {
            //  <End Role="Category" Type="PetShopModel1.Store.Category" Multiplicity="1">
            //     <OnDelete Action="Cascade" /> 
            //  </End> 
            //  <End Role="Product" Type="PetShopModel1.Store.Product" Multiplicity="*" />
            var end = new AssociationEnd()
            {
                Role = role,
                Type = string.Concat(StorageSchema.Namespace, ".", entity.EntityKeyName)
            };

            if (isCascadeDelete)
            {
                end.OnDelete.Add(new OnAction() { Action = EdmxConstants.OnDeleteActionCascade });
            }

            assocication.Ends.Add(end);

            return end;
        }

        private static void UpdateStorageAssociationEndMultiplicity(Association association, AssociationEnd principalEnd, AssociationEnd dependentEnd)
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
                default:
                    principalEnd.Multiplicity = MultiplicityConstants.One;
                    dependentEnd.Multiplicity = MultiplicityConstants.Many;
                    break;
            }
        }

        private static void CreateStorageAssociationReferentialConstraintProperties(LinqToEdmx.Model.Storage.Association association, IEnumerable<AssociationProperty> properties, bool isParentEntity)
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

                var principalProperty = association.ReferentialConstraint.Principal.PropertyRefs.Where(p => p.Name == principalProp.KeyName).FirstOrDefault();
                if (principalProperty == null)
                {
                    principalProperty = new PropertyRef()
                    {
                        Name = principalProp.KeyName
                    };
                    association.ReferentialConstraint.Principal.PropertyRefs.Add(principalProperty);
                }

                var dependentProperty = association.ReferentialConstraint.Dependent.PropertyRefs.Where(p => p.Name == dependentProp.KeyName).FirstOrDefault();
                if (dependentProperty == null)
                {
                    dependentProperty = new PropertyRef()
                    {
                        Name = dependentProp.KeyName
                    };
                    association.ReferentialConstraint.Dependent.PropertyRefs.Add(dependentProperty);
                }
            }
        }

        private static void ResolveStorageAssociationValues(Association association, out IEntity principalEntity, out IEntity dependentEntity, out bool isParentEntity, out string keyName, out string toRole, out string fromRole)
        {
            bool isManyToManyEntity = association.IsParentManyToMany();
            principalEntity = !isManyToManyEntity ? association.Entity : association.ForeignEntity;
            dependentEntity = !isManyToManyEntity ? association.ForeignEntity : association.Entity;

            toRole = principalEntity.EntityKeyName;
            fromRole = dependentEntity.EntityKeyName;
            if (toRole.Equals(fromRole)) fromRole += 1;

            keyName = isManyToManyEntity ? association.Entity.EntityKeyName : association.AssociationKeyName;

            isParentEntity = association.IsParentEntity;
            if (association.AssociationType == AssociationType.ManyToMany)
                isParentEntity &= association.IsParentManyToMany();
        }

        private string ResolveStorageEntityName(string entityName)
        {
            if (_storageEntityNames.ContainsKey(entityName))
                return _storageEntityNames[entityName];

            return entityName;
        }

        private static string GetNativeType(ISchemaProperty property)
        {
            string nativeType = property.NativeType;
            switch (property.DataType)
            {
                case DbType.Binary:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    {
                        if (property.NativeType != "text" && property.NativeType != "ntext" &&
                            property.NativeType != "timestamp" && property.NativeType != "image")
                            if (property.Size == -1)
                                nativeType += "(max)";
                        break;
                    }
            }

            return nativeType;
        }

        #endregion
    }
}
