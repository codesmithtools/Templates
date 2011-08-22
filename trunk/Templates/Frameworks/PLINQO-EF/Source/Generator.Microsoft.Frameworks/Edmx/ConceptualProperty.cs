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
using System.Linq;
using CodeSmith.SchemaHelper.Util;
using Generator.Microsoft.Frameworks;
using Generator.Microsoft.Frameworks.Utility;
using LinqToEdmx.Model.Conceptual;

namespace CodeSmith.SchemaHelper
{
    [System.Diagnostics.DebuggerDisplay("ConceptualProperty = {Name}, Type = {SystemType}, Key = {KeyName}, Entity = {Entity.Name}")]
    public sealed class ConceptualProperty : PropertyBase<EntityProperty>
    {
        #region Constructor(s)

        /// <summary>
        /// Constructor that passes in the entity that this class will represent.
        /// </summary>
        public ConceptualProperty(EntityProperty property, IEntity entity) : base(property, entity)
        {
        }

        #endregion

        #region Method Overrides

        /// <summary>
        /// Loads the Property Settings.
        /// This method is called from the base classes constructor.
        /// </summary>
        public override void Initialize()
        {
            #region Base Properties

            KeyName = PropertySource.Name;
            Name = PropertySource.Name;

            if (PropertySource.Documentation != null && PropertySource.Documentation.LongDescription != null)
                Description = PropertySource.Documentation.LongDescription.ToString();

            GetterAccess = PropertySource.GetterAccess;
            SetterAccess = PropertySource.SetterAccess;

            #region Data Type Related

            if (SystemTypeMapper.EfConceptualTypeToSystemType.ContainsKey(PropertySource.Type.ToString()))
            {
                SystemType = SystemTypeMapper.EfConceptualTypeToSystemType[PropertySource.Type.ToString()];
            }
            else
            {
                SystemType = PropertySource.Type.ToString().Replace(string.Concat(((ConceptualEntity)Entity).Namespace, "."), "");
                ExtendedProperties.Add(EdmxConstants.ComplexPropertyExtendedProperty, true);
            }
            
            DefaultValue = PropertySource.DefaultValue;
            Scale = PropertySource.Scale ?? 0;
            Precision = PropertySource.Precision ?? 0;
            FixedLength = PropertySource.FixedLength ?? false;

            int temp;
            if (PropertySource.MaxLength != null && Int32.TryParse(PropertySource.MaxLength.ToString(), out temp))
            {
                Size = temp;
            }

            Unicode = PropertySource.Unicode ?? false;

            IsNullable = PropertySource.Nullable;

            #endregion

            SystemType = TypeHelper.ResolveSystemType(SystemType, IsNullable, true);
            PropertyType = ResolvePropertyType();

            #endregion

            #region EDMX Specific Properties

            #endregion
        }

        #endregion

        #region Private Methods

        private PropertyType ResolvePropertyType()
        {
            PropertyType? type = null;

            var isPrimaryKey = Entity.HasKey && Entity.Key.Properties.Count(p => p.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) > 0;
            if (isPrimaryKey) 
                type = PropertyType.Key;

            // Concurrency
            if (!string.IsNullOrEmpty(PropertySource.ConcurrencyMode) && PropertySource.ConcurrencyMode.Equals(EdmxConstants.ConcurrencyModeFixed, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!type.HasValue)
                    type = PropertyType.Concurrency;
                else
                    type |= PropertyType.Concurrency;
            }

            // http://msdn.microsoft.com/en-us/library/system.data.metadata.edm.storegeneratedpattern.aspx
            switch (PropertySource.StoreGeneratedPattern)
            {
                case EdmxConstants.StoreGeneratedPatternIdentity:
                    if (!type.HasValue)
                        type = PropertyType.Identity;
                    else
                        type |= PropertyType.Identity;
                    break;
                case EdmxConstants.StoreGeneratedPatternComputed:
                    if (!type.HasValue)
                        type = PropertyType.Computed;
                    else
                        type |= PropertyType.Computed;
                    break;
                case EdmxConstants.ConcurrencyModeNone:
                    break;
            }

            // Check for index.
            if (Boolean.TrueString.Equals(PropertySource.GetAttributeValue(EdmxConstants.IsIndexCustomAttribute)))
            {
                if (!type.HasValue)
                    type = PropertyType.Index;
                else
                    type |= PropertyType.Index;
            }

            //if (IsForeignKey) type &= Enums.PropertyType.Foreign;

            return type ?? PropertyType.Normal;
        }

        #endregion
    }
}
