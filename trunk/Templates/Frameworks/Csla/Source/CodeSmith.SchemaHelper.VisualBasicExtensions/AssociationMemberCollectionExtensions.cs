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
                    var parameter = String.Format(", ByVal {0} As {1}", property.ForeignProperty.Entity.VariableName, property.ForeignProperty.Entity.Name);

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