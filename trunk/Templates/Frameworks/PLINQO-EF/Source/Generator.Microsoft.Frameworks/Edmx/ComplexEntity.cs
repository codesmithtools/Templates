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
using System.Linq;
using LinqToEdmx.Model.Conceptual;
using Generator.Microsoft.Frameworks;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ComplexEntity = {Name}, Key = {EntityKeyName}")]
    public sealed class ComplexEntity : EntityBase<ComplexType>
    {
        #region Constructor(s)

        /// <summary>
        /// Constructor that passes in the entity that this class will represent.
        /// </summary>
        public ComplexEntity(ComplexType entity, string @namespace) : base(entity, @namespace)
        {
            EntityKeyName = EntitySource.Name;
            Name = EntitySource.Name;
            TypeAccess = !string.IsNullOrEmpty(EntitySource.TypeAccess) ? EntitySource.TypeAccess : AccessibilityConstants.Public;

            LoadProperties();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
        }
        
        /// <summary>
        /// Override to populate the properties from the implemented entity.
        /// </summary>
        protected override void LoadProperties()
        {
            foreach (var prop in EntitySource.Properties)
            {
                if (!Configuration.Instance.ExcludeRegexIsMatch(prop.Name) && !PropertyMap.ContainsKey(prop.Name))
                {
                    var property = new ComplexProperty(prop, this);

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
        }

        /// <summary>
        /// Override to populate the keys from the implemented entity.
        /// </summary>
        protected override void LoadKeys()
        {
        }

        /// <summary>
        /// Load the extended properties for the entity.
        /// </summary>
        protected override void LoadExtendedProperties()
        {
            if(EntitySource.GetAttributeValue(EdmxConstants.IsFunctionEntityCustomAttribute) != null)
                ExtendedProperties.Add(EdmxConstants.IsFunctionEntityCustomAttribute, true);
        }

        protected override void PopulateInheritanceProperties()
        {
        }

        /// <summary>
        /// Load the Search Criteria for the entity
        /// </summary>
        protected override void LoadSearchCriteria()
        {
        }

        #endregion
    }
}
