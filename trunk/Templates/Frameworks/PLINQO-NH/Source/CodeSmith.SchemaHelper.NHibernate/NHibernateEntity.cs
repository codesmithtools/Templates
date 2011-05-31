using System;
using System.Linq;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateEntity : EntityBase<XDocument>
    {
        public readonly static string[] DefaultAttributes = new[] { "name", "table", "lazy", "MS_DiagramPane1", "MS_DiagramPane2", "MS_DiagramPaneCount", NHibernateUtilities.FileName };

        internal readonly string XmlNamespace;

        private string _safeName;

        private readonly string _fileName;

        public bool IsView { get; private set; }

        public NHibernateEntity(XDocument doc, string fileName, bool isView = false)
            : base(doc)
        {
            IsView = isView;

            // ReSharper disable PossibleNullReferenceException
            XmlNamespace = EntitySource.Root.Name.NamespaceName;

            var @namespace = EntitySource.Root
                .Attribute("namespace")
                .Value; 
            var name = EntitySource.Root
                .Descendant("class", XmlNamespace)
                .Attribute("name")
                .Value;
            var table = EntitySource.Root
                .Descendant("class", XmlNamespace)
                .Attribute("table")
                .Value;
            
            Namespace = @namespace;
            EntityKeyName = name;
            Name = name;

            _fileName = fileName.Replace(NHibernateUtilities.MapExtension, String.Empty);
            _safeName = table;
            // ReSharper restore PossibleNullReferenceException
        }

        public override void Initialize()
        {
            // ReSharper disable PossibleNullReferenceException
            var @class = EntitySource.Root.Descendant("class", XmlNamespace);
            var lazy = @class.Attribute("lazy");

            ExtendedProperties.Add(NHibernateUtilities.FileName, _fileName);
            if (lazy != null)
                ExtendedProperties.Add(NHibernateUtilities.Lazy, lazy.Value);

            var customAttributes = @class
               .Attributes()
               .Where(a => !DefaultAttributes.Contains(a.Name.ToString()));
            foreach (var customAttribute in customAttributes)
                ExtendedProperties.Add(customAttribute.Name.ToString(), customAttribute.Value);

            LoadProperties();
            LoadKeys();
            LoadAssociations();
            // ReSharper restore PossibleNullReferenceException
        }

        protected override void LoadProperties()
        {
            var properties = EntitySource.Root
                .Descendants("property", XmlNamespace)
                .ToList();

            var version = EntitySource.Root.Descendant("version", XmlNamespace);
            if (version != null)
                properties.Add(version);

            foreach (var prop in properties)
            {
                var propName = prop.Attribute("name");
                if (propName == null)
                    continue;

                if (Configuration.Instance.ExcludeRegexIsMatch(propName.Value) || PropertyMap.ContainsKey(propName.Value))
                    continue;
                
                var property = new NHibernateProperty(prop, this);
                PropertyMap.Add(property.Name, property);
            }

            if (PropertyMap.Values.Where(em => (em.PropertyType & PropertyType.Concurrency) == PropertyType.Concurrency).Count() > 1)
                throw new Exception(String.Format("More than one Concurrency property in {0}", EntityKeyName));
        }
        
        protected override void AddSearchCriteria()
        {
        }

        protected override void LoadAssociations()
        {
            foreach (var associate in EntitySource.Root.Descendants("many-to-one", XmlNamespace))
                LoadAssociation(associate, associate, AssociationType.ManyToOne);

            var bags = EntitySource.Root.Descendants("bag", XmlNamespace);
            foreach (var bag in bags)
            {
                var oneToMany = bag.Descendant("one-to-many", XmlNamespace);
                LoadAssociation(bag, oneToMany, AssociationType.OneToMany);

                var manyToMany = bag.Descendant("many-to-many", XmlNamespace);
                LoadAssociation(bag, manyToMany, AssociationType.ManyToMany);
            }
        }

        private void LoadAssociation(XElement nameElement, XElement typeElement, AssociationType type)
        {
            if (nameElement == null || typeElement == null)
                return;

            var associationName = nameElement.Attribute("name");
            var associationType = typeElement.Attribute("class");
            if (associationName == null || associationType == null)
                return;

            if (Configuration.Instance.ExcludeRegexIsMatch(associationName.Value) || AssociationMap.ContainsKey(associationName.Value))
                return;

            var associationEntity = EntityStore.Instance.GetEntity(associationType.Value);
            if (associationEntity == null)
                return;

            var association = new Association(associationName.Value, type, this, associationEntity, false);
            association.Name = associationName.Value;
            
            switch(type)
            {
                case AssociationType.ManyToMany:
                case AssociationType.OneToMany:
                    var key = nameElement.Descendant("key", XmlNamespace);
                    foreach (var column in key.Descendants("column", XmlNamespace))
                    {
                        var associationProperty = new NHibernateAssociationProperty(column, this);
                        association.AddAssociationProperty(null, associationProperty);
                    }
                    break;

                case AssociationType.ManyToOne:
                    foreach (var column in nameElement.Descendants("column", XmlNamespace))
                    {
                        var associationProperty = new NHibernateAssociationProperty(column, this);
                        association.AddAssociationProperty(associationProperty, null);
                    }
                    break;

                default:
                    break;
            }

            var customAttributes = nameElement
                .Attributes()
                .Where(a => !NHibernateUtilities.AssociationDefaultAttributes.Contains(a.Name.ToString()));
            foreach (var customAttribute in customAttributes)
                association.ExtendedProperties.Add(customAttribute.Name.ToString(), customAttribute.Value);

            AssociationMap.Add(association.Name, association);
        }

        protected override void LoadExtendedProperties()
        {
        }

        protected override void LoadKeys()
        {
            LoadCompositeKey();
            LoadKey();
        }

        private void LoadCompositeKey()
        {
            var compositeKey = EntitySource.Root.Descendant("composite-id", XmlNamespace);
            if (compositeKey == null)
                return;

            var keys = compositeKey.Descendants("key-property", XmlNamespace);
            foreach(var key in keys)
            {
                var property = new NHibernateProperty(key, this);
                Key.Properties.Add(property);
            }
        }

        private void LoadKey()
        {
            var key = EntitySource.Root.Descendant("id", XmlNamespace);
            if (key == null)
                return;

            var property = new NHibernateProperty(key, this);
            Key.Properties.Add(property);
        }

        public override string GetSafeName()
        {
            return _safeName;
        }
    }
}