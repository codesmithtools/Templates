using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationCollectionExtensions
    {
        public static List<Association> Distinct(this List<Association> associations)
        {
            return associations.Distinct<Association>(new DistinctAssociationComparer()).ToList();
        }

        public class DistinctAssociationComparer : IEqualityComparer<Association>
        {
            public bool Equals(Association x, Association y)
            {
                return x.ClassName == y.ClassName;
            }

            public int GetHashCode(Association obj)
            {
                return obj.ClassName.GetHashCode();
            }
        }
    }
}