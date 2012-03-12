using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberCollectionExtensions
    /// </summary>
    public static class MemberCollectionExtensions
    {
        public static string BuildObjectInitializer(this List<IProperty> members)
        {
            return members.BuildObjectInitializer(false);
        }

        public static string BuildObjectInitializer(this List<IProperty> members, bool isObjectFactory)
        {
            return members.BuildObjectInitializer(isObjectFactory, false);
        }

        public static string BuildObjectInitializer(this List<IProperty> members, bool isObjectFactory, bool usePropertyName)
        {
            return members.BuildObjectInitializer(isObjectFactory, usePropertyName, false);
        }

        public static string BuildObjectInitializer(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal)
        {
            return members.BuildObjectInitializer(isObjectFactory, usePropertyName, includeOriginal, "criteria.");
        }

        public static string BuildObjectInitializer(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal, string prefix)
        {
            string parameters = String.Empty;

            foreach (var property in members)
            {
                var propertyName = isObjectFactory ? 
                                String.Format("item.{0}", property.Name) : 
                                usePropertyName ? property.Name : property.VariableName;

                if (includeOriginal && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity))
                    propertyName = isObjectFactory ? String.Format("item.Original{0}", property.Name) : String.Format("Original{0}", property.Name);
                parameters += string.Format("\r\n            {3}{0} = {1}{2}", property.Name, propertyName, property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte()" ? ".Value" : String.Empty, prefix);
            }

            return parameters.TrimStart(new[] { '\r', '\n', ',' });
        }

        public static string BuildParametersVariables(this List<IProperty> members)
        {
            return members.BuildParametersVariables(true);
        }

        public static string BuildParametersVariables(this List<IProperty> members, bool isNullable)
        {
            string parameters = String.Empty;

            foreach (var property in members)
            {
                string systemType = isNullable ? property.SystemType : property.SystemType.TrimEnd(new[] { '?' });
                parameters += string.Format(", ByVal {0} As {1}", property.VariableName, systemType);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildCommandParameters(this List<IProperty> members)
        {
            return BuildCommandParameters(members, false);
        }

        public static string BuildCommandParameters(this List<IProperty> members, bool isObjectFactory)
        {
            return BuildCommandParameters(members, isObjectFactory, false);
        }

        public static string BuildCommandParameters(this List<IProperty> members, bool isObjectFactory, bool usePropertyName)
        {
            return BuildCommandParameters(members, isObjectFactory, usePropertyName, false);
        }

        public static string BuildCommandParameters(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool isChildInsertUpdate)
        {
            return members.BuildCommandParameters(isObjectFactory, usePropertyName, isChildInsertUpdate, false);
        }

        public static string BuildCommandParameters(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool isChildInsertUpdate, bool includeOutPutParameters)
        {
            return members.BuildCommandParameters(isObjectFactory, usePropertyName, isChildInsertUpdate, includeOutPutParameters, false);
        }

        public static string BuildCommandParameters(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool isChildInsertUpdate, bool includeOutPutParameters, bool isUpdateStatement)
        {
            string commandParameters = String.Empty;
            string castPrefix = isObjectFactory ? "item." : String.Empty;

            foreach (var property in members)
            {
                string className = String.Empty;
                string includeThisPrefix = !isObjectFactory ? "Me." : String.Empty;
                string propertyName = property.Name;
                string originalPropertyName = String.Format("Original{0}", property.Name);
                
                // Resolve property Name from relationship.
                if (isChildInsertUpdate && property.IsType(PropertyType.Foreign))
                {
                    foreach (Association association in property.Entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
                    {
                        foreach (AssociationProperty associationProperty in association.Properties)
                        {
                            if (property.KeyName == associationProperty.Property.KeyName && property == associationProperty.Property)
                            {
                                propertyName = String.Format("{0}.{1}", associationProperty.ForeignProperty.Entity.VariableName, associationProperty.ForeignProperty.Name);

                                var format = associationProperty.ForeignProperty.IsType(PropertyType.Key) && !associationProperty.ForeignProperty.IsType(PropertyType.Identity) ? "{0}.Original{1}" : "{0}.{1}";
                                originalPropertyName = String.Format(format, associationProperty.ForeignProperty.Entity.VariableName, associationProperty.ForeignProperty.Name);

                                className = associationProperty.ForeignProperty.Entity.VariableName;
                                includeThisPrefix = String.Empty;
                                break;
                            }
                        }   
                    }
                }

                var nullableType = String.Format("{0}{1}", !isObjectFactory ? "Me." : String.Empty, property.Name);
                var originalNullableType = String.Format("{0}Original{1}", !isObjectFactory ? "Me." : String.Empty, property.Name);
                //var nullableType = string.Format("New {0}()", property.SystemType);
                //if (property.SystemType == "System.String" || property.SystemType == "System.Byte()")
                //    nullableType = "Nothing";

                string originalCast;
                string cast;
                if (property.IsNullable)
                {
                    //includeThisPrefix = this.
                    //castprefix = item.
                    //propertyName = bo.propertyname or propertyname
                    if (!string.IsNullOrEmpty(className) && property.SystemType != "System.Byte()")
                    {
                        cast = string.Format("ADOHelper.NullCheck(If(Not({3} Is Nothing), {0}{1}{2}, {4})))", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = string.Format("ADOHelper.NullCheck(If(Not({3} Is Nothing), {0}{1}{2}, {4})))", includeThisPrefix, castPrefix, originalPropertyName, className, originalNullableType);
                    }
                    else
                    {
                        cast = string.Format("ADOHelper.NullCheck({0}{1}{2}))", includeThisPrefix, castPrefix, propertyName);
                        originalCast = string.Format("ADOHelper.NullCheck({0}{1}{2}))", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(className) && property.SystemType != "System.Byte()")
                    {
                        cast = string.Format("If(Not({3} Is Nothing), {0}{1}{2}, {4}))", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = string.Format("If(Not({3} Is Nothing), {0}{1}{2}, {4}))", includeThisPrefix, castPrefix, originalPropertyName, className, originalNullableType);
                    }
                    else
                    {
                        cast = string.Format("{0}{1}{2})", includeThisPrefix, castPrefix, propertyName);
                        originalCast = string.Format("{0}{1}{2})", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }

                if (property.IsBinarySqlDbType())
                {
                    commandParameters += String.Format(Environment.NewLine + "                command.Parameters.Add(\"{0}{1}\", {2}, {3}).Value = {4}", Configuration.Instance.ParameterPrefix, property.KeyName, property.GetBinarySqlDbType(), property.Size, cast.Substring(0, cast.Length - 1));

                    continue;
                }

                bool includeOriginalPropertyName = isUpdateStatement && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity);
                if (isUpdateStatement && includeOriginalPropertyName)
                    commandParameters += String.Format(Environment.NewLine + "                command.Parameters.AddWithValue(\"{0}Original{1}\", {2}", Configuration.Instance.ParameterPrefix, property.KeyName, originalCast);

                commandParameters += String.Format(Environment.NewLine + "                command.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, property.KeyName, cast);

                if ((property.IsType(PropertyType.Identity) || (property.IsDbType(DbType.Guid) && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Foreign))) && includeOutPutParameters)
                {
                    if(isUpdateStatement)
                        commandParameters += String.Format(Environment.NewLine + "                command.Parameters(\"{0}{1}\").Direction = ParameterDirection.Input", Configuration.Instance.ParameterPrefix, property.KeyName);
                    else
                        commandParameters += String.Format(Environment.NewLine + "                command.Parameters(\"{0}{1}\").Direction = ParameterDirection.Output", Configuration.Instance.ParameterPrefix, property.KeyName);
                }
            }

            return commandParameters.TrimStart(new[] { ' ', '\r', '\n' });
        }

        public static string BuildHasValueCommandParameters(this List<IProperty> members)
        {
            string commandParameters = String.Empty;

            foreach (var property in members)
            {
                if (property.IsNullable)
                    commandParameters += String.Format(Environment.NewLine + "                command.Parameters.AddWithValue(\"{0}{1}HasValue\", criteria.{2}HasValue)", Configuration.Instance.ParameterPrefix, property.KeyName, property.Name);
            }

            return commandParameters.TrimStart(new[] { ' ', '\r', '\n' });
        }

        public static string BuildIdentityKeyEqualityStatements(this List<IProperty> members)
        {
            return members.BuildIdentityKeyEqualityStatements("");
        }

        public static string BuildIdentityKeyEqualityStatements(this List<IProperty> members, string prefix)
        {
            if (members == null || members.Count == 0) return String.Empty;

            string statement = String.Empty;

            foreach (var property in members)
            {
                if (property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity))
                    statement += string.Format(" Or Not {1}Original{0} = {1}{0}", property.Name, prefix);
            }

            if (String.IsNullOrWhiteSpace(statement))
                return String.Empty;

            return statement.Substring(3, statement.Length - 3).TrimStart(new[] { ' ' });
        }

        public static string BuildEqualityStatements(this List<IProperty> members, string prefix)
        {
            if (members == null || members.Count == 0)
                return String.Empty;

            string statement = String.Empty;
            foreach (var property in members)
            {
                statement += String.Format(" AndAlso {0}.{1} = {2}", prefix, property.Name, property.VariableName);
            }

            return statement.Substring(8, statement.Length - 8).TrimStart(new[] { ' ' });
        }
    }
}
