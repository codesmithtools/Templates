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
            return associations.BuildParametersVariables(false);
        }

        public static string BuildParametersVariables(this List<Association> associations, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            foreach (Association association in associations)
            {
                foreach (AssociationProperty property in association.Properties)
                {
                    var parameter = string.Format(", ByVal {0} As {1}", Util.NamingConventions.VariableName(property.Property.Name), property.Property.Name);

                    if (!parameters.Contains(parameter))
                        parameters += parameter;
                }
            }

            if (includeConnectionParameter)
                parameters += ", ByVal connection As SqlConnection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}