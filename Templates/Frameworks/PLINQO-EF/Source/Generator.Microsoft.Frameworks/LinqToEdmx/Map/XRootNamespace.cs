using System.IO;
using System.Xml;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  public class XRootNamespace
  {
    private XDocument _xDocument;

    private XTypedElement _rootObject;

    private XRootNamespace()
    {
    }

    public XRootNamespace(Mapping root)
    {
      _xDocument = new XDocument(root.Untyped);
      _rootObject = root;
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

    public static XRootNamespace Load(string xmlFile)
    {
      var root = new XRootNamespace
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

    public static XRootNamespace Load(string xmlFile, LoadOptions options)
    {
      var root = new XRootNamespace
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

    public static XRootNamespace Load(TextReader textReader)
    {
      var root = new XRootNamespace
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

    public static XRootNamespace Load(TextReader textReader, LoadOptions options)
    {
      var root = new XRootNamespace
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

    public static XRootNamespace Load(XmlReader xmlReader)
    {
      var root = new XRootNamespace
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

    public static XRootNamespace Parse(string text)
    {
      var root = new XRootNamespace
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

    public static XRootNamespace Parse(string text, LoadOptions options)
    {
      var root = new XRootNamespace
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