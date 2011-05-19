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
using System.Data;
using CodeSmith.SchemaHelper.Util;
using Generator.Microsoft.Frameworks;
using Generator.Microsoft.Frameworks.Utility;
using LinqToEdmx.Model.Conceptual;

namespace CodeSmith.SchemaHelper
{
    [System.Diagnostics.DebuggerDisplay("FunctionProperty = {Name}, Type = {SystemType}, Key = {KeyName}, Entity = {Entity.Name}")]
    public sealed class FunctionParameter : PropertyBase<FunctionImportParameter>
    {
        #region Constructor(s)

        /// <summary>
        /// Constructor that passes in the entity that this class will represent.
        /// </summary>
        public FunctionParameter(FunctionImportParameter property, IEntity entity) : base(property, entity)
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
            KeyName = PropertySource.Name;
            Name = PropertySource.Name;

            if (PropertySource.Documentation != null && PropertySource.Documentation.LongDescription != null)
                Description = PropertySource.Documentation.LongDescription.ToString();

            #region Data Type Related

            if (SystemTypeMapper.EfConceptualTypeToSystemType.ContainsKey(PropertySource.Type.ToString()))
            {
                SystemType = SystemTypeMapper.EfConceptualTypeToSystemType[PropertySource.Type.ToString()];
            }
            else
            {
                SystemType = PropertySource.Type.ToString().Replace(string.Concat(((FunctionEntity)Entity).Namespace, "."), "");
                ExtendedProperties.Add(EdmxConstants.ComplexPropertyExtendedProperty, true);
            }
            
            Scale = PropertySource.Scale ?? 0;
            Precision = PropertySource.Precision ?? 0;
            ParameterDirection = PropertySource.Mode == "In" ? ParameterDirection.Input : PropertySource.Mode == "InOut" ? ParameterDirection.InputOutput : ParameterDirection.Output;

            int temp;
            if (PropertySource.MaxLength != null && Int32.TryParse(PropertySource.MaxLength.ToString(), out temp))
            {
                Size = temp;
            }

            Unicode = false;
            IsNullable = false;
            
            #endregion

            SystemType = TypeHelper.ResolveSystemType(SystemType, IsNullable, true);
            PropertyType = PropertyType.Normal;
        }

        #endregion

        /// <summary>
        /// The direction of the Parameter of the command
        /// </summary>
        public ParameterDirection ParameterDirection { get; private set; }
    }
}
