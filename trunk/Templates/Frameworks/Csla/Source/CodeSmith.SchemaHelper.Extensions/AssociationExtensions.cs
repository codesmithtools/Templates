using System;

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
    }
}