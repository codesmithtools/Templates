using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  public class StorageSchema : XTypedElement, IXMetaData
  {
    private TSchema _contentField;

    public StorageSchema()
    {
      SetInnerType(new TSchema());
    }

    public StorageSchema(TSchema content)
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

    public TSchema Content
    {
      get
      {
        return _contentField;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<Association> Associations
    {
      get
      {
        return _contentField.Associations;
      }
      set
      {
        _contentField.Associations = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<EntityTypeStore> EntityTypeStores
    {
      get
      {
        return _contentField.EntityTypes;
      }
      set
      {
        _contentField.EntityTypes = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<EntityContainer> EntityContainers
    {
      get
      {
        return _contentField.EntityContainers;
      }
      set
      {
        _contentField.EntityContainers = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<Function> Functions
    {
      get
      {
        return _contentField.Functions;
      }
      set
      {
        _contentField.Functions = value;
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IEnumerable<XElement> Any
    {
      get
      {
        return _contentField.Any;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string @Namespace
    {
      get
      {
        return _contentField.@Namespace;
      }
      set
      {
        _contentField.@Namespace = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Alias
    {
      get
      {
        return _contentField.Alias;
      }
      set
      {
        _contentField.Alias = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Provider
    {
      get
      {
        return _contentField.Provider;
      }
      set
      {
        _contentField.Provider = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string ProviderManifestToken
    {
      get
      {
        return _contentField.ProviderManifestToken;
      }
      set
      {
        _contentField.ProviderManifestToken = value;
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
        return XName.Get("Schema", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

    public static explicit operator StorageSchema(XElement xe)
    {
      return XTypedServices.ToXTypedElement<StorageSchema, TSchema>(xe, LinqToXsdTypeManager.Instance);
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

    public static StorageSchema Load(string xmlFile)
    {
      return XTypedServices.Load<StorageSchema, TSchema>(xmlFile, LinqToXsdTypeManager.Instance);
    }

    public static StorageSchema Load(Stream xmlStream)
    {
      using (xmlStream)
      {
        return XTypedServices.Load<StorageSchema, TSchema>(xmlStream, LinqToXsdTypeManager.Instance);
      }
    }

    public static StorageSchema Parse(string xml)
    {
      return XTypedServices.Parse<StorageSchema, TSchema>(xml, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return new StorageSchema(((TSchema) (Content.Clone())));
    }

    private void SetInnerType(TSchema contentField)
    {
      _contentField = ((TSchema) (XTypedServices.GetCloneIfRooted(contentField)));
      XTypedServices.SetName(this, _contentField);
    }
  }
}