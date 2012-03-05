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
using System.Linq;
using Generator.Microsoft.Frameworks;
using LinqToEdmx.Model.Conceptual;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConceptualEntity = {Name}, Key = {EntityKeyName}")]
    public sealed class ConceptualEntity : EntityBase<EntityType>
    {
        private readonly IEnumerable<LinqToEdmx.Model.Conceptual.Association> _associations;
        private readonly IEnumerable<EntityContainer.AssociationSetLocalType> _associationSets;

        #region Constructor(s)

        /// <summary>
        /// Constructor that passes in the entity that this class will represent.
        /// </summary>
        public ConceptualEntity(EntityType entity, string @namespace) : base(entity, @namespace)
        {
            EntityKeyName = EntitySource.Name;
            Name = EntitySource.Name;

            if (!string.IsNullOrEmpty(EntitySource.BaseType))
            {
                BaseType = EntitySource.BaseType.Replace(string.Concat(Namespace, "."), "");
            }

            IsAbstract = EntitySource.IsAbstract;
            TypeAccess = !string.IsNullOrEmpty(EntitySource.TypeAccess) ? EntitySource.TypeAccess : AccessibilityConstants.Public;

            LoadKeys();
            LoadProperties();
        }

        public ConceptualEntity(EntityType entity, IEnumerable<LinqToEdmx.Model.Conceptual.Association> associations, IEnumerable<EntityContainer.AssociationSetLocalType> associationSets, string @namespace) : this(entity, @namespace)
        {
            _associations = associations;
            _associationSets = associationSets;
        }

        #endregion

        #region Public Overrides

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            LoadAssociations();
            PopulateInheritanceProperties();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override to populate the properties from the implemented entity.
        /// </summary>
        protected override void LoadProperties()
        {
            foreach (var prop in EntitySource.Properties)
            {
                var property = new ConceptualProperty(prop, this);
                if (!Configuration.Instance.ExcludeRegexIsMatch(prop.Name) && !PropertyMap.ContainsKey(property.Name))
                {
                    PropertyMap.Add(property.Name, property);
                }
            }

            if (PropertyMap.Values.Where(em => (em.PropertyType & PropertyType.Concurrency) == PropertyType.Concurrency).Count() > 1)
                throw new Exception(String.Format("More than one Concurrency property in {0}", EntityKeyName));
        }

        /// <summary>
        /// Override to populate the associations from the implemented entity.
        /// </summary>
        protected override void LoadAssociations()
        {
            if (_associations == null || _associations.Count() == 0 || _associationSets == null || _associationSets.Count() == 0) return;
            
            //<AssociationSet Name="FK__Item__Supplier__1273C1CD" Association="PetShop.Data.FK__Item__Supplier__1273C1CD">
            //
            // <Association Name="FK__Item__Supplier__1273C1CD">
            //  <End Role="Supplier" Type="PetShop.Data.Supplier" Multiplicity="0..1" />
            //  <End Role="Item" Type="PetShop.Data.Item" Multiplicity="*" />
            //  <ReferentialConstraint>
            //    <Principal Role="Supplier">
            //      <PropertyRef Name="SuppId" />
            //    </Principal>
            //    <Dependent Role="Item">
            //      <PropertyRef Name="Supplier" />
            //    </Dependent>
            //  </ReferentialConstraint>
            //</Association>
            //
            // Custom Association:
            //
            //<Association Name="InventoryCustomProperties">
            //  <End Type="PetShop.Data.Inventory" Role="Inventory" Multiplicity="1" />
            //  <End Type="PetShop.Data.CustomProperties" Role="CustomProperties" Multiplicity="1" />
            //</Association>
            //
            //<NavigationProperty Name="Supplier1" Relationship="PetShop.Data.FK__Item__Supplier__1273C1CD" FromRole="Item" ToRole="Supplier" />
            var relationships = (from np in EntitySource.NavigationProperties
                                join associationSet in _associationSets on
                                    np.Relationship.ToLower().Trim() equals associationSet.Association.ToLower().Trim()
                                join association in _associations on
                                    associationSet.Name.ToLower().Trim() equals association.Name.ToLower().Trim()
                                select new {NavigationProperty = np, Association = association}).ToList();

            foreach (var rel in relationships)
            {
                // This sucks but is there a better way to try and detect user defined association's principal role?
                var principalRoleName = rel.Association.ReferentialConstraint != null
                                            ? rel.Association.ReferentialConstraint.Principal.Role
                                            : rel.Association.Name.StartsWith(rel.NavigationProperty.FromRole, StringComparison.InvariantCultureIgnoreCase)
                                                  ? rel.NavigationProperty.FromRole
                                                  : rel.NavigationProperty.ToRole;

                var principalRole = rel.Association.Ends.Where(e => e.Role.Equals(principalRoleName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                var dependentRole = rel.Association.Ends.Where(e => e != principalRole).FirstOrDefault();
                if(principalRole == null || dependentRole == null) 
                    continue;

                IEntity principalEntity = EntityStore.Instance.GetEntity(principalRole.Type.Replace(string.Concat(Namespace, "."), ""));
                IEntity dependentEntity = EntityStore.Instance.GetEntity(dependentRole.Type.Replace(string.Concat(Namespace, "."), ""));
                if (principalEntity == null || dependentEntity == null) 
                    continue;

                Association association;
                AssociationType type;
                if (principalEntity.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) // Current Entity is the principal entity (e --fk--> this ( Parent ).
                {
                    if (principalRole.Multiplicity == MultiplicityConstants.ZeroToOne)
                        type = dependentRole.Multiplicity == MultiplicityConstants.ZeroToOne || dependentRole.Multiplicity == MultiplicityConstants.One ? AssociationType.OneToZeroOrOne : AssociationType.ZeroOrOneToMany;
                    else if (principalRole.Multiplicity == MultiplicityConstants.One)
                        type = dependentRole.Multiplicity == MultiplicityConstants.ZeroToOne ? AssociationType.OneToZeroOrOne : dependentRole.Multiplicity == MultiplicityConstants.One ? AssociationType.OneToOne : AssociationType.OneToMany;
                    else if (principalRole.Multiplicity == MultiplicityConstants.Many)
                        type = dependentRole.Multiplicity == MultiplicityConstants.ZeroToOne ? AssociationType.ManyToZeroOrOne : dependentRole.Multiplicity == MultiplicityConstants.One ? AssociationType.ManyToOne : AssociationType.ManyToMany;
                    else
                        throw new ArgumentException(String.Format("Invalid Multiplicity detected in the {0} Association.", rel.Association.Name));

                    // Note: There is no second association for ManyToMany associations...
                    association = new Association(rel.Association.Name, type, principalEntity, dependentEntity, true, Namespace) { Name = rel.NavigationProperty.Name };

                    if (rel.Association.ReferentialConstraint != null)
                    {
                        var properties = rel.Association.ReferentialConstraint.Principal.PropertyRefs;
                        var otherProperties = rel.Association.ReferentialConstraint.Dependent.PropertyRefs;
                        UpdatePropertyTypesWithForeignKeys(properties);

                        for (int index = 0; index < properties.Count; index++)
                        {
                            if (!PropertyMap.ContainsKey(properties[index].Name)) continue;
                            association.AddAssociationProperty(PropertyMap[properties[index].Name], dependentEntity.Properties.Where(p => p.KeyName.Equals(otherProperties[index].Name)).FirstOrDefault());
                        }
                    }
                }
                else // Current Entity is the dependent entity (child).
                {
                    if (dependentRole.Multiplicity == MultiplicityConstants.ZeroToOne)
                        type = principalRole.Multiplicity == MultiplicityConstants.ZeroToOne || principalRole.Multiplicity == MultiplicityConstants.One ? AssociationType.OneToZeroOrOne : AssociationType.ZeroOrOneToMany;
                    else if (dependentRole.Multiplicity == MultiplicityConstants.One)
                        type = principalRole.Multiplicity == MultiplicityConstants.ZeroToOne ? AssociationType.OneToZeroOrOne : principalRole.Multiplicity == MultiplicityConstants.One ? AssociationType.OneToOne : AssociationType.OneToMany;
                    else if (dependentRole.Multiplicity == MultiplicityConstants.Many)
                        type = principalRole.Multiplicity == MultiplicityConstants.ZeroToOne ? AssociationType.ManyToZeroOrOne : principalRole.Multiplicity == MultiplicityConstants.One ? AssociationType.ManyToOne : AssociationType.ManyToMany;
                    else
                        throw new ArgumentException(String.Format("Invalid Multiplicity detected in the {0} Association.", rel.Association.Name));

                    // Note: There is no second association for ManyToMany associations...
                    association = new Association(rel.Association.Name, type, dependentEntity, principalEntity, rel.Association.ReferentialConstraint != null ? false : true, Namespace) { Name = rel.NavigationProperty.Name };
                    
                    if (rel.Association.ReferentialConstraint != null)
                    {
                        var properties = rel.Association.ReferentialConstraint.Dependent.PropertyRefs;
                        var otherProperties = rel.Association.ReferentialConstraint.Principal.PropertyRefs;
                        UpdatePropertyTypesWithForeignKeys(properties);

                        for (int index = 0; index < properties.Count; index++)
                        {
                            if (!PropertyMap.ContainsKey(properties[index].Name)) continue;
                            association.AddAssociationProperty(PropertyMap[properties[index].Name], principalEntity.Properties.Where(p => p.KeyName.Equals(otherProperties[index].Name)).FirstOrDefault());
                        }
                    }
                }

                if ((rel.Association.ReferentialConstraint == null || association.Properties.Count > 0) && !string.IsNullOrEmpty(association.AssociationKeyName) && !AssociationMap.ContainsKey(association.AssociationKeyName))
                    AssociationMap.Add(association.AssociationKeyName, association);
            }
        }

        private void UpdatePropertyTypesWithForeignKeys(IEnumerable<PropertyRef> properties)
        {
            foreach (var property in properties)
            {
                var prop = PropertyMap[property.Name];
                prop.PropertyType |= PropertyType.Foreign;
            }
        }

        /// <summary>
        /// Override to populate the keys from the implemented entity.
        /// </summary>
        protected override void LoadKeys()
        {
            if (EntitySource.Key == null) return;

            var keys = (from property in EntitySource.Properties
                        join key in EntitySource.Key.PropertyRefs
                            on property.Name equals key.Name
                        select property).ToList();

            foreach (var prop in keys)
            {
                if (!Configuration.Instance.ExcludeRegexIsMatch(prop.Name))
                {
                    Key.Properties.Add(new ConceptualProperty(prop, this));
                }
            }
        }

        /// <summary>
        /// Load the extended properties for the entity.
        /// </summary>
        protected override void LoadExtendedProperties()
        {
            if (Boolean.TrueString.Equals(EntitySource.GetAttributeValue(EdmxConstants.IsViewEntityCustomAttribute)))
                ExtendedProperties.Add(EdmxConstants.IsViewEntityCustomAttribute, true);
        }

        protected override void PopulateInheritanceProperties()
        {
            if(!string.IsNullOrEmpty(BaseType))
                BaseEntity = EntityStore.Instance.GetEntity(BaseType);
            
            foreach (IEntity entity in EntityStore.Instance.EntityCollection.Values)
            {
                if ((entity is ConceptualEntity) == false || Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase) ||
                    string.IsNullOrEmpty(((ConceptualEntity)entity).BaseType) || !Name.Equals(((ConceptualEntity)entity).BaseType, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!DerivedEntities.Contains(entity))
                    DerivedEntities.Add(entity);
            }
        }

        #region Search Criteria Methods

        /// <summary>
        /// Load the Search Criteria for the entity
        /// </summary>
        protected override void LoadSearchCriteria()
        {
            switch (Configuration.Instance.SearchCriteriaProperty.SearchCriteria)
            {
                case SearchCriteriaType.All:
                    AddPrimaryKeySearchCriteria();
                    AddIndexSearchCriteria();
                    AddForeignKeySearchCriteria();
                    break;
                case SearchCriteriaType.ForeignKey:
                    AddForeignKeySearchCriteria();
                    break;
                case SearchCriteriaType.Index:
                    AddIndexSearchCriteria();
                    break;
                case SearchCriteriaType.PrimaryKey:
                    AddPrimaryKeySearchCriteria();
                    break;
                case SearchCriteriaType.NoForeignKeys:
                    AddIndexSearchCriteria();
                    AddPrimaryKeySearchCriteria();
                    break;
            }
        }

        /// <summary>
        /// Add PrimaryKeys to the SearchCriteria
        /// </summary>
        private void AddPrimaryKeySearchCriteria()
        {
            if (Key.Properties.Count == 0)
                return;

            var searchCriteria = new SearchCriteria(SearchCriteriaType.PrimaryKey);

            foreach (var member in Key.Properties)
            {
                if (member != null)
                    searchCriteria.Properties.Add(member);
            }

            searchCriteria.IsUniqueResult = true;

            AddToSearchCriteria(searchCriteria);
        }

        /// <summary>
        /// Add ForeignKeys to the SearchCriteria collection
        /// </summary>
        private void AddForeignKeySearchCriteria()
        {
            foreach (var association in AssociationMap.Values)
            {
                var searchCriteria = new SearchCriteria(SearchCriteriaType.ForeignKey);
                searchCriteria.Association = association;

                foreach (var property in association.Properties)
                {
                    searchCriteria.ForeignProperties.Add(property);
                    searchCriteria.Properties.Add(property.Property);
                }

                if (association.AssociationType == AssociationType.ManyToOne || association.AssociationType == AssociationType.ManyToZeroOrOne)
                {
                    AddToSearchCriteria(searchCriteria);
                }

                association.SearchCriteria = searchCriteria;
            }
        }

        /// <summary>
        /// Add all the indexes to the Search Criteria
        /// </summary>
        private void AddIndexSearchCriteria()
        {
            //foreach (IndexSchema indexSchema in _table.Indexes)
            //{
            //    var searchCriteria = new SearchCriteria(SearchCriteriaType.Index);

            //    foreach (var column in indexSchema.MemberColumns)
            //    {
            //        var name = column.Name;
            //        var property = Properties.Where(x => x.Name == name).FirstOrDefault();

            //        if (property != null)
            //            searchCriteria.Properties.Add(property);
            //    }

            //    if (indexSchema.IsUnique)
            //        searchCriteria.IsUniqueResult = true;

            //    AddToSearchCriteria(searchCriteria);
            //}
        }

        /// <summary>
        /// Add a SearchCriteria to the mapping collection
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        private void AddToSearchCriteria(SearchCriteria searchCriteria)
        {
            var key = searchCriteria.Key;

            var result = (!string.IsNullOrEmpty(key) && searchCriteria.Properties.Count > 0 && SearchCriteria.Where(x => x.Key == key).Count() == 0);

            if (result)
                SearchCriteria.Add(searchCriteria);

            return;
        }

        #endregion

        #endregion

        #region Properties

        private string BaseType { get; set; }

        #endregion
    }
}
