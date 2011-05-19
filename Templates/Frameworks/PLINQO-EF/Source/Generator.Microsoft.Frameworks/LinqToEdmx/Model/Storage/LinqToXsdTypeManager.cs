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
      TypeDictionary.Add(XName.Get("TEdmx", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (TEdmx));
      TypeDictionary.Add(XName.Get("TDesigner", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Designer.Designer));
      TypeDictionary.Add(XName.Get("TOptions", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Options));
      TypeDictionary.Add(XName.Get("TDiagrams", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Diagrams));
      TypeDictionary.Add(XName.Get("TConnection", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Connection));
      TypeDictionary.Add(XName.Get("TDesignerInfoPropertySet", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (DesignerInfoPropertySet));
      TypeDictionary.Add(XName.Get("TDesignerProperty", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (DesignerProperty));
      TypeDictionary.Add(XName.Get("TDiagram", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Diagram));
      TypeDictionary.Add(XName.Get("TEntityTypeShape", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (EntityTypeShape));
      TypeDictionary.Add(XName.Get("TAssociationConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (AssociationConnector));
      TypeDictionary.Add(XName.Get("TInheritanceConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (InheritanceConnector));
      TypeDictionary.Add(XName.Get("TConnectorPoint", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (ConnectorPoint));
      TypeDictionary.Add(XName.Get("TRuntime", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Runtime));
      TypeDictionary.Add(XName.Get("TRuntimeConceptualModels", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (ConceptualModels));
      TypeDictionary.Add(XName.Get("TRuntimeStorageModels", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (StorageModels));
      TypeDictionary.Add(XName.Get("TRuntimeMappings", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Mappings));
      TypeDictionary.Add(XName.Get("TDataServices", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (DataServices));
      TypeDictionary.Add(XName.Get("TSchema", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (TSchema));
      TypeDictionary.Add(XName.Get("TDocumentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
      TypeDictionary.Add(XName.Get("TText", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Text));
      TypeDictionary.Add(XName.Get("TAssociation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Association));
      TypeDictionary.Add(XName.Get("TConstraint", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Constraint));
      TypeDictionary.Add(XName.Get("TReferentialConstraintRoleElement", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (ReferentialConstraintRoleElement));
      TypeDictionary.Add(XName.Get("TEntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityTypeStore));
      TypeDictionary.Add(XName.Get("TEntityKeyElement", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityKeyElement));
      TypeDictionary.Add(XName.Get("TPropertyRef", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (PropertyRef));
      TypeDictionary.Add(XName.Get("TAssociationEnd", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (AssociationEnd));
      TypeDictionary.Add(XName.Get("TOnAction", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (OnAction));
      TypeDictionary.Add(XName.Get("TEntityProperty", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityProperty));
      TypeDictionary.Add(XName.Get("TFunction", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Function));
      TypeDictionary.Add(XName.Get("TParameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Parameter));
      TypeDictionary.Add(XName.Get("TSchema", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.TSchema));
      TypeDictionary.Add(XName.Get("TDocumentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.Documentation));
      TypeDictionary.Add(XName.Get("TText", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.Text));
      TypeDictionary.Add(XName.Get("TXmlOrText", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (XmlOrText));
      TypeDictionary.Add(XName.Get("TUsing", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Using));
      TypeDictionary.Add(XName.Get("TAssociation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.Association));
      TypeDictionary.Add(XName.Get("TComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (ComplexType));
      TypeDictionary.Add(XName.Get("TConstraint", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.Constraint));
      TypeDictionary.Add(XName.Get("TReferentialConstraintRoleElement", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.ReferentialConstraintRoleElement));
      TypeDictionary.Add(XName.Get("TNavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (NavigationProperty));
      TypeDictionary.Add(XName.Get("TEntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.EntityType));
      TypeDictionary.Add(XName.Get("TFunction", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.Function));
      TypeDictionary.Add(XName.Get("TFunctionParameter", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionParameter));
      TypeDictionary.Add(XName.Get("TCollectionType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (TCollectionType));
      TypeDictionary.Add(XName.Get("TTypeRef", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (TypeRef));
      TypeDictionary.Add(XName.Get("TReferenceType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (ReferenceType));
      TypeDictionary.Add(XName.Get("TRowType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (RowType));
      TypeDictionary.Add(XName.Get("TProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Property));
      TypeDictionary.Add(XName.Get("TFunctionReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionReturnType));
      TypeDictionary.Add(XName.Get("TEntityKeyElement", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.EntityKeyElement));
      TypeDictionary.Add(XName.Get("TPropertyRef", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.PropertyRef));
      TypeDictionary.Add(XName.Get("TAssociationEnd", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.AssociationEnd));
      TypeDictionary.Add(XName.Get("TOnAction", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.OnAction));
      TypeDictionary.Add(XName.Get("TEntityProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.EntityProperty));
      TypeDictionary.Add(XName.Get("TComplexTypeProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (ComplexTypeProperty));
      TypeDictionary.Add(XName.Get("TFunctionImportParameter", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionImportParameter));
      TypeDictionary.Add(XName.Get("TMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (TMapping));
      TypeDictionary.Add(XName.Get("TQueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (QueryView));
      TypeDictionary.Add(XName.Get("TAlias", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Alias));
      TypeDictionary.Add(XName.Get("TEntityContainerMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityContainerMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportMappingResultMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportMappingResultMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportComplexTypeMapping));
      TypeDictionary.Add(XName.Get("TEntitySetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntitySetMapping));
      TypeDictionary.Add(XName.Get("TAssociationSetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (AssociationSetMapping));
      TypeDictionary.Add(XName.Get("TEntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeMapping));
      TypeDictionary.Add(XName.Get("TFunctionImportEntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportEntityTypeMapping));
      TypeDictionary.Add(XName.Get("TMappingFragment", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (MappingFragment));
      TypeDictionary.Add(XName.Get("TEntityTypeModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeModificationFunctionMapping));
      TypeDictionary.Add(XName.Get("TAssociationSetModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (AssociationSetModificationFunctionMapping));
      TypeDictionary.Add(XName.Get("TEntityTypeModificationFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeModificationFunction));
      TypeDictionary.Add(XName.Get("TEntityTypeModificationFunctionWithResult", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeModificationFunctionWithResult));
      TypeDictionary.Add(XName.Get("TAssociationSetModificationFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (AssociationSetModificationFunction));
      TypeDictionary.Add(XName.Get("TFunctionMappingEndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingEndProperty));
      TypeDictionary.Add(XName.Get("TFunctionMappingScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingScalarProperty));
      TypeDictionary.Add(XName.Get("TResultBinding", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ResultBinding));
      TypeDictionary.Add(XName.Get("TFunctionMappingAssociationEnd", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingAssociationEnd));
      TypeDictionary.Add(XName.Get("TFunctionMappingComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingComplexProperty));
      TypeDictionary.Add(XName.Get("TCondition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Condition));
      TypeDictionary.Add(XName.Get("TFunctionImportCondition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportCondition));
      TypeDictionary.Add(XName.Get("TEndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EndProperty));
      TypeDictionary.Add(XName.Get("TComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ComplexProperty));
      TypeDictionary.Add(XName.Get("TComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ComplexTypeMapping));
      TypeDictionary.Add(XName.Get("TScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ScalarProperty));
    }

    private static void BuildElementDictionary()
    {
      ElementDictionary.Add(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityContainer));
      ElementDictionary.Add(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.EntityContainer));
      ElementDictionary.Add(XName.Get("Edmx", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Edmx));
      ElementDictionary.Add(XName.Get("Schema", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (StorageSchema));
      ElementDictionary.Add(XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Conceptual.ConceptualSchema));
      ElementDictionary.Add(XName.Get("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Mapping));
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
      return ElementDictionary[XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl")];
    }
  }
}