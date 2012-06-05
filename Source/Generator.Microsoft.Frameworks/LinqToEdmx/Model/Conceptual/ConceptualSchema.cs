using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  public class ConceptualSchema : XTypedElement, IXMetaData
  {
    private TSchema _contentField;

    public ConceptualSchema()
    {
      SetInnerType(new TSchema());
    }

    public ConceptualSchema(TSchema content)
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<Using> @Usings
    {
      get
      {
        return _contentField.@Usings;
      }
      set
      {
        _contentField.@Usings = value;
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<ComplexType> ComplexTypes
    {
      get
      {
        return _contentField.ComplexTypes;
      }
      set
      {
        _contentField.ComplexTypes = value;
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<EntityType> EntityTypes
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
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
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
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
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
        return XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm");
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

    public static explicit operator ConceptualSchema(XElement xe)
    {
      return XTypedServices.ToXTypedElement<ConceptualSchema, TSchema>(xe, LinqToXsdTypeManager.Instance);
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

    public static ConceptualSchema Load(string xmlFile)
    {
      return XTypedServices.Load<ConceptualSchema, TSchema>(xmlFile, LinqToXsdTypeManager.Instance);
    }

    public static ConceptualSchema Load(Stream xmlStream)
    {
      using (xmlStream)
      {
        return XTypedServices.Load<ConceptualSchema, TSchema>(xmlStream, LinqToXsdTypeManager.Instance);
      }
    }

    public static ConceptualSchema Parse(string xml)
    {
      return XTypedServices.Parse<ConceptualSchema, TSchema>(xml, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return new ConceptualSchema(((TSchema) (Content.Clone())));
    }

    private void SetInnerType(TSchema contentField)
    {
      _contentField = ((TSchema) (XTypedServices.GetCloneIfRooted(contentField)));
      XTypedServices.SetName(this, _contentField);
    }
  }
}