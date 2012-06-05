using System.IO;
using System.Xml;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  public class XRootNamespace
  {
    private XDocument _doc;

    private XTypedElement _rootObject;

    private XRootNamespace()
    {
    }

    public XRootNamespace(Edmx root)
    {
      _doc = new XDocument(root.Untyped);
      _rootObject = root;
    }

    public Edmx Edmx
    {
      get
      {
        return _rootObject as Edmx;
      }
    }

    public XDocument XDocument
    {
      get
      {
        return _doc;
      }
    }

    public static XRootNamespace Load(string xmlFile)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Load(xmlFile)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRootNamespace Load(string xmlFile, LoadOptions options)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Load(xmlFile, options)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRootNamespace Load(TextReader textReader)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Load(textReader)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRootNamespace Load(TextReader textReader, LoadOptions options)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Load(textReader, options)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRootNamespace Load(XmlReader xmlReader)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Load(xmlReader)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRootNamespace Parse(string text)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Parse(text)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public static XRootNamespace Parse(string text, LoadOptions options)
    {
      var root = new XRootNamespace {
                                      _doc = XDocument.Parse(text, options)
                                    };
      var typedRoot = XTypedServices.ToXTypedElement(root._doc.Root, LinqToXsdTypeManager.Instance);
      if ((typedRoot == null))
      {
        throw new LinqToXsdException("Invalid root element in xml document.");
      }
      root._rootObject = typedRoot;
      return root;
    }

    public virtual void Save(string fileName)
    {
      _doc.Save(fileName);
    }

    public virtual void Save(TextWriter textWriter)
    {
      _doc.Save(textWriter);
    }

    public virtual void Save(XmlWriter writer)
    {
      _doc.Save(writer);
    }

    public virtual void Save(TextWriter textWriter, SaveOptions options)
    {
      _doc.Save(textWriter, options);
    }

    public virtual void Save(string fileName, SaveOptions options)
    {
      _doc.Save(fileName, options);
    }
  }
}