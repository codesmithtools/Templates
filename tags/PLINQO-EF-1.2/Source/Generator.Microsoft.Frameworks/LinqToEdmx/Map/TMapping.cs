using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  /// <summary>
  /// <para>
  /// Regular expression: (Alias*, EntityContainerMapping)
  /// </para>
  /// </summary>
  public class TMapping : XTypedElement, IXMetaData
  {
    private const string SpaceFixedValue = "C-S";

    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<Alias> _aliasField;

    static TMapping()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("Alias", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")), new NamedContentModelEntity(XName.Get("EntityContainerMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (Alias*, EntityContainerMapping)
    /// </para>
    /// </summary>
    public IList<Alias> Aliases
    {
      get
      {
        if ((_aliasField == null))
        {
          _aliasField = new XTypedList<Alias>(this, LinqToXsdTypeManager.Instance, XName.Get("Alias", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _aliasField;
      }
      set
      {
        if ((value == null))
        {
          _aliasField = null;
        }
        else
        {
          if ((_aliasField == null))
          {
            _aliasField = XTypedList<Alias>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Alias", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_aliasField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// <para>
    /// Regular expression: (Alias*, EntityContainerMapping)
    /// </para>
    /// </summary>
    public EntityContainerMapping EntityContainerMapping
    {
      get
      {
        return ((EntityContainerMapping) GetElement(XName.Get("EntityContainerMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("EntityContainerMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Space
    {
      get
      {
        return SpaceFixedValue;
      }
      set
      {
        if (value.Equals(SpaceFixedValue))
        {
        }
        else
        {
          throw new LinqToXsdFixedValueException(value, SpaceFixedValue);
        }
        SetAttribute(XName.Get("Space", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
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
        return XName.Get("TMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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
      return ContentModel;
    }

    #endregion

    public static explicit operator TMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<TMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Alias", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Alias));
      LocalElementDictionary.Add(XName.Get("EntityContainerMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityContainerMapping));
    }
  }
}