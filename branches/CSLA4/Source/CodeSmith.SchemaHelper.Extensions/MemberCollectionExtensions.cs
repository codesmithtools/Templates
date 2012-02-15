using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberCollectionExtensions
    /// </summary>
    public static class MemberCollectionExtensions
    {
        public static string BuildVariableArguments(this List<IProperty> members)
        {
            string parameters = string.Empty;

            foreach (IProperty property in members)
            {
                parameters += String.Format(", {0}{1}", property.VariableName, property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPropertyVariableArguments(this List<IProperty> members)
        {
            string parameters = string.Empty;

            foreach (IProperty property in members)
            {
                parameters += String.Format(", {0}{1}", property.Name, property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPrivateMemberVariableArguments(this List<IProperty> members)
        {
            string parameters = string.Empty;

            foreach (IProperty property in members)
            {
                parameters += String.Format(", {0}{1}", property.PrivateMemberVariableName, property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseColumns(this List<IProperty> members)
        {
            string columnNames = string.Empty;

            foreach (IProperty property in members)
            {
                columnNames += String.Format(", [{0}]", property.KeyName);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseParameters(this List<IProperty> members)
        {
            return BuildDataBaseParameters(members, new List<AssociationProperty>());
        }

        public static string BuildDataBaseParameters(this List<IProperty> members, List<AssociationProperty> associationMembers)
        {
            string columnNames = string.Empty;

            foreach (IProperty property in members)
            {
                columnNames += String.Format(", {0}{1}", Configuration.Instance.ParameterPrefix, property.KeyName);
            }

            foreach (AssociationProperty associationProperty in associationMembers)
            {
                if (!associationProperty.Property.IsType(PropertyType.Identity))
                {
                    string output = String.Format(", {0}{1}", Configuration.Instance.ParameterPrefix, associationProperty.Property.KeyName);

                    if (!columnNames.Contains(output))
                        columnNames += output;
                }
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildSetStatements(this List<IProperty> members)
        {
            string setStatements = "\t\t\t\t\t\t SET";

            foreach (IProperty property in members)
            {
                setStatements += String.Format(" [{0}] = {1},", property.KeyName, property.BuildParameterVariableName());
            }

            return setStatements.TrimStart(new[] { '\t', '\n' }).TrimEnd(new[] { ',' });
        }

        public static string BuildWhereStatements(this List<IProperty> members)
        {
            return members.BuildWhereStatements(false);
        }

        public static string BuildWhereStatements(this List<IProperty> members, bool isUpdate)
        {
            string columnNames = string.Empty;

            foreach (var property in members)
            {
                if (isUpdate && !property.IsType(PropertyType.Identity))
                    columnNames += String.Format("[{0}] = {1} AND ", property.KeyName, String.Format("{0}Original{1}", Configuration.Instance.ParameterPrefix, property.KeyName));
                else
                    columnNames += String.Format("[{0}] = {1} AND ", property.KeyName, property.BuildParameterVariableName());
            }

            return String.Format("WHERE {0}", columnNames.Remove(columnNames.Length - 5, 5));
        }
    }
}
