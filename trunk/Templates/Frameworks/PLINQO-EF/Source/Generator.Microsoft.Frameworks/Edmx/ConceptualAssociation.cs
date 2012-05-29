using System;
using System.Collections.Generic;
using System.Linq;
using LinqToEdmx.Model.Conceptual;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class ConceptualAssociation : AssociationBase<Association> {

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
        public ConceptualAssociation(Association source, AssociationType associationType, IEntity entity, IEntity foreignEntity, bool isParentEntity, string @namespace = null, IAssociation intermediaryAssociation = null)
            : base(source, associationType, entity, foreignEntity, isParentEntity, @namespace, intermediaryAssociation) {
        }

        /// <summary>
        /// Do any Post constructor initialization here.
        /// By default, this does nothing.
        /// </summary>
        public override void Initialize() {
            AssociationKeyName = AssociationSource.Name;
        }

        /// <summary>
        /// Override to populate the properties from the implemented association.
        /// </summary>
        protected override void LoadProperties() {
            if (AssociationSource.ReferentialConstraint == null)
                return;

            IList<PropertyRef> properties = IsParentEntity ? AssociationSource.ReferentialConstraint.Principal.PropertyRefs : AssociationSource.ReferentialConstraint.Dependent.PropertyRefs;
            IList<PropertyRef> otherProperties = IsParentEntity ? AssociationSource.ReferentialConstraint.Dependent.PropertyRefs : AssociationSource.ReferentialConstraint.Principal.PropertyRefs;

            for (int index = 0; index < properties.Count; index++) {
                AddAssociationProperty(
                    Entity.Properties.FirstOrDefault(p => String.Equals(p.KeyName, properties[index].Name, StringComparison.OrdinalIgnoreCase)), 
                    ForeignEntity.Properties.FirstOrDefault(p => String.Equals(p.KeyName, otherProperties[index].Name, StringComparison.OrdinalIgnoreCase)));
            }
        }

        /// <summary>
        /// Override to populate the extended properties from the implemented association.
        /// </summary>
        protected override void LoadExtendedProperties() {}
    }
}
