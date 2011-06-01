using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateCommandEntity : EntityBase<XDocument>
    {
        public const string ModelSuffix = "Result";

        internal readonly string XmlNamespace;

        public bool IsAssociated { get; private set; }

        public string AssociatedEntityName { get; private set; }

        public NHibernateCommandEntity(XDocument doc, string fileName)
            : base(doc)
        {
            // ReSharper disable PossibleNullReferenceException
            XmlNamespace = EntitySource.Root.Name.NamespaceName;

            Namespace = EntitySource.Root
                .Attribute("namespace")
                .Value; 

            Name = EntitySource.Root
                .Descendant("sql-query", XmlNamespace)
                .Attribute("name")
                .Value;

            EntityKeyName = fileName.Replace(NHibernateUtilities.MapExtension, String.Empty);

            var association = EntitySource.Root.Descendant("return", XmlNamespace);
            if (association != null)
            {
                IsAssociated = true;
                AssociatedEntityName = association.Attribute("class").Value;
            }

            // ReSharper restore PossibleNullReferenceException
        }

        protected override void AddSearchCriteria()
        {
            var searchCriteria = new SearchCriteria(SearchCriteriaType.CustomCommand);

            var queryParams = EntitySource.Root.Descendants("query-param", XmlNamespace);
            foreach (var queryParam in queryParams)
            {
                var property = new NHibernateCommandProperty(queryParam, this);
                searchCriteria.Properties.Add(property);
            }

            SearchCriteria.Add(searchCriteria);
        }

        public override void Initialize()
        {
            AddSearchCriteria();
            LoadProperties();
        }

        protected override void LoadAssociations()
        {
        }

        protected override void LoadExtendedProperties()
        {
        }

        protected override void LoadKeys()
        {
        }

        protected override void LoadProperties()
        {
            var queryParams = EntitySource.Root.Descendants("return-scalar", XmlNamespace);
            foreach (var queryParam in queryParams)
            {
                var property = new NHibernateCommandProperty(queryParam, this);
                PropertyMap.Add(property.Name, property);
            }
        }

        public string GetModelName()
        {
            return IsAssociated
                ? AssociatedEntityName
                : (EntityKeyName + ModelSuffix);
        }
    }
}
