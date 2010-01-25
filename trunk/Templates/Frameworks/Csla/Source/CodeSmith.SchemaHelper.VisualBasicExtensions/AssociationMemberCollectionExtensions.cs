using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationMemberCollectionExtensions
    {
        public static string BuildParametersVariables(this List<Association> associations)
        {
            string parameters = string.Empty;

            foreach (Association association in associations)
            {
                foreach (AssociationMember member in association)
                {
                    parameters += string.Format(", ByVal {0} As {1}", Util.NamingConventions.VariableName(member.ClassName), member.ClassName);
                }
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}