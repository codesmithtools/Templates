using System;
using System.Linq;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateEntity : EntityBase<XDocument>
    {
        public readonly static string[] DefaultAttributes = new[] { "name", "table", "lazy", "MS_DiagramPane1", "MS_DiagramPane2", "MS_DiagramPaneCount" };

        internal readonly string XmlNamespace;

        private readonly string _safeName;

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

            _safeName = table;
            // ReSharper restore PossibleNullReferenceException
        }

        public override void Initialize()
        {
            // ReSharper disable PossibleNullReferenceException
            var @class = EntitySource.Root.Descendant("class", XmlNamespace);
            var lazy = @class.Attribute("lazy");

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
                if (!PropertyMap.ContainsKey(property.Name))
                    PropertyMap.Add(property.Name, property);
            }

            if (PropertyMap.Values.Count(em => em.IsType(PropertyType.Concurrency)) > 1)
                throw new Exception(String.Format("More than one Concurrency property in {0}", EntityKeyName));
        }

        protected override void LoadSearchCriteria()
        {
        }

        protected override void LoadAssociations()
        {
            foreach (var associate in EntitySource.Root.Descendants("many-to-one", XmlNamespace)) {
                var association = NHibernateAssociation.FromElement(this, associate, associate, AssociationType.ManyToOne, XmlNamespace);
                if (association != null && !AssociationMap.ContainsKey(association.Name))
                    AssociationMap.Add(association.Name, association);
            }

            var bags = EntitySource.Root.Descendants("bag", XmlNamespace);
            foreach (var bag in bags) {
                var oneToMany = bag.Descendant("one-to-many", XmlNamespace);
                var association = NHibernateAssociation.FromElement(this, bag, oneToMany, AssociationType.OneToMany, XmlNamespace);
                if (association != null && !AssociationMap.ContainsKey(association.Name))
                    AssociationMap.Add(association.Name, association);

                var manyToMany = bag.Descendant("many-to-many", XmlNamespace);
                association = NHibernateAssociation.FromElement(this, bag, manyToMany, AssociationType.ManyToMany, XmlNamespace);
                if (association != null && !AssociationMap.ContainsKey(association.Name))
                    AssociationMap.Add(association.Name, association);
            }
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
            foreach(var key in keys) {
                var property = new NHibernateProperty(key, this);
                Key.Properties.Add(property);
            }

            var foriegnKeys = compositeKey.Descendants("key-many-to-one", XmlNamespace);
            foreach (var foriegnKey in foriegnKeys) {
                var association = NHibernateAssociation.FromElement(this, foriegnKey, foriegnKey, AssociationType.ManyToOne, XmlNamespace);
                if (association != null && !AssociationMap.ContainsKey(association.Name))
                    Key.Associations.Add(association);
            }
        }

        private void LoadKey() {
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