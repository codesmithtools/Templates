using System.IO;
using System.Xml;
using System.Xml.Linq;
using LinqToEdmx.Map;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  public class XRoot
  {
    private XDocument _xDocument;

    private XTypedElement _rootObject;

    private XRoot()
    {
    }

    public XRoot(EntityContainer root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public XRoot(StorageSchema root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public XRoot(Conceptual.EntityContainer root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public XRoot(Conceptual.ConceptualSchema root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public XRoot(Edmx root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public XRoot(Mapping root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public EntityContainer EntityContainer
    {
      get
      {
        return _rootObject as EntityContainer;
      }
    }

    public StorageSchema StorageSchema
    {
      get
      {
        return _rootObject as StorageSchema;
      }
    }

    public Conceptual.EntityContainer EntityContainer1
    {
      get
      {
        return _rootObject as Conceptual.EntityContainer;
      }
    }

    public Conceptual.ConceptualSchema Schema1
    {
      get
      {
        return _rootObject as Conceptual.ConceptualSchema;
      }
    }

    public Edmx Edmx
    {
      get
      {
        return _rootObject as Edmx;
      }
    }

    public Mapping Mapping
    {
      get
      {
        return _rootObject as Mapping;
      }
    }

    public XDocument XDocument
    {
      get
      {
        return _xDocument;
      }
    }

    public static XRoot Load(string xmlFile)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Load(xmlFile)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRoot Load(string xmlFile, LoadOptions options)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Load(xmlFile, options)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRoot Load(TextReader textReader)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Load(textReader)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRoot Load(TextReader textReader, LoadOptions options)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Load(textReader, options)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRoot Load(XmlReader xmlReader)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Load(xmlReader)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRoot Parse(string text)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Parse(text)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRoot Parse(string text, LoadOptions options)
    {
      var root = new XRoot
                   {
                     _xDocument = XDocument.Parse(text, options)
                   };
      var typedRoot = XTypedServices.ToXTypedElement(root._xDocument.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public virtual void Save(string fileName)
    {
      _xDocument.Save(fileName);
    }

    public virtual void Save(TextWriter textWriter)
    {
      _xDocument.Save(textWriter);
    }

    public virtual void Save(XmlWriter writer)
    {
      _xDocument.Save(writer);
    }

    public virtual void Save(TextWriter textWriter, SaveOptions options)
    {
      _xDocument.Save(textWriter, options);
    }

    public virtual void Save(string fileName, SaveOptions options)
    {
      _xDocument.Save(fileName, options);
    }
  }
}