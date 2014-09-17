using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  /// <summary>
  /// <para>
  /// Regular expression: ((ScalarProperty? | AssociationEnd? | ComplexProperty?)+, ResultBinding*)
  /// </para>
  /// </summary>
  public class EntityTypeModificationFunctionWithResult : EntityTypeModificationFunction, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<ResultBinding> ResultBindingField;

    static EntityTypeModificationFunctionWithResult()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: ((ScalarProperty? | AssociationEnd? | ComplexProperty?)+, ResultBinding*)
    /// </para>
    /// </summary>
    public IList<ResultBinding> ResultBindings
    {
      get
      {
        if ((ResultBindingField == null))
        {
          ResultBindingField = new XTypedList<ResultBinding>(this, LinqToXsdTypeManager.Instance, XName.Get("ResultBinding", XMLNamespaceFactory.CS));
        }
        return ResultBindingField;
      }
      set
      {
        if ((value == null))
        {
          ResultBindingField = null;
        }
        else
        {
          if ((ResultBindingField == null))
          {
            ResultBindingField = XTypedList<ResultBinding>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ResultBinding", XMLNamespaceFactory.CS));
          }
          else
          {
            XTypedServices.SetList(ResultBindingField, value);
          }
        }
      }
    }

    #region IXMetaData Members

    Dictionary<XName, Type> IXMetaData.LocalElementsDictionary
    {
      get
      {
        return LocalElementDictionary;
      }
    }

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("TEntityTypeModificationFunctionWithResult", XMLNamespaceFactory.CS);
      }
    }

    SchemaOrigin IXMetaData.TypeOrigin
    {
      get
      {
        return SchemaOrigin.Fragment;
      }
    }

    ILinqToXsdTypeManager IXMetaData.TypeManager
    {
      get
      {
        return LinqToXsdTypeManager.Instance;
      }
    }

    ContentModelEntity IXMetaData.GetContentModel()
    {
      return ContentModelEntity.Default;
    }

    #endregion

    public static explicit operator EntityTypeModificationFunctionWithResult(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityTypeModificationFunctionWithResult>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ScalarProperty", XMLNamespaceFactory.CS), typeof (FunctionMappingScalarProperty));
      LocalElementDictionary.Add(XName.Get("AssociationEnd", XMLNamespaceFactory.CS), typeof (FunctionMappingAssociationEnd));
      LocalElementDictionary.Add(XName.Get("ComplexProperty", XMLNamespaceFactory.CS), typeof (FunctionMappingComplexProperty));
      LocalElementDictionary.Add(XName.Get("ResultBinding", XMLNamespaceFactory.CS), typeof (ResultBinding));
    }
  }
}