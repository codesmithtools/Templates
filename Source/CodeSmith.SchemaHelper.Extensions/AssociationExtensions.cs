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
            string parameters = string.Empty;

            foreach (AssociationMember member in association)
            {
                if (useAssociatedColumn)
                    parameters += string.Format(", {0}", member.AssociatedColumn.PropertyName);
                else
                    parameters += string.Format(", {0}", member.PropertyName);
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