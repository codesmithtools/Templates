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
                    var parameter = String.Format(", {0} {1}", property.Property.Name, Util.NamingConventions.VariableName(property.Property.Name));

                    if (!parameters.Contains(parameter))
                        parameters += parameter;
                }
            }

            if (includeConnectionParameter)
                parameters += ", var connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}