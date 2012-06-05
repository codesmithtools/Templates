using System;
using System.Linq;
using System.Xml.Linq;
using CodeSmith.SchemaHelper.NHibernate;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class NHibernateAssociation : AssociationBase<XElement> {
        private readonly string _xmlNamespace;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="associationType"></param>
        /// <param name="entity"></param>
        /// <param name="foreignEntity"></param>
        /// <param name="isParentEntity"></param>
        /// <param name="namespace"></param>
        /// <param name="intermediaryAssociation"></param>
        /// <param name="xmlNamespace"> </param>
        public NHibernateAssociation(XElement source, AssociationType associationType, IEntity entity, IEntity foreignEntity, bool isParentEntity, string @namespace = null, IAssociation intermediaryAssociation = null, string xmlNamespace = null)
            : base(source, associationType, entity, foreignEntity, isParentEntity, @namespace, intermediaryAssociation) {
            _xmlNamespace = xmlNamespace;
        }

        /// <summary>
        /// Do any Post constructor initialization here.
        /// By default, this does nothing.
        /// </summary>
        public override void Initialize() {
            AssociationKeyName = Name = AssociationSource.Attribute("name").Value;
        }

        /// <summary>
        /// Override to populate the properties from the implemented association.
        /// </summary>
        protected override void LoadProperties() {
            switch (AssociationType) {
                case AssociationType.ManyToMany:
                case AssociationType.OneToMany:
                case AssociationType.ZeroOrOneToMany:
                    var key = AssociationSource.Descendant("key", _xmlNamespace);
                    foreach (var column in key.Descendants("column", _xmlNamespace)) {
                        var associationProperty = new NHibernateAssociationProperty(column, Entity);
                        AddAssociationProperty(null, associationProperty);
                    }
                    break;

                case AssociationType.ManyToOne:
                    foreach (var column in AssociationSource.Descendants("column", _xmlNamespace)) {
                        var associationProperty = new NHibernateAssociationProperty(column, Entity);
                        AddAssociationProperty(associationProperty, null);
                    }
                    break;
            }
        }

        /// <summary>
        /// Override to populate the extended properties from the implemented association.
        /// </summary>
        protected override void LoadExtendedProperties() {
            var customAttributes = AssociationSource.Attributes().Where(a => !NHibernateUtilities.AssociationDefaultAttributes.Contains(a.Name.ToString()));
            foreach (var customAttribute in customAttributes)
                ExtendedProperties.Add(customAttribute.Name.ToString(), customAttribute.Value);
        }

        public static NHibernateAssociation FromElement(IEntity source, XElement nameElement, XElement typeElement, AssociationType type, string xmlNamespace) {
            if (nameElement == null || typeElement == null)
                return null;

            var associationName = nameElement.Attribute("name");
            var associationType = typeElement.Attribute("class");
            if (associationName == null || associationType == null)
                return null;

            if (Configuration.Instance.ExcludeRegexIsMatch(associationName.Value))
                return null;

            var associationEntity = EntityStore.Instance.GetEntity(associationType.Value);
            if (associationEntity == null)
                return null;

            var isParentEntity = type == AssociationType.ZeroOrOneToMany || type == AssociationType.OneToMany || type == AssociationType.ManyToMany;
            return new NHibernateAssociation(nameElement, type, source, associationEntity, isParentEntity, xmlNamespace: xmlNamespace);
        }
    }
}
