using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationMemberCollectionExtensions
    /// </summary>
    public static class AssociationMemberCollectionExtensions
    {
        public static List<SearchCriteria> ListSearchCriteria(this AssociationMember member)
        {
            return member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(member.Name)).ToList();
        }

        public static string BuildObjectInitializer(this AssociationMember member)
        {
            return string.Format("{0} = {1}", member.PropertyName, member.VariableName);
        }

        public static Entity AssociationEntity(this AssociationMember member)
        {
            return new Entity(member.Table);
        }

        public static List<SearchCriteria> AssociationEntityListSearchCriteria(this AssociationMember member)
        {
            return  member.AssociationEntity().SearchCriteria
                    .Where(sc =>
                        sc.MethodName.EndsWith(member.LocalColumn.Name) ||
                        sc.MethodName.EndsWith(member.LocalColumn.ColumnName) ||
                        sc.MethodName.EndsWith(member.Name) ||
                        sc.MethodName.EndsWith(member.ColumnName) && 
                        !sc.IsUniqueResult)
                    .ToList();
        }

        public static string ResolveManyToOneNameConflict(this AssociationMember member, Entity entity)
        {
            string propertyName = Util.NamingConventions.PropertyName(member.ColumnName);

            foreach (AssociationMember association in entity.ManyToOne)
            {
                if(association.PropertyName == propertyName)
                {
                    return entity.ResolveCriteriaColumnName(member.ColumnName);
                }
            }

            return member.ColumnName;
        }
    }
}