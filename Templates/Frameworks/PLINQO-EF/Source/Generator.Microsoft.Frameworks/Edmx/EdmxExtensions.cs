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
using System.Xml.Linq;
using CodeSmith.SchemaHelper;
using LinqToEdmx.Model.Conceptual;
using Xml.Schema.Linq;

namespace Generator.Microsoft.Frameworks
{
    public static class Extensions
    {
        public static void SetAttributeValue(this XTypedElement element, string key, string value)
        {
            if (element == null || String.IsNullOrEmpty(key)) return;

            element.Untyped.SetAttributeValue(XName.Get(key, EdmxConstants.CustomAttributeNameSpace), value);
        }

        public static string GetAttributeValue(this XTypedElement element, string key)
        {
            if (element == null || String.IsNullOrEmpty(key)) return null;

            var attribute = element.Untyped.Attribute(XName.Get(key, EdmxConstants.CustomAttributeNameSpace));

            return attribute != null ? attribute.Value : null;
        }

        public static bool IsParentManyToMany(this IAssociation association)
        {
            return association.AssociationType == AssociationType.ManyToMany && association.IntermediaryAssociation != null && association.IntermediaryAssociation.IsParentEntity ||
                   association.AssociationType == AssociationType.ManyToMany && association.IntermediaryAssociation == null && association.IsParentEntity;
        }

        public static bool IsParentManyToMany(this IEntity entity)
        {
            return entity.GetAssociations(AssociationType.ManyToMany).Count == 1 && entity.GetAssociations(AssociationType.ManyToMany).First().IntermediaryAssociation.IsParentEntity;
        }

        public static bool IsComplexType(this EntityProperty property, string conceptualNamespace)
        {
            if (property == null || property.Type == null) return false;

            return property.Type.ToString().StartsWith(conceptualNamespace);
        }

        #region Complex Type Extensions

        public static bool Exists(this IEnumerable<ComplexType> types, string name, string conceptualNamespace = "")
        {
            if (types == null || String.IsNullOrEmpty(name)) return false;

            name = String.IsNullOrEmpty(conceptualNamespace) ? name : name.Replace(String.Concat(conceptualNamespace, "."), "");
            return types.Count(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        public static ComplexType Find(this IEnumerable<ComplexType> types, string name, string conceptualNamespace = "")
        {
            if (types == null || String.IsNullOrEmpty(name)) return null;

            name = String.IsNullOrEmpty(conceptualNamespace) ? name : name.Replace(String.Concat(conceptualNamespace, "."), "");
            return types.Where(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public static bool IsUsed(this ComplexType type)
        {
            if (type == null) return false;

            //TODO: Look through the edmx file and see if a specific ComplexType is being used.
            return true;
        }

        #endregion

        #region Function Extensions

        public static bool IsComplexType(this EntityContainer.FunctionImportLocalType function, string conceptualNamespace)
        {
            if (function == null || function.ReturnType == null) return false;

            return function.ReturnType.ToString().Trim().StartsWith(String.Concat("Collection(", conceptualNamespace));
        }

        public static bool Exists(this IEnumerable<LinqToEdmx.Model.Storage.Function> functions, string name, string storageNamespace = "")
        {
            if (functions == null || String.IsNullOrEmpty(name)) return false;

            name = String.IsNullOrEmpty(storageNamespace) ? name : name.Replace(String.Concat(storageNamespace, "."), "");
            return functions.Count(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        public static bool Exists(this IEnumerable<EntityContainer.FunctionImportLocalType> functions, string name)
        {
            if (functions == null || String.IsNullOrEmpty(name)) return false;

            return functions.Count(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        #endregion
    }
}
