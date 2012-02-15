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
            // Associations.Where(a => a.AssociationType == AssociationType.ManyToOne), contains properties in the current entity.

            //In child class. Check its associations.. (Item)
            foreach (Association association in entity.AssociatedOneToMany)
            {
                //Child's Parameter List. (Product, supplier)
                foreach (Association childAssociation in GetRelatedEntity(association[0]).Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                {
                    // see if we already passed in the param.
                    var childParameter = string.Format(", {0}", Util.NamingConventions.VariableName(childAssociation[0].ClassName));
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

        private static Entity GetRelatedEntity(AssociationMember associationMember)
        {
            foreach (Entity e in associationMember.Entity.EntityManager.Entities)
            {
                if (e.Table.Name == associationMember.TableName) return e;
            }

            return associationMember.Entity;
        }

        private static string GetRelatedTableParameter(Entity entity, Association association)
        {
            AssociationMember childMember = association[0];
            foreach (var searchCriteria in entity.SearchCriteria(SearchCriteriaEnum.ForeignKey))
            {
                bool found = false;
                foreach (Member member in searchCriteria.Members)
                {
                    if (childMember.AssociatedColumn.TableName.Equals(member.TableName) || childMember.TableName.Equals(member.TableName))
                    {
                        found = true;
                        foreach (Association association2 in entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                        {
                            foreach (AssociationMember rootMember in association2)
                            {
                                if (rootMember.ClassName.Equals(childMember.ClassName))
                                {
                                    return string.Format(", {0}", Util.NamingConventions.VariableName(rootMember.ClassName));
                                }
                            }
                        }
                    }
                }

                if (found)
                {
                   return string.Format(", new {0}({1})", childMember.ClassName, GetRelatedEntity(childMember).PrimaryKey.KeyMembers.BuildPropertyVariableArguments());
                }
            }

            return ", Nothing";
        }
    }
}
