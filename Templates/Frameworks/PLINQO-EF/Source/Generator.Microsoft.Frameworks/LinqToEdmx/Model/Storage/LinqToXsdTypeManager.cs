using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Designer;
using LinqToEdmx.Map;
using LinqToEdmx.Model.Conceptual;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  public class LinqToXsdTypeManager : ILinqToXsdTypeManager
  {
    private static readonly Dictionary<XName, Type> TypeDictionary = new Dictionary<XName, Type>();

    private static readonly Dictionary<XName, Type> ElementDictionary = new Dictionary<XName, Type>();

    private static readonly Dictionary<Type, Type> WrapperDictionary = new Dictionary<Type, Type>();

    private static XmlSchemaSet _schemaSet;

    private static readonly LinqToXsdTypeManager TypeManagerSingleton = new LinqToXsdTypeManager();

    static LinqToXsdTypeManager()
    {
      BuildTypeDictionary();
      BuildElementDictionary();
      BuildWrapperDictionary();
    }

    public static LinqToXsdTypeManager Instance
    {
      get
      {
        return TypeManagerSingleton;
      }
    }

    #region ILinqToXsdTypeManager Members

    XmlSchemaSet ILinqToXsdTypeManager.Schemas
    {
      get
      {
        if ((_schemaSet == null))
        {
          var tempSet = new XmlSchemaSet();
          Interlocked.CompareExchange(ref _schemaSet, tempSet, null);
        }
        return _schemaSet;
      }
      set
      {
        _schemaSet = value;
      }
    }

    Dictionary<XName, Type> ILinqToXsdTypeManager.GlobalTypeDictionary
    {
      get
      {
        return TypeDictionary;
      }
    }

    Dictionary<XName, Type> ILinqToXsdTypeManager.GlobalElementDictionary
    {
      get
      {
        return ElementDictionary;
      }
    }

    Dictionary<Type, Type> ILinqToXsdTypeManager.RootContentTypeMapping
    {
      get
      {
        return WrapperDictionary;
      }
    }

    #endregion

    private static void BuildTypeDictionary()
    {
      TypeDictionary.Add(XName.Get("TEdmx", XMLNamespaceFactory.Edmx), typeof(TEdmx));
      TypeDictionary.Add(XName.Get("TDesigner", XMLNamespaceFactory.Edmx), typeof (Designer.Designer));
      TypeDictionary.Add(XName.Get("TOptions", XMLNamespaceFactory.Edmx), typeof (Options));
      TypeDictionary.Add(XName.Get("TDiagrams", XMLNamespaceFactory.Edmx), typeof (Diagrams));
      TypeDictionary.Add(XName.Get("TConnection", XMLNamespaceFactory.Edmx), typeof (Connection));
      TypeDictionary.Add(XName.Get("TDesignerInfoPropertySet", XMLNamespaceFactory.Edmx), typeof (DesignerInfoPropertySet));
      TypeDictionary.Add(XName.Get("TDesignerProperty", XMLNamespaceFactory.Edmx), typeof (DesignerProperty));
      TypeDictionary.Add(XName.Get("TDiagram", XMLNamespaceFactory.Edmx), typeof (Diagram));
      TypeDictionary.Add(XName.Get("TEntityTypeShape", XMLNamespaceFactory.Edmx), typeof (EntityTypeShape));
      TypeDictionary.Add(XName.Get("TAssociationConnector", XMLNamespaceFactory.Edmx), typeof (AssociationConnector));
      TypeDictionary.Add(XName.Get("TInheritanceConnector", XMLNamespaceFactory.Edmx), typeof (InheritanceConnector));
      TypeDictionary.Add(XName.Get("TConnectorPoint", XMLNamespaceFactory.Edmx), typeof (ConnectorPoint));
      TypeDictionary.Add(XName.Get("TRuntime", XMLNamespaceFactory.Edmx), typeof (Runtime));
      TypeDictionary.Add(XName.Get("TRuntimeConceptualModels", XMLNamespaceFactory.Edmx), typeof (ConceptualModels));
      TypeDictionary.Add(XName.Get("TRuntimeStorageModels", XMLNamespaceFactory.Edmx), typeof (StorageModels));
      TypeDictionary.Add(XName.Get("TRuntimeMappings", XMLNamespaceFactory.Edmx), typeof (Mappings));
      TypeDictionary.Add(XName.Get("TDataServices", XMLNamespaceFactory.Edmx), typeof (DataServices));
      TypeDictionary.Add(XName.Get("TSchema", XMLNamespaceFactory.SSDL), typeof (TSchema));
      TypeDictionary.Add(XName.Get("TDocumentation", XMLNamespaceFactory.SSDL), typeof (Documentation));
      TypeDictionary.Add(XName.Get("TText", XMLNamespaceFactory.SSDL), typeof (Text));
      TypeDictionary.Add(XName.Get("TAssociation", XMLNamespaceFactory.SSDL), typeof (Association));
      TypeDictionary.Add(XName.Get("TConstraint", XMLNamespaceFactory.SSDL), typeof (Constraint));
      TypeDictionary.Add(XName.Get("TReferentialConstraintRoleElement", XMLNamespaceFactory.SSDL), typeof (ReferentialConstraintRoleElement));
      TypeDictionary.Add(XName.Get("TEntityType", XMLNamespaceFactory.SSDL), typeof (EntityTypeStore));
      TypeDictionary.Add(XName.Get("TEntityKeyElement", XMLNamespaceFactory.SSDL), typeof (EntityKeyElement));
      TypeDictionary.Add(XName.Get("TPropertyRef", XMLNamespaceFactory.SSDL), typeof (PropertyRef));
      TypeDictionary.Add(XName.Get("TAssociationEnd", XMLNamespaceFactory.SSDL), typeof (AssociationEnd));
      TypeDictionary.Add(XName.Get("TOnAction", XMLNamespaceFactory.SSDL), typeof (OnAction));
      TypeDictionary.Add(XName.Get("TEntityProperty", XMLNamespaceFactory.SSDL), typeof (EntityProperty));
      TypeDictionary.Add(XName.Get("TFunction", XMLNamespaceFactory.SSDL), typeof (Function));
      TypeDictionary.Add(XName.Get("TParameter", XMLNamespaceFactory.SSDL), typeof (Parameter));
      TypeDictionary.Add(XName.Get("TSchema", XMLNamespaceFactory.Edm), typeof (Conceptual.TSchema));
      TypeDictionary.Add(XName.Get("TDocumentation", XMLNamespaceFactory.Edm), typeof (Conceptual.Documentation));
      TypeDictionary.Add(XName.Get("TText", XMLNamespaceFactory.Edm), typeof (Conceptual.Text));
      TypeDictionary.Add(XName.Get("TXmlOrText", XMLNamespaceFactory.Edm), typeof (XmlOrText));
      TypeDictionary.Add(XName.Get("TUsing", XMLNamespaceFactory.Edm), typeof (Using));
      TypeDictionary.Add(XName.Get("TAssociation", XMLNamespaceFactory.Edm), typeof (Conceptual.Association));
      TypeDictionary.Add(XName.Get("TComplexType", XMLNamespaceFactory.Edm), typeof (ComplexType));
      TypeDictionary.Add(XName.Get("TConstraint", XMLNamespaceFactory.Edm), typeof (Conceptual.Constraint));
      TypeDictionary.Add(XName.Get("TReferentialConstraintRoleElement", XMLNamespaceFactory.Edm), typeof (Conceptual.ReferentialConstraintRoleElement));
      TypeDictionary.Add(XName.Get("TNavigationProperty", XMLNamespaceFactory.Edm), typeof (NavigationProperty));
      TypeDictionary.Add(XName.Get("TEntityType", XMLNamespaceFactory.Edm), typeof (Conceptual.EntityType));
      TypeDictionary.Add(XName.Get("TFunction", XMLNamespaceFactory.Edm), typeof (Conceptual.Function));
      TypeDictionary.Add(XName.Get("TFunctionParameter", XMLNamespaceFactory.Edm), typeof (FunctionParameter));
      TypeDictionary.Add(XName.Get("TCollectionType", XMLNamespaceFactory.Edm), typeof (TCollectionType));
      TypeDictionary.Add(XName.Get("TTypeRef", XMLNamespaceFactory.Edm), typeof (TypeRef));
      TypeDictionary.Add(XName.Get("TReferenceType", XMLNamespaceFactory.Edm), typeof (ReferenceType));
      TypeDictionary.Add(XName.Get("TRowType", XMLNamespaceFactory.Edm), typeof (RowType));
      TypeDictionary.Add(XName.Get("TProperty", XMLNamespaceFactory.Edm), typeof (Property));
      TypeDictionary.Add(XName.Get("TFunctionReturnType", XMLNamespaceFactory.Edm), typeof (FunctionReturnType));
      TypeDictionary.Add(XName.Get("TEntityKeyElement", XMLNamespaceFactory.Edm), typeof (Conceptual.EntityKeyElement));
      TypeDictionary.Add(XName.Get("TPropertyRef", XMLNamespaceFactory.Edm), typeof (Conceptual.PropertyRef));
      TypeDictionary.Add(XName.Get("TAssociationEnd", XMLNamespaceFactory.Edm), typeof (Conceptual.AssociationEnd));
      TypeDictionary.Add(XName.Get("TOnAction", XMLNamespaceFactory.Edm), typeof (Conceptual.OnAction));
      TypeDictionary.Add(XName.Get("TEntityProperty", XMLNamespaceFactory.Edm), typeof (Conceptual.EntityProperty));
      TypeDictionary.Add(XName.Get("TComplexTypeProperty", XMLNamespaceFactory.Edm), typeof (ComplexTypeProperty));
      TypeDictionary.Add(XName.Get("TFunctionImportParameter", XMLNamespaceFactory.Edm), typeof (FunctionImportParameter));
      TypeDictionary.Add(XName.Get("TMapping", XMLNamespaceFactory.CS), typeof (TMapping));
      TypeDictionary.Add(XName.Get("TQueryView", XMLNamespaceFactory.CS), typeof (QueryView));
      TypeDictionary.Add(XName.Get("TAlias", XMLNamespaceFactory.CS), typeof (Alias));
      TypeDictionary.Add(XName.Get("TEntityContainerMapping", XMLNamespaceFactory.CS), typeof (EntityContainerMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportMapping", XMLNamespaceFactory.CS), typeof (FunctionImportMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportMappingResultMapping", XMLNamespaceFactory.CS), typeof (FunctionImportMappingResultMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportComplexTypeMapping", XMLNamespaceFactory.CS), typeof (FunctionImportComplexTypeMapping));
      TypeDictionary.Add(XName.Get("TEntitySetMapping", XMLNamespaceFactory.CS), typeof (EntitySetMapping));
      TypeDictionary.Add(XName.Get("TAssociationSetMapping", XMLNamespaceFactory.CS), typeof (AssociationSetMapping));
      TypeDictionary.Add(XName.Get("TEntityTypeMapping", XMLNamespaceFactory.CS), typeof (EntityTypeMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportEntityTypeMapping", XMLNamespaceFactory.CS), typeof (FunctionImportEntityTypeMapping));
      TypeDictionary.Add(XName.Get("TMappingFragment", XMLNamespaceFactory.CS), typeof (MappingFragment));
      TypeDictionary.Add(XName.Get("TEntityTypeModificationFunctionMapping", XMLNamespaceFactory.CS), typeof (EntityTypeModificationFunctionMapping));
      TypeDictionary.Add(XName.Get("TAssociationSetModificationFunctionMapping", XMLNamespaceFactory.CS), typeof (AssociationSetModificationFunctionMapping));
      TypeDictionary.Add(XName.Get("TEntityTypeModificationFunction", XMLNamespaceFactory.CS), typeof (EntityTypeModificationFunction));
      TypeDictionary.Add(XName.Get("TEntityTypeModificationFunctionWithResult", XMLNamespaceFactory.CS), typeof (EntityTypeModificationFunctionWithResult));
      TypeDictionary.Add(XName.Get("TAssociationSetModificationFunction", XMLNamespaceFactory.CS), typeof (AssociationSetModificationFunction));
      TypeDictionary.Add(XName.Get("TFunctionMappingEndProperty", XMLNamespaceFactory.CS), typeof (FunctionMappingEndProperty));
      TypeDictionary.Add(XName.Get("TFunctionMappingScalarProperty", XMLNamespaceFactory.CS), typeof (FunctionMappingScalarProperty));
      TypeDictionary.Add(XName.Get("TResultBinding", XMLNamespaceFactory.CS), typeof (ResultBinding));
      TypeDictionary.Add(XName.Get("TFunctionMappingAssociationEnd", XMLNamespaceFactory.CS), typeof (FunctionMappingAssociationEnd));
      TypeDictionary.Add(XName.Get("TFunctionMappingComplexProperty", XMLNamespaceFactory.CS), typeof (FunctionMappingComplexProperty));
      TypeDictionary.Add(XName.Get("TCondition", XMLNamespaceFactory.CS), typeof (Condition));
      TypeDictionary.Add(XName.Get("TFunctionImportCondition", XMLNamespaceFactory.CS), typeof (FunctionImportCondition));
      TypeDictionary.Add(XName.Get("TEndProperty", XMLNamespaceFactory.CS), typeof (EndProperty));
      TypeDictionary.Add(XName.Get("TComplexProperty", XMLNamespaceFactory.CS), typeof (ComplexProperty));
      TypeDictionary.Add(XName.Get("TComplexTypeMapping", XMLNamespaceFactory.CS), typeof (ComplexTypeMapping));
      TypeDictionary.Add(XName.Get("TScalarProperty", XMLNamespaceFactory.CS), typeof (ScalarProperty));
    }

    private static void BuildElementDictionary()
    {
      ElementDictionary.Add(XName.Get("EntityContainer", XMLNamespaceFactory.SSDL), typeof(EntityContainer));
      ElementDictionary.Add(XName.Get("EntityContainer", XMLNamespaceFactory.Edm), typeof(Conceptual.EntityContainer));
      ElementDictionary.Add(XName.Get("Edmx", XMLNamespaceFactory.Edmx), typeof(Edmx));
      ElementDictionary.Add(XName.Get("Schema", XMLNamespaceFactory.SSDL), typeof(StorageSchema));
      ElementDictionary.Add(XName.Get("Schema", XMLNamespaceFactory.Edm), typeof(Conceptual.ConceptualSchema));
      ElementDictionary.Add(XName.Get("Mapping", XMLNamespaceFactory.CS), typeof(Mapping));
    }

    private static void BuildWrapperDictionary()
    {
      WrapperDictionary.Add(typeof (Edmx), typeof (TEdmx));
      WrapperDictionary.Add(typeof (StorageSchema), typeof (TSchema));
      WrapperDictionary.Add(typeof (Conceptual.ConceptualSchema), typeof (Conceptual.TSchema));
      WrapperDictionary.Add(typeof (Mapping), typeof (TMapping));
    }

    protected internal static void AddSchemas(XmlSchemaSet schemas)
    {
      schemas.Add(_schemaSet);
    }

    public static Type GetRootType()
    {
      return ElementDictionary[XName.Get("EntityContainer", XMLNamespaceFactory.SSDL)];
    }
  }
}