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
            string parameters = string.Empty;

            foreach (AssociationMember member in association)
            {
                parameters += string.Format(", {0}", member.AssociatedColumn.PropertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPropertyVariables(this Association association)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in association)
            {
                parameters += member.AssociatedColumn.PropertyName;
            }

            return parameters;
        }
    }
}