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
            string parameters = string.Empty;

            foreach (var property in members)
            {
                if(property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") continue;

                var propertyName = isObjectFactory ? 
                                String.Format("item.{0}", property.Name) : 
                                usePropertyName ? property.Name : property.VariableName;

                if (includeOriginal && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity))
                    propertyName = isObjectFactory ? String.Format("item.Original{0}", property.Name) : String.Format("Original{0}", property.Name);
                
                parameters += String.Format(", {0} = {1}", property.Name, propertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildNullableObjectInitializer(this List<IProperty> members)
        {
            return members.BuildNullableObjectInitializer(false);
        }

        public static string BuildNullableObjectInitializer(this List<IProperty> members, bool isObjectFactory)
        {
            return members.BuildNullableObjectInitializer(isObjectFactory, false);
        }

        public static string BuildNullableObjectInitializer(this List<IProperty> members, bool isObjectFactory, bool usePropertyName)
        {
            return members.BuildNullableObjectInitializer(isObjectFactory, usePropertyName, false);
        }

        public static string BuildNullableObjectInitializer(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal)
        {
            return members.BuildNullableObjectInitializer(isObjectFactory, usePropertyName, includeOriginal, "criteria.");
        }

        public static string BuildNullableObjectInitializer(this List<IProperty> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal, string prefix)
        {
            string parameters = string.Empty;

            foreach (var property in members)
            {
                if ((property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") == false) continue;

                var propertyName = isObjectFactory ?
                                String.Format("item.{0}", property.Name) :
                                usePropertyName ? property.Name : property.VariableName;

                if (includeOriginal && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity))
                    propertyName = isObjectFactory ? String.Format("item.Original{0}", property.Name) : String.Format("Original{0}", property.Name);

                parameters += String.Format("\r\n                if({1}.HasValue) {2}{0} = {1}.Value;", property.Name, propertyName, prefix);
            }

            return parameters.TrimStart(new[] { '\r', '\n', ' ' });
        }

        public static string BuildParametersVariables(this List<IProperty> members)
        {
            return members.BuildParametersVariables(true);
        }

        public static string BuildParametersVariables(this List<IProperty> members, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (var property in members)
            {
                string systemType = isNullable ? property.SystemType : property.SystemType.TrimEnd(new[] { '?' });
                parameters += String.Format(", {0} {1}", systemType, property.VariableName);
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
            string commandParameters = string.Empty;
            string castPrefix = isObjectFactory ? "item." : string.Empty;

            foreach (var property in members)
            {
                string className = string.Empty;
                string includeThisPrefix = !isObjectFactory ? "this." : string.Empty;
                string propertyName = property.Name;
                string originalPropertyName = String.Format("Original{0}", property.Name);
                
                // Resolve property Name from relationship.
                if (isChildInsertUpdate && property.IsType(PropertyType.Foreign))
                {
                    foreach (Association association in property.Entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne))
                    {
                        foreach (AssociationProperty associationProperty in association.Properties)
                        {
                            if (property.KeyName == associationProperty.ForeignProperty.KeyName)// && property.ForeignProperty == associationProperty.ForeignProperty.ForeignProperty)
                            {
                                propertyName = String.Format("{0}.{1}", Util.NamingConventions.VariableName(associationProperty.Property.Name), associationProperty.Property.Name);

                                var format = associationProperty.Property.IsType(PropertyType.Key) && !associationProperty.Property.IsType(PropertyType.Identity) ? "{0}.Original{1}" : "{0}.{1}";
                                originalPropertyName = String.Format(format, Util.NamingConventions.VariableName(associationProperty.Property.Name), associationProperty.Property.Name);

                                className = Util.NamingConventions.VariableName(associationProperty.Property.Name);
                                includeThisPrefix = String.Empty;
                                break;
                            }
                        }
                    }
                }

                var nullableType = String.Format("{0}{1}", !isObjectFactory ? "this." : string.Empty, property.Name);
                var originalNullableType = String.Format("{0}Original{1}", !isObjectFactory ? "this." : string.Empty, property.Name);
                //var nullableType = String.Format("new {0}()", property.SystemType);
                //if (property.SystemType == "System.String" || property.SystemType == "System.Byte[]")
                //    nullableType = "null";

                string originalCast;
                string cast;
                if (property.IsNullable && property.SystemType != "System.Byte[]")
                {
                    //includeThisPrefix = this.
                    //castprefix = item.
                    //propertyName = bo.propertyname or propertyname
                    if (!string.IsNullOrEmpty(className))
                    {
                        cast = String.Format("ADOHelper.NullCheck({3} != null ? {0}{1}{2} : {4}));", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = String.Format("ADOHelper.NullCheck({3} != null ? {0}{1}{2} : {4}));", includeThisPrefix, castPrefix, originalPropertyName, className, originalNullableType);
                    }
                    else
                    {
                        cast = String.Format("ADOHelper.NullCheck({0}{1}{2}));", includeThisPrefix, castPrefix, propertyName);
                        originalCast = String.Format("ADOHelper.NullCheck({0}{1}{2}));", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(className))
                    {
                        cast = String.Format("{3} != null ? {0}{1}{2} : {4});", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = String.Format("{3} != null ? {0}{1}{2} : {4});", includeThisPrefix, castPrefix, originalPropertyName, className, originalNullableType);
                    }
                    else
                    {
                        cast = String.Format("{0}{1}{2});", includeThisPrefix, castPrefix, propertyName);
                        originalCast = String.Format("{0}{1}{2});", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }

                bool includeOriginalPropertyName = isUpdateStatement && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity);
                if (isUpdateStatement && includeOriginalPropertyName)
                    commandParameters += String.Format(Environment.NewLine + "                    command.Parameters.AddWithValue(\"{0}Original{1}\", {2}", Configuration.Instance.ParameterPrefix, property.KeyName, originalCast);

                commandParameters += String.Format(Environment.NewLine + "                    command.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, property.KeyName, cast);

                if ((property.IsType(PropertyType.Identity) || (property.IsDbType(DbType.Guid) && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Foreign))) && includeOutPutParameters)
                {
                    if (isUpdateStatement)
                        commandParameters += String.Format(Environment.NewLine + "                    command.Parameters[\"{0}{1}\"].Direction = ParameterDirection.Input;", Configuration.Instance.ParameterPrefix, property.KeyName);
                    else
                        commandParameters += String.Format(Environment.NewLine + "                    command.Parameters[\"{0}{1}\"].Direction = ParameterDirection.Output;", Configuration.Instance.ParameterPrefix, property.KeyName);
                }
            }

            return commandParameters.TrimStart(new[] { ' ', '\r', '\n' });
        }

        public static string BuildHasValueCommandParameters(this List<IProperty> members)
        {
            string commandParameters = string.Empty;

            foreach (var property in members)
            {
                if (property.IsNullable)
                    commandParameters += String.Format(Environment.NewLine + "                    command.Parameters.AddWithValue(\"{0}{1}HasValue\", criteria.{2}HasValue);", Configuration.Instance.ParameterPrefix, property.KeyName, property.Name);
            }

            return commandParameters.TrimStart(new[] { ' ', '\r', '\n' });
        }

        public static string BuildIdentityKeyEqualityStatements(this List<IProperty> members)
        {
            return members.BuildIdentityKeyEqualityStatements("");
        }

        public static string BuildIdentityKeyEqualityStatements(this List<IProperty> members, string prefix)
        {
            if (members == null || members.Count == 0) return string.Empty;

            string statement = string.Empty;

            foreach (var property in members)
            {
                if (property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity))
                    statement += String.Format(" || {1}Original{0} != {1}{0}", property.Name, prefix);
            }

            return statement.TrimStart(new[] { '|', ' ' });
        }
        //LinqToSQL MOdification
        /// <summary>
        /// 
        /// </summary>
        /// <param name="members"></param>
        /// <param name="usePropertyName"></param>
        /// <param name="isChildInsertUpdate"></param>
        /// <param name="isUpdateStatement"></param>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public static string BuildLinqToSQLCommandParameters(this List<IProperty> members, bool usePropertyName, bool isChildInsertUpdate, bool isUpdateStatement, int indentLevel)
        {
            string commandParameters = string.Empty;
            string tabLevel = new string((char)9,indentLevel);
            foreach (var property in members)
            {
                
                string includeThisPrefix =  "this." ;
                string propertyName = property.Name;
                string strPropertyName = property.Name;
                string originalPropertyName = isUpdateStatement && property.IsType(PropertyType.Key) && !property.IsType(PropertyType.Identity) ? String.Format("Original{0}", property.Name) : string.Empty;
                string columnName = property.KeyName;
                if (property.HasByteArrayColumn())
                {
                    strPropertyName = "LinqToSQLHelper.GetBinary(" + includeThisPrefix + propertyName + ")";
                }
                else
                {
                    strPropertyName = includeThisPrefix + propertyName;

                }
                commandParameters += String.Format("\r\n{0}item.{1} = {2};", tabLevel, propertyName, strPropertyName);
            }

            return commandParameters.TrimStart(new[] { '\r', '\n' });
        }
    }
}
