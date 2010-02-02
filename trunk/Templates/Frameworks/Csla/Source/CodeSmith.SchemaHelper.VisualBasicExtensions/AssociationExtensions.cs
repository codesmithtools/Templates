using System;

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
                            member.IsNullable && member.SystemType != "System.String" ? ".Value" : string.Empty);
                    }
                }
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t', ',', ' ' });
        }
    }
}