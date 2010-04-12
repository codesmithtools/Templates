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
            var parameters = "Me";
            var thisKey = string.Format(", {0}", Util.NamingConventions.VariableName(entity.ClassName));
            bool isFirst = true;

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
                            if (!(isFirst && thisKey.Equals(childParameter)))
                            {
                                // look it up or append null..
                                parameters += GetRelatedTableParameter(entity, childMember);
                            }
                            else
                            {
                                isFirst = false;
                            }
                        }
                    }
                }
            }

            if (includeConnectionParameter)
                parameters += ", connection";

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
                foreach (Member member in searchCriteria.Members)
                {
                    if (childMember.TableName.Equals(member.TableName) &&
                        childMember.ColumnName.Equals(member.ColumnName))
                    {
                        foreach (Association association in entity.AssociatedManyToOne)
                        {
                            foreach (AssociationMember rootMember in association)
                            {
                                if (rootMember.ClassName.Equals(childMember.ClassName))
                                {
                                    return string.Format(", {0}", Util.NamingConventions.VariableName(rootMember.ClassName));
                                }
                            }
                        }
                    }
                }

                return string.Format(", New {0}({1})", childMember.ClassName, searchCriteria.Members.BuildVariableArguments());
            }

            return ", Nothing";
        }
    }
}
