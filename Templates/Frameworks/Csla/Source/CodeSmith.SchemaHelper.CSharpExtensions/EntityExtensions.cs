using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for EntityExtensions
    /// </summary>
    public static class EntityExtensions
    {
        public static string BuildUpdateChildrenParametersVariables(this IEntity entity)
        {
            return entity.BuildUpdateChildrenParametersVariables(true);
        }

        public static string BuildUpdateChildrenParametersVariables(this IEntity entity, bool includeConnectionParameter)
        {
            var parameters = "this";
            var thisKey = String.Format(", {0}", Util.NamingConventions.VariableName(entity.Name));
            bool isFirst = true;

            // AssociatedOneToMany, contains properties that need to be passed into child entity update/insert.
            // Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne), contains properties in the current entity.
            
            //In child class. Check its associations.. (Item)
            foreach (Association association in entity.Associations.Where(a => a.AssociationType == AssociationType.OneToMany))
            {
                //Child's Parameter List. (Product, supplier)
                foreach (Association childAssociation in GetRelatedEntity(association.Properties[0]).Associations.Where(a => a.AssociationType == AssociationType.OneToMany))
                {
                    // see if we already passed in the param.
                    var childParameter = String.Format(", {0}", Util.NamingConventions.VariableName(childAssociation.Properties[0].Property.Name));
                    if (!(isFirst && thisKey.Equals(childParameter)))
                    {
                        // look it up or append null..
                        parameters += GetRelatedTableParameter(entity, childAssociation);
                    }
                    else
                    {
                        isFirst = false;
                    }
                }
            }

            if (includeConnectionParameter)
                parameters += ", connection";

            return parameters;
        }

        private static IEntity GetRelatedEntity(AssociationProperty associationProperty)
        {
            //foreach (IEntity e in associationProperty.Association.Entity.EntityManager.Entities)
            //{
            //    if (e.EntityKeyName == associationProperty.ForeignProperty.Entity.EntityKeyName) return e;
            //}

            return associationProperty.Association.Entity;
        }

        private static string GetRelatedTableParameter(IEntity entity, Association association)
        {
            //AssociationProperty childMember = association[0];
            //foreach (var searchCriteria in entity.SearchCriteria(SearchCriteriaType.ForeignKey))
            //{
            //    bool found = false;
            //    foreach (IProperty property in searchCriteria.Properties)
            //    {
            //        if (childMember.ForeignProperty.ForeignProperty.Equals(property.ForeignProperty) || childMember.ForeignProperty.Equals(property.ForeignProperty))
            //        {
            //            found = true;
            //            foreach (Association association2 in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
            //            {
            //                foreach (AssociationProperty rootMember in association2)
            //                {
            //                    if (rootMember.Name.Equals(childMember.Name))
            //                    {
            //                        return String.Format(", {0}", Util.NamingConventions.VariableName(rootMember.Name));
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if(found)
            //    {
            //        return String.Format(", new {0}({1})", childMember.Name, GetRelatedEntity(childMember).Key.Properties.BuildPropertyVariableArguments());
            //    }
            //}

            return ", null";
        }
    }
}
