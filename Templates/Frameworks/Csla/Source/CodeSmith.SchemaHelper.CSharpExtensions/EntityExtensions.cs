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
        public static string BuildUpdateChildrenParametersVariables(this Entity entity)
        {
            return entity.BuildUpdateChildrenParametersVariables(true);
        }

        public static string BuildUpdateChildrenParametersVariables(this Entity entity, bool includeConnectionParameter)
        {
            var parameters = "this";
            var thisKey = string.Format(", {0}", Util.NamingConventions.VariableName(entity.ClassName));

            // AssociatedOneToMany, contains properties that need to be passed into child entity update/insert.
            // AssociatedManyToOne, contains properties in the current entity.
            
            foreach (Association association in entity.AssociatedOneToMany)
            {
                //In child class. Check its associations.. (Item)
                foreach (AssociationMember member in association)
                {
                    //Child's Parameter List. (Product, supplier)
                    foreach (Association childAssociation in GetRelatedEntity(member).AssociatedManyToOne)
                    {
                        foreach (AssociationMember childMember in childAssociation)
                        {
                            // see if we already passed in the param.
                            var childParameter = string.Format(", {0}", Util.NamingConventions.VariableName(childMember.ClassName));
                            if (!parameters.Contains(childParameter) || thisKey.Equals(childParameter))
                            {
                                // look it up or append null..
                                childParameter = GetRelatedTableParameter(entity, childMember);
                                if ((!parameters.Contains(childParameter) && thisKey.Equals(childParameter)) || (childParameter.Equals(", null") && !parameters.EndsWith(", null")))
                                    parameters += childParameter;
                            }
                        }
                    }
                }
            }

            if (includeConnectionParameter)
                parameters += ", connection";

            foreach (Association association in entity.AssociatedManyToOne)
            {
                foreach (AssociationMember member in association)
                {
                    var parameter = string.Format(", {0}", Util.NamingConventions.VariableName(member.ClassName));

                    if (!parameters.Contains(parameter))
                        parameters += parameter;
                }
            }

            return parameters;
        }

        private static Entity GetRelatedEntity(AssociationMember associationMember)
        {
            foreach (Entity e in associationMember.Entity.EntityManager.Entities)
            {
                if (e.Table.Name == associationMember.TableName) return e;
            }

            return associationMember.Entity;
        }

        private static string GetRelatedTableParameter(Entity entity, AssociationMember childMember)
        {
            foreach (var searchCriteria in entity.SearchCriteria(SearchCriteriaEnum.ForeignKey))
            {
                if (searchCriteria.Members.Any(member => childMember.TableName.Equals(member.TableName) && childMember.ColumnName.Equals(member.ColumnName)))
                {
                    return string.Format(", new {0}({1})", childMember.ClassName, searchCriteria.Members.BuildVariableArguments());
                }
            }

            return ", null";
        }
    }
}
