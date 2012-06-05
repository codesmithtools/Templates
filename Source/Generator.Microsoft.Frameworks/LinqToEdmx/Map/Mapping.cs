using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  public class Mapping : XTypedElement, IXMetaData
  {
    private TMapping _contentField;

    public Mapping()
    {
      SetInnerType(new TMapping());
    }

    public Mapping(TMapping content)
    {
      SetInnerType(content);
    }

    public override XElement Untyped
    {
      get
      {
        return base.Untyped;
      }
      set
      {
        base.Untyped = value;
        _contentField.Untyped = value;
      }
    }

    public TMapping Content
    {
      get
      {
        return _contentField;
      }
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
        return _contentField.Aliases;
      }
      set
      {
        _contentField.Aliases = value;
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
        return _contentField.EntityContainerMapping;
      }
      set
      {
        _contentField.EntityContainerMapping = value;
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
        return _contentField.Space;
      }
      set
      {
        _contentField.Space = value;
      }
    }

    #region IXMetaData Members

    Dictionary<XName, Type> IXMetaData.LocalElementsDictionary
    {
      get
      {
        var schemaMetaData = ((IXMetaData) (Content));
        return schemaMetaData.LocalElementsDictionary;
      }
    }

    XTypedElement IXMetaData.Content
    {
      get
      {
        return Content;
      }
    }

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
      }
    }

    SchemaOrigin IXMetaData.TypeOrigin
    {
      get
      {
        return SchemaOrigin.Element;
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

    public static explicit operator Mapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Mapping, TMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public void Save(string xmlFile)
    {
      XTypedServices.Save(xmlFile, Untyped);
    }

    public void Save(TextWriter tw)
    {
      XTypedServices.Save(tw, Untyped);
    }

    public void Save(XmlWriter xmlWriter)
    {
      XTypedServices.Save(xmlWriter, Untyped);
    }

    public static Mapping Load(string xmlFile)
    {
      return XTypedServices.Load<Mapping, TMapping>(xmlFile, LinqToXsdTypeManager.Instance);
    }

    public static Mapping Load(Stream xmlStream)
    {
      using (xmlStream)
      {
        return XTypedServices.Load<Mapping, TMapping>(xmlStream, LinqToXsdTypeManager.Instance);
      }
    }

    public static Mapping Parse(string xml)
    {
      return XTypedServices.Parse<Mapping, TMapping>(xml, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return new Mapping(((TMapping) (Content.Clone())));
    }

    private void SetInnerType(TMapping contentField)
    {
      _contentField = ((TMapping) (XTypedServices.GetCloneIfRooted(contentField)));
      XTypedServices.SetName(this, _contentField);
    }
  }
}