using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using LinqToEdmx.Common;
using LinqToEdmx.Designer;
using LinqToEdmx.Map;
using LinqToEdmx.Model.Conceptual;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx
{
  public class Edmx : XTypedElement, IXMetaData
  {
    private static readonly string ConceptualModelNamespace = typeof(ConceptualSchema).Namespace;
    private static readonly string StorageModelNamespace = typeof(StorageSchema).Namespace;
    private static readonly string MappingNamespace = typeof(Mapping).Namespace;
    private const string ResourceSchemeString = @"res://";

    /// <summary>
    /// Initializes a new instance of the <see cref="Edmx"/> class.
    /// </summary>
    public Edmx()
    {
      SetInnerType(new TEdmx());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Edmx"/> class.
    /// </summary>
    /// <param name="content">The content.</param>
    public Edmx(TEdmx content)
    {
      SetInnerType(content);
    }

    /// <summary>
    /// Gets all items of type <typeparamref name="T"/> from the data model.
    /// </summary>
    /// <typeparam name="T">The type of items to return.</typeparam>
    /// <returns>
    /// A sequence that contains all items of type <typeparamref name="T"/> from the data model.
    /// </returns>
    public IEnumerable<T> GetItems<T>()
      where T : XTypedElement, new()
    {
      // Constrain the search by namespace to improve performance
      var typeNamespace = typeof (T).Namespace;

      if(typeNamespace == ConceptualModelNamespace)
      {
        return Runtimes.First().ConceptualModels.Query.Descendants<T>();
      }
      if (typeNamespace == StorageModelNamespace)
      {
        return Runtimes.First().StorageModels.Query.Descendants<T>();
      }
      if (typeNamespace == MappingNamespace)
      {
        return Runtimes.First().Mappings.Query.Descendants<T>();
      }
      return Runtimes.First().Query.Descendants<T>();
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
        Content.Untyped = value;
      }
    }

    private TEdmx Content
    {
      get;
      set;
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
    /// </para>
    /// </summary>
    public IList<Designer.Designer> Designers
    {
      get
      {
        return Content.Designers;
      }
      set
      {
        Content.Designers = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
    /// </para>
    /// </summary>
    public IList<Runtime> Runtimes
    {
      get
      {
        return Content.Runtimes;
      }
      set
      {
        Content.Runtimes = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
    /// </para>
    /// </summary>
    public IList<DataServices> DataServices
    {
      get
      {
        return Content.DataServices;
      }
      set
      {
        Content.DataServices = value;
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Version
    {
      get
      {
        return Content.Version;
      }
      set
      {
        Content.Version = value;
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
        return XName.Get("Edmx", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator Edmx(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Edmx, TEdmx>(xe, LinqToXsdTypeManager.Instance);
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

    /// <summary>
    /// Loads metadata and mapping information from the given path.
    /// </summary>
    /// <param name="path">
    /// A pipe-delimited list of directories, files, and resource locations in which to look for metadata and mapping information.
    /// Blank spaces on each side of the pipe separator are ignored.
    /// </param>
    /// <returns>An <see cref="Edmx"/> instance loaded with metadata from the <paramref name="path"/>.</returns>
    /// <remarks>
    /// The path to the workspace metadata follows the same rules as paths for embedded resources in the Entity Data Model connection string. 
    /// See <see cref="http://msdn.microsoft.com/en-us/library/cc716756(VS.100).aspx">Connection Strings (Entity Framework)</see> for more information.
    /// </remarks>
    public static Edmx Load(string path)
    {
      return SplitPath(path).Aggregate(BuildEmptyEdmx(), Load);
    }

    private static IEnumerable<string> SplitPath(string path)
    {
      return path
        .Split(new []{'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(splitPath => splitPath.Trim());
    }

    /// <summary>
    /// Loads metadata and mapping information from the given path into an <see cref="Edmx"/> instance.
    /// </summary>
    /// <param name="edmx">The <see cref="Edmx"/> instance in which to load metadata from the given path.</param>
    /// <param name="splitPath">An non-delimited, i.e. split, path from which to load metadata.</param>
    /// <returns>An edmx instance loaded with metadata from the given <paramref name="splitPath"/>.</returns>
    private static Edmx Load(Edmx edmx, string splitPath)
    {
      if (splitPath.StartsWith(ResourceSchemeString))
      {
        return LoadFromEmbeddedResource(edmx, splitPath);
      }

      if (MapToMetadataFileType(splitPath) == MetadataFileType.ConceptualModel)
      {
        edmx.Runtimes.First().ConceptualModels.ConceptualSchema = ConceptualSchema.Load(splitPath);
        return edmx;
      }
      if (MapToMetadataFileType(splitPath) == MetadataFileType.StorageModel)
      {
        edmx.Runtimes.First().StorageModels.StorageSchema = StorageSchema.Load(splitPath);
        return edmx;
      }
      if (MapToMetadataFileType(splitPath) == MetadataFileType.Mapping)
      {
        edmx.Runtimes.First().Mappings.Mapping = Mapping.Load(splitPath);
        return edmx;
      }
      if (MapToMetadataFileType(splitPath) == MetadataFileType.Edmx)
      {
        return XTypedServices.Load<Edmx, TEdmx>(splitPath, LinqToXsdTypeManager.Instance);
      }
      throw new ArgumentException(String.Format("The path argument '{0}' must represent a path to an edmx, csdl, ssdl or msl file.", splitPath));
    }

    private static Edmx LoadFromEmbeddedResource(Edmx edmx, string splitPath)
    {
      return GetStreamsFromResourcePath(splitPath)
        .Aggregate(edmx, (edmxResult, metadataFileTypeAndStream) => Load(edmxResult, metadataFileTypeAndStream.Key, metadataFileTypeAndStream.Value));
    }

    private static Edmx Load(Edmx edmx, MetadataFileType fileType, Stream metadataStream)
    {
      switch (fileType)
      {
        case MetadataFileType.ConceptualModel:
          edmx.Runtimes.First().ConceptualModels.ConceptualSchema = ConceptualSchema.Load(metadataStream);
          return edmx;
        case MetadataFileType.StorageModel:
          edmx.Runtimes.First().StorageModels.StorageSchema = StorageSchema.Load(metadataStream);
          return edmx;
        case MetadataFileType.Mapping:
          edmx.Runtimes.First().Mappings.Mapping = Mapping.Load(metadataStream);
          return edmx;
        case MetadataFileType.Edmx:
          return Load(metadataStream);
        default:
          throw new ArgumentException(String.Format("The fileType '{0}' must represent an edmx, csdl, ssdl or msl file.", fileType));
      }
    }

    public static Edmx Load(Stream xmlStream)
    {
      using (xmlStream)
      {
        return XTypedServices.Load<Edmx, TEdmx>(xmlStream, LinqToXsdTypeManager.Instance);
      }
    }

    private static MetadataFileType? MapToMetadataFileType(string filePath)
    {
      switch (Path.GetExtension(filePath))
      {
        case MetadataFileExtensions.ConceptualModel:
          return MetadataFileType.ConceptualModel;
        case MetadataFileExtensions.StorageModel:
          return MetadataFileType.StorageModel;
        case MetadataFileExtensions.Mapping:
          return MetadataFileType.Mapping;
        case MetadataFileExtensions.Edmx:
          return MetadataFileType.Edmx;
        default:
          return null;
      }
    }

    /// <summary>
    /// Specifies the set of file name extensions for files that contain Entity Data Model metadata.
    /// </summary>
    private struct MetadataFileExtensions
    {
      public const string ConceptualModel = ".csdl";
      public const string StorageModel = ".ssdl";
      public const string Mapping = ".msl";
      public const string Edmx = ".edmx";
    }

    /// <summary>
    /// Specifies the types of files that contain Entity Data Model metadata.
    /// </summary>
    private enum MetadataFileType
    {
      ConceptualModel,
      StorageModel,
      Mapping,
      Edmx
    }

    private static Dictionary<MetadataFileType, Stream> GetStreamsFromResourcePath(string embeddedResourcePath)
    {
      var fileTypesToGet = GetFileTypesFromPath(embeddedResourcePath);
      return GetAssembliesToSearch(embeddedResourcePath)
             .Select(assembly => GetStreamsFromAssembly(assembly, fileTypesToGet))
             .FirstOrDefault(metadataStream => metadataStream.Keys.Contains(fileTypesToGet));
    }

    private static IEnumerable<MetadataFileType> GetFileTypesFromPath(string embeddedResourcePath)
    {
      var resourceName = embeddedResourcePath
        .Replace(ResourceSchemeString, String.Empty)
        .Split('/')
        .ElementAtOrDefault(1);

      if (resourceName == String.Empty)
      {
        yield return MetadataFileType.ConceptualModel;
        yield return MetadataFileType.StorageModel;
        yield return MetadataFileType.Mapping;
      }
      else
      {
        yield return MapToMetadataFileType(resourceName).Value;
      }
    }

    private static IEnumerable<Assembly> GetAssembliesToSearch(string embeddedResourcePath)
    {
      return SpecifiesAssembly(embeddedResourcePath)
               ? GetSpecifiedAssembly(embeddedResourcePath).AsEnumerable()
               : GetWildcardSearchAssemblies();
    }

    private static IEnumerable<Assembly> GetWildcardSearchAssemblies()
    {
        // Search the calling assembly
      return Assembly.GetCallingAssembly()
        // then search referenced assemblies
        .Concat(Assembly.GetCallingAssembly().GetReferencedAssemblies().Select(assemblyName => Assembly.ReflectionOnlyLoad(assemblyName.FullName)))
        // then search assemblies in the bin directory of the application
        .Concat(from filePath in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                where (Path.GetExtension(filePath) == ".dll") || (Path.GetExtension(filePath) == ".exe")
                select Assembly.ReflectionOnlyLoadFrom(filePath));
    }

    private static Assembly GetSpecifiedAssembly(string embeddedResourcePath)
    {
      var assemblyFullName = GetAssemblyFullName(embeddedResourcePath);
      return assemblyFullName != null
               ? Assembly.ReflectionOnlyLoad(assemblyFullName)
               : null;
    }

    private static bool SpecifiesAssembly(string embeddedResourcePath)
    {
      return GetAssemblyFullName(embeddedResourcePath) != null;
    }

    private static string GetAssemblyFullName(string embeddedResourcePath)
    {
      var pathWithoutScheme = embeddedResourcePath.Replace(ResourceSchemeString, String.Empty);
      return pathWithoutScheme.StartsWith("*")
               ? null
               : pathWithoutScheme.Split('/').First();
    }

    private static Dictionary<MetadataFileType, Stream> GetStreamsFromAssembly(Assembly assembly, IEnumerable<MetadataFileType> fileTypesToGet)
    {
      return (from resourceName in assembly.GetManifestResourceNames()
              let metadataFileType = MapToMetadataFileType(resourceName)
              where metadataFileType != null && fileTypesToGet.Contains(metadataFileType.Value)
              select new {
                           MetadataFileType = metadataFileType.Value,
                           Stream = assembly.GetManifestResourceStream(resourceName)
                         }).ToDictionary(pair => pair.MetadataFileType, pair => pair.Stream);
    }

    /// <summary>
    /// Builds an empty <see cref="Edmx"/> instance in which to load metadata content.
    /// </summary>
    /// <returns></returns>
    private static Edmx BuildEmptyEdmx()
    {
      var runtime = new Runtime {
                                  ConceptualModels = new ConceptualModels(),
                                  StorageModels = new StorageModels(),
                                  Mappings = new Mappings()
                                };

      var tedmx = new TEdmx {
                              Runtimes = new List<Runtime> {
                                                             runtime
                                                           }
                            };

      return new Edmx(tedmx);
    }

    public static Edmx Parse(string xml)
    {
      return XTypedServices.Parse<Edmx, TEdmx>(xml, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return new Edmx(((TEdmx) (Content.Clone())));
    }

    private void SetInnerType(TEdmx contentField)
    {
      Content = ((TEdmx) (XTypedServices.GetCloneIfRooted(contentField)));
      XTypedServices.SetName(this, Content);
    }
  }
}