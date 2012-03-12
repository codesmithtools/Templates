using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationExtensions
    {
        public static string BuildPropertyVariableArguments(this Association association)
        {
            return association.BuildPropertyVariableArguments(true);
        }

        public static string BuildPropertyVariableArguments(this Association association, bool useAssociatedColumn)
        {
            string parameters = String.Empty;

            foreach (AssociationProperty property in association.Properties)
            {
                if (useAssociatedColumn)
                    parameters += String.Format(", {0}", property.ForeignProperty.Name);
                else
                    parameters += String.Format(", {0}", property.Property);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPropertyVariables(this Association association)
        {
            string parameters = String.Empty;

            foreach (AssociationProperty property in association.Properties)
            {
                parameters += property.ForeignProperty.Name;
            }

            return parameters;
        }

        public static string BuildArgumentVariables(this IEnumerable<Association> associations, bool includeConnectionParameter)
        {
            string parameters = String.Empty;

            foreach (var association in associations)
            {
                parameters += String.Format(", {0}", association.ForeignEntity.VariableName);
            }

            if (includeConnectionParameter)
                parameters += ", connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}