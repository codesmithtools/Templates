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

namespace Generator.Microsoft.Frameworks
{
    public static class EdmxConstants
    {
        public const string ComplexPropertyExtendedProperty = "CS_IsComplexProperty";
        public const string EntityNamespace = "EntityNamespace";
        public const string ContextNamespace = "ContextNamespace";
        public const string ConcurrencyModeNone = "None";
        public const string ConcurrencyModeFixed = "Fixed";
        public const string StoreGeneratedPatternComputed = "Computed";
        public const string StoreGeneratedPatternIdentity = "Identity";
        public const string OnDeleteActionNone = "None";
        public const string OnDeleteActionCascade = "Cascade";
        public const string StorageSchemaGenerationTypeAttributeValueTables = "Tables";
        public const string StorageSchemaGenerationTypeAttributeValueViews = "Views";

        public const string IsIndexCustomAttribute = "IsUnique";
        public const string IsViewEntityCustomAttribute = "IsView";
        public const string IsFunctionEntityCustomAttribute = "IsFunction";
        public const string CustomAttributeNameSpace = "http://tempuri.org/Attribute";
    }
}