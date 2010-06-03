using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationExtensions
    {
        public static string BuildObjectInitializer(this Association association)
        {
            return association.BuildObjectInitializer(false);
        }

        public static string BuildObjectInitializer(this Association association, bool usePropertyName)
        {
            string parameters = string.Empty;

            foreach (Member member in association.SearchCriteria.Members)
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        parameters += string.Format("\r\n\t\t\t\tcriteria.{0} = {1}{2}", Util.NamingConventions.PropertyName(associationMember.ColumnName),
                            usePropertyName ? member.PropertyName : member.VariableName,
                            member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()" ? ".Value" : string.Empty);
                    }
                }
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t', ',', ' ' });
        }

        public static string BuildParametersVariables(this Association association)
        {
            return association.BuildParametersVariables(false);
        }

        public static string BuildParametersVariables(this Association association, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in association)
            {
                var parameter = string.Format(", ByVal {1} As {0}", member.ClassName, Util.NamingConventions.VariableName(member.ClassName));

                if (!parameters.Contains(parameter))
                    parameters += parameter;
            }

            if (includeConnectionParameter)
                parameters += ", ByVal connection As SqlConnection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildUpdateStatementVariables(this Association association, List<Association> associations, int currentRecord, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            for (int index = 0; index < associations.Count; index++)
            {
                var parameter = string.Format(", {0}", Util.NamingConventions.VariableName(associations[index].ClassName));
                if (parameters.Contains(parameter)) continue;
                
                if(index == currentRecord)
                {
                    parameters += parameter;
                }
                else
                {
                    parameters += ", Nothing";
                }
            }

            if (includeConnectionParameter)
                parameters += ", connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}