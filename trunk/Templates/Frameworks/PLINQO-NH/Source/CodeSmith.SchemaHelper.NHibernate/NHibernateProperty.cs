using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateProperty : PropertyBase<XElement>
    {
        public static readonly string[] DefaultAttributes = new[] { "name", "column", "type", NHibernateUtilities.NHibernateType, NHibernateUtilities.GeneratorClass };

        private string _safeName;

        public NHibernateProperty(XElement element, IEntity entity)
            : base(element, entity)
        {
        }

        public override void Initialize()
        {
            // ReSharper disable PossibleNullReferenceException
            var name = PropertySource.Attribute("name").Value;
            var type = PropertySource.Attribute("type").Value;
            var column = PropertySource.Attribute("column").Value;
            
            var lengthAttribute = PropertySource.Attribute("length");
            int? length = null;
            if (lengthAttribute != null)
            {
                int lengthInt;
                if (Int32.TryParse(lengthAttribute.Value, out lengthInt))
                    length = lengthInt;
            }

            KeyName = name;
            Name = name;
            GetterAccess = "public";
            SetterAccess = "public";
            SystemType = FromNHibernateType(type, length);

            _safeName = column;

            ExtendedProperties.Add(NHibernateUtilities.NHibernateType, type);

            var generator = PropertySource.Descendant("generator", ((NHibernateEntity)Entity).XmlNamespace);
            if (generator != null)
                ExtendedProperties.Add(NHibernateUtilities.GeneratorClass, generator.Attribute("class").Value);

            var customAttributes = PropertySource
                .Attributes()
                .Where(a => !DefaultAttributes.Contains(a.Name.ToString()));
            foreach (var customAttribute in customAttributes)
            {
                // Special case, always preserve length from DB.
                if (customAttribute.Name.ToString() == "length")
                    continue;

                ExtendedProperties.Add(customAttribute.Name.ToString(), customAttribute.Value);
            }
            // ReSharper restore PossibleNullReferenceException
        }

        public string FromNHibernateType(string nhibernateType, int? length)
        {
            var systemType = FromNHibernateTypeMap.ContainsKey(nhibernateType)
                ? FromNHibernateTypeMap[nhibernateType]
                : "System.String";

            if (length.HasValue && length > 1)
            {
                if (systemType == "System.Char")
                    return "System.String";
            }

            return systemType;
        }

        public override string GetSafeName()
        {
            return _safeName;
        }

        private static MapCollection _fromNHibernateTypeMap;

        private static MapCollection FromNHibernateTypeMap
        {
            get
            {
                if (_fromNHibernateTypeMap == null)
                {
                    string path;
                    if (!Map.TryResolvePath("NHibernateToSystemType", String.Empty, out path))
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        var baseDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
                        Map.TryResolvePath("NHibernateToSystemType", baseDirectory, out path);
                    }

                    _fromNHibernateTypeMap = Map.Load(path);
                }

                return _fromNHibernateTypeMap;
            }
        }
    }
}