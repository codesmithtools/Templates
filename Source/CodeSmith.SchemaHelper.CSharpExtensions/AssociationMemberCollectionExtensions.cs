using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationMemberCollectionExtensions
    {
        public static string BuildParametersVariables(this List<IAssociation> associations)
        {
            return associations.BuildParametersVariables(false);
        }

        public static string BuildParametersVariables(this List<IAssociation> associations, bool includeConnectionParameter)
        {
            string parameters = String.Empty;

            foreach (var association in associations)
            {
                foreach (AssociationProperty property in association.Properties)
                {
                    var parameter = String.Format(", {0} {1}", property.ForeignProperty.Entity.Name, property.ForeignProperty.Entity.VariableName);

                    if (!parameters.Contains(parameter))
                        parameters += parameter;
                }
            }

            if (includeConnectionParameter)
                parameters += ", SqlConnection connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}