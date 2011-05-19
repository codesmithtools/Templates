using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  /// <summary>
  /// <para>
  /// Regular expression: (DeleteFunction?, InsertFunction?, UpdateFunction?)
  /// </para>
  /// </summary>
  public class EntityTypeModificationFunctionMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    static EntityTypeModificationFunctionMapping()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (DeleteFunction?, InsertFunction?, UpdateFunction?)
    /// </para>
    /// </summary>
    public EntityTypeModificationFunction DeleteFunction
    {
      get
      {
        return ((EntityTypeModificationFunction) GetElement(XName.Get("DeleteFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("DeleteFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (DeleteFunction?, InsertFunction?, UpdateFunction?)
    /// </para>
    /// </summary>
    public EntityTypeModificationFunctionWithResult InsertFunction
    {
      get
      {
        return ((EntityTypeModificationFunctionWithResult) GetElement(XName.Get("InsertFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("InsertFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (DeleteFunction?, InsertFunction?, UpdateFunction?)
    /// </para>
    /// </summary>
    public EntityTypeModificationFunctionWithResult UpdateFunction
    {
      get
      {
        return ((EntityTypeModificationFunctionWithResult) GetElement(XName.Get("UpdateFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("UpdateFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
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
        return XName.Get("TEntityTypeModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator EntityTypeModificationFunctionMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityTypeModificationFunctionMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("DeleteFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeModificationFunction));
      LocalElementDictionary.Add(XName.Get("InsertFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeModificationFunctionWithResult));
      LocalElementDictionary.Add(XName.Get("UpdateFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeModificationFunctionWithResult));
    }
  }
}