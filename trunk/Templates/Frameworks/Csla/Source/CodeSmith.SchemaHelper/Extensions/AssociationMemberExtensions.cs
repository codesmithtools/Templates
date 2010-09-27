using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationMemberCollectionExtensions
    /// </summary>
    public static class AssociationMemberExtensions
    {
        public static Entity AssociationEntity(this AssociationMember member)
        {
            using (CodeSmith.Engine.AssemblyResolver.Current.UseManagedAssemblyResolver)
            {
                return new Entity(member.Table);
            }
        }
    }
}